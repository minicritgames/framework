using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Events;
using Debug = UnityEngine.Debug;

namespace Minikit
{
    public class MKLoadStep
    {
        public string label { get; private set; }
        public double milliseconds { get; internal set; }
        public double longestSliceMilliseconds { get; internal set; }
        public int frames { get; internal set; }
        public bool failed { get; internal set; }

        private readonly Func<IEnumerator> routine;


        public MKLoadStep(string _label, Func<IEnumerator> _routine)
        {
            label = _label;
            routine = _routine;
        }

        public MKLoadStep(string _label, Action _action)
        {
            label = _label;
            routine = () => DoAction(_action);
        }

        private static IEnumerator DoAction(Action _action)
        {
            _action?.Invoke();
            yield break;
        }

        internal IEnumerator CreateRoutine()
        {
            return routine?.Invoke();
        }
    }

    public class MKLoadSequence
    {
        // Yield this from a step to always wait for the next frame; a yielded null only gives up the frame once the budget is spent
        public static readonly object nextFrame = new();

        public UnityEvent<MKLoadSequence, MKLoadStep> OnStepStarted = new();
        public UnityEvent<MKLoadSequence, MKLoadStep> OnStepFinished = new();
        public UnityEvent<MKLoadSequence> OnFinished = new();

        public string name { get; private set; }
        public float frameBudgetMilliseconds = 10f;
        public MKLoadStep currentStep { get; private set; }
        public bool isRunning { get; private set; }
        public bool isFinished { get; private set; }
        public bool isCancelled { get; private set; }
        public double milliseconds { get; private set; }
        public IReadOnlyList<MKLoadStep> steps => stepList;

        private readonly List<MKLoadStep> stepList = new();
        private readonly Stopwatch sliceStopwatch = new();
        private readonly Stopwatch stepStopwatch = new();
        private readonly Stopwatch totalStopwatch = new();
        private int nextStepIndex;
        private Coroutine coroutine;


        public MKLoadSequence(string _name)
        {
            name = _name;
        }

        public MKLoadStep Add(string _label, Action _action)
        {
            return Add(new MKLoadStep(_label, _action));
        }

        public MKLoadStep Add(string _label, Func<IEnumerator> _routine)
        {
            return Add(new MKLoadStep(_label, _routine));
        }

        // Steps may be added while the sequence runs, so listeners of a started load can queue their own work
        public MKLoadStep Add(MKLoadStep _step)
        {
            stepList.Add(_step);
            return _step;
        }

        public void Start()
        {
            if (isRunning
                || isFinished)
            {
                return;
            }

            isRunning = true;
            coroutine = MKTimerManager.instance.StartCoroutine(DoRun());
        }

        // Runs every step to the end within this call; steps may only yield null, the next frame marker or nested iterators
        public void RunImmediately()
        {
            if (isRunning
                || isFinished)
            {
                return;
            }

            isRunning = true;
            totalStopwatch.Restart();
            while (!isCancelled
                && nextStepIndex < stepList.Count)
            {
                MKLoadStep step = BeginStep();
                IEnumerator routine = step.CreateRoutine();
                if (routine != null)
                {
                    DrainImmediately(step, routine);
                }
                if (isFinished)
                {
                    return;
                }
                EndStep(step);
            }
            Finish();
        }

        public void Cancel()
        {
            if (isFinished)
            {
                return;
            }

            isCancelled = true;
            if (coroutine != null
                && MKTimerManager.instanceExists)
            {
                MKTimerManager.instance.StopCoroutine(coroutine);
            }
            coroutine = null;
            if (currentStep != null)
            {
                EndStep(currentStep);
            }
            Finish();
        }

        private IEnumerator DoRun()
        {
            totalStopwatch.Restart();
            sliceStopwatch.Restart();
            while (!isCancelled
                && nextStepIndex < stepList.Count)
            {
                MKLoadStep step = BeginStep();
                IEnumerator routine = step.CreateRoutine();
                while (routine != null)
                {
                    bool more;
                    try
                    {
                        more = routine.MoveNext();
                    }
                    catch (Exception exception)
                    {
                        Debug.LogException(exception);
                        step.failed = true;
                        more = false;
                    }

                    if (isFinished)
                    {
                        yield break;
                    }

                    RecordSlice(step);
                    if (!more)
                    {
                        break;
                    }

                    object yielded = routine.Current;
                    if (yielded == null
                        && sliceStopwatch.Elapsed.TotalMilliseconds < frameBudgetMilliseconds)
                    {
                        continue;
                    }

                    step.frames++;
                    yield return yielded == nextFrame ? null : yielded;
                    sliceStopwatch.Restart();
                }
                EndStep(step);

                if (sliceStopwatch.Elapsed.TotalMilliseconds >= frameBudgetMilliseconds)
                {
                    yield return null;
                    sliceStopwatch.Restart();
                }
            }
            coroutine = null;
            Finish();
        }

        private MKLoadStep BeginStep()
        {
            MKLoadStep step = stepList[nextStepIndex++];
            currentStep = step;
            stepStopwatch.Restart();
            OnStepStarted.Invoke(this, step);
            return step;
        }

        private void EndStep(MKLoadStep _step)
        {
            _step.milliseconds = stepStopwatch.Elapsed.TotalMilliseconds;
            currentStep = null;
            OnStepFinished.Invoke(this, _step);
        }

        private void RecordSlice(MKLoadStep _step)
        {
            _step.longestSliceMilliseconds = Math.Max(_step.longestSliceMilliseconds, sliceStopwatch.Elapsed.TotalMilliseconds);
        }

        private void DrainImmediately(MKLoadStep _step, IEnumerator _routine)
        {
            Stack<IEnumerator> stack = new();
            stack.Push(_routine);
            while (stack.Count > 0)
            {
                IEnumerator top = stack.Peek();
                bool more;
                try
                {
                    more = top.MoveNext();
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                    _step.failed = true;
                    return;
                }

                if (!more)
                {
                    stack.Pop();
                    continue;
                }

                object yielded = top.Current;
                if (yielded is IEnumerator nested)
                {
                    stack.Push(nested);
                }
                else if (yielded != null
                    && yielded != nextFrame)
                {
                    Debug.LogError($"Load step '{_step.label}' yielded {yielded.GetType().Name}, which can't be waited on when running immediately");
                }
            }
        }

        private void Finish()
        {
            milliseconds = totalStopwatch.Elapsed.TotalMilliseconds;
            isRunning = false;
            isFinished = true;
            OnFinished.Invoke(this);
        }
    }
} // Minikit namespace

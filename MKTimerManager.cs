using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Minikit
{
    public class MKTimerHandle_Tick
    {
        private Action action;
        private float timeOfStart;
        private float duration;
        public bool cancelRequested { get; private set; } = false;
        public bool isCompleted => Time.time > timeOfStart + duration;
        public float timeLeft => Mathf.Clamp(Time.time - timeOfStart + duration, 0f, duration);


        public MKTimerHandle_Tick(float _duration, Action _action)
        {
            action = _action;
            timeOfStart = Time.time;
            duration = _duration;
        }


        public void Cancel()
        {
            cancelRequested = true;
        }

        public void Finish()
        {
            Cancel();
            action?.Invoke();
        }
    }

    public class MKTimerHandle_Recurring
    {
        private Action action;
        private float interval;
        private float nextFireTime;
        public bool cancelRequested { get; private set; } = false;
        public bool shouldFire => !cancelRequested && Time.time >= nextFireTime;


        public MKTimerHandle_Recurring(float _interval, Action _action)
        {
            action = _action;
            interval = _interval;
            nextFireTime = Time.time + interval;
        }


        public void Cancel()
        {
            cancelRequested = true;
        }

        public void Fire()
        {
            action?.Invoke();
            // Schedule from now rather than from the prior fire time so a long frame doesn't backlog catch-up fires.
            nextFireTime = Time.time + interval;
        }
    }

    public class MKTimerHandle_Coroutine
    {
        private Coroutine coroutine;
        private Action action;


        public MKTimerHandle_Coroutine(Coroutine _coroutine, Action _action)
        {
            coroutine = _coroutine;
            action = _action;
        }

        
        public void Cancel()
        {
            MKTimerManager.instance.StopCoroutine(coroutine);
        }

        public void Finish()
        {
            Cancel();
            action?.Invoke();
        }
    }

    public class MKTimerManager : MonoBehaviour
    {
        [HideInInspector] public UnityEvent OnUpdate = new();
        [HideInInspector] public UnityEvent OnFixedUpdate = new();
        [HideInInspector] public UnityEvent OnLateUpdate = new();
        
        protected List<MKTimerHandle_Tick> timerHandles = new();
        protected List<MKTimerHandle_Recurring> recurringHandles = new();

        protected static MKTimerManager __instance;
        public static MKTimerManager instance
        {
            get
            {
                if (!__instance)
                {
                    GameObject timerManagerGO = new("TimerManager");
                    __instance = timerManagerGO.AddComponent<MKTimerManager>();
                }
                
                return __instance;
            }
        }

        
        protected virtual void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        protected virtual void Update()
        {
            foreach (MKTimerHandle_Tick timerHandle in timerHandles.ToArray())
            {
                if (timerHandle.cancelRequested)
                {
                    timerHandles.Remove(timerHandle);
                    continue;
                }

                if (timerHandle.isCompleted)
                {
                    timerHandle.Finish();
                    timerHandles.Remove(timerHandle);
                    continue;
                }
            }

            foreach (MKTimerHandle_Recurring recurringHandle in recurringHandles.ToArray())
            {
                if (recurringHandle.cancelRequested)
                {
                    recurringHandles.Remove(recurringHandle);
                    continue;
                }

                if (recurringHandle.shouldFire)
                {
                    recurringHandle.Fire();
                }
            }

            OnUpdate?.Invoke();
        }

        protected virtual void FixedUpdate()
        {
            OnFixedUpdate?.Invoke();
        }

        protected virtual void LateUpdate()
        {
            OnLateUpdate?.Invoke();
        }
        

        public MKTimerHandle_Tick NewTimer_Tick(float _delay, Action _action)
        {
            MKTimerHandle_Tick timerHandle = new MKTimerHandle_Tick(_delay, _action);
            timerHandles.Add(timerHandle);
            return timerHandle;
        }

        public MKTimerHandle_Recurring NewTimer_Recurring(float _interval, Action _action)
        {
            MKTimerHandle_Recurring recurringHandle = new MKTimerHandle_Recurring(_interval, _action);
            recurringHandles.Add(recurringHandle);
            return recurringHandle;
        }

        public MKTimerHandle_Coroutine NewTimer_Coroutine(float _delay, Action _action)
        {
            return new MKTimerHandle_Coroutine(StartCoroutine(DoCoroutineTimer(_delay, _action)), _action);
        }

        private IEnumerator DoCoroutineTimer(float _delay, Action _action)
        {
            yield return new WaitForSeconds(_delay);

            _action?.Invoke();
        }

        public MKTimerHandle_Coroutine NewTimer_Coroutine_NextFrame(Action _action)
        {
            return NewTimer_Coroutine_Frames(1, _action);
        }

        public MKTimerHandle_Coroutine NewTimer_Coroutine_Frames(int _frames, Action _action)
        {
            return new MKTimerHandle_Coroutine(StartCoroutine(DoCoroutineTimer_Frames(_frames, _action)), _action);
        }

        private IEnumerator DoCoroutineTimer_Frames(int _frames, Action _action)
        {
            if (_frames > 0)
            {
                for (int i = 0; i < _frames; i++)
                {
                    yield return new WaitForEndOfFrame();
                }
            }

            _action?.Invoke();
        }
    }
} // Minicrit namespace

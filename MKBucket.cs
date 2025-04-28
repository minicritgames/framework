using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;

namespace Minikit
{
    public class MKBucket<TKey, TValue>
    {
        public int Count => bucketEntriesByKey.Count;

        protected Dictionary<TKey, TValue> bucketEntriesByKey = new();


        public MKBucket()
        {
        }


        /// <summary> Attempts to add an entry to the bucket. Returns false if the bucket already has the given key. </summary>
        public virtual bool Add(TKey _key, TValue _value)
        {
            return bucketEntriesByKey.TryAdd(_key, _value);
        }

        /// <summary> Attempts to remove an entry from the bucket by key. Returns false if the bucket doesn't have the given key. </summary>
        public virtual bool Remove(TKey _key)
        {
            if (bucketEntriesByKey.ContainsKey(_key))
            {
                return bucketEntriesByKey.Remove(_key);
            }

            return false;
        }

        public virtual bool Has(TKey _key)
        {
            return bucketEntriesByKey.ContainsKey(_key);
        }
    }

    public class MKTruthBucket<TKey> : MKBucket<TKey, bool>
    {
        public virtual bool AnyTruth()
        {
            return bucketEntriesByKey.Values.FirstOrDefault(b => b);
        }

        public virtual bool NoTruth()
        {
            return bucketEntriesByKey.Values.FirstOrDefault(b => b) == false;
        }
    }

    public class MKStackBucket<TKey, TValue> : MKBucket<TKey, TValue>
    {
        public UnityEvent<TValue> OnTopValueChanged = new();

        private KeyValuePair<TKey, TValue> topPair;


        public bool Peek(out KeyValuePair<TKey, TValue> _outPair)
        {
            if (topPair.Key != null)
            {
                _outPair = topPair;
                return true;
            }

            _outPair = new KeyValuePair<TKey, TValue>(default(TKey), default(TValue));
            return false;
        }

        public bool Pop(out KeyValuePair<TKey, TValue> _outPair)
        {
            if (bucketEntriesByKey.ContainsKey(topPair.Key))
            {
                if (Remove(topPair.Key))
                {
                    _outPair = topPair;
                    return true;
                }
            }

            _outPair = new KeyValuePair<TKey, TValue>(default(TKey), default(TValue));
            return false;
        }

        public bool Push(TKey _key, TValue _value)
        {
            return Add(_key, _value);
        }

        public override bool Add(TKey _key, TValue _value)
        {
            if (base.Add(_key, _value))
            {
                topPair = new KeyValuePair<TKey, TValue>(_key, _value);
                OnTopValueChanged.Invoke(topPair.Value);

                return true;
            }
            else
            {
                return false;
            }
        }

        public override bool Remove(TKey _key)
        {
            if (base.Remove(_key))
            {
                bool topPairChanged = false;

                // Nullify our cached top value if we just removed it
                if (topPair.Key.Equals(_key))
                {
                    topPair = new KeyValuePair<TKey, TValue>(default(TKey), default(TValue));
                    topPairChanged = true;
                }

                // If our cached top value is null, update it to whatever was most recently added to the bucket (if anything)
                if (topPair.Key == null
                    && bucketEntriesByKey.Count > 0)
                {
                    topPair = bucketEntriesByKey.ElementAt(bucketEntriesByKey.Count - 1);
                    topPairChanged = true;
                }

                if (topPairChanged)
                {
                    OnTopValueChanged.Invoke(topPair.Value);
                }

                return true;
            }
            else
            {
                return false;
            }
        }
    }

    /// <summary> Does not allow entries with duplicate priorities due to the nature of SortedList. </summary>
    /// <typeparam name="TPriority"> The type to use as the priority comparer - int is allowed </typeparam>
    public class MKPriorityBucket<TKey, TValue, TPriority>
    {
        public UnityEvent<TKey> OnHighestPriorityKeyChanged = new();
        public UnityEvent<TValue> OnHighestPriorityValueChanged = new();

        public int Count => sortedList.Count;

        private TKey lastHighestPriorityKey;
        private TValue lastHighestPriorityValue;
        private SortedList<TPriority, KeyValuePair<TKey, TValue>> sortedList = new();


        public MKPriorityBucket()
        {

        }


        /// <summary> Attempts to add an entry to the bucket. Returns false if the bucket already has the given key. </summary>
        public virtual bool Add(TKey _key, TValue _value, TPriority _priority)
        {
            foreach (KeyValuePair<TPriority, KeyValuePair<TKey, TValue>> pair in sortedList.ToArray())
            {
                if (pair.Value.Key.Equals(_key)) // If we already have an entry for this key
                {
                    if (!pair.Key.Equals(_priority))
                    {
                        sortedList.Remove(pair.Key); // Remove the entry with the old priority
                        sortedList.Add(_priority, new KeyValuePair<TKey, TValue>(_key, _value));
                        OnSortedListChanged();

                        return true;
                    }
                    else
                    {
                        sortedList[pair.Key] = new KeyValuePair<TKey, TValue>(_key, _value);
                        OnSortedListChanged();

                        return true;
                    }
                }
            }

            // If we already have this level of priority, don't add this entry
            if (sortedList.ContainsKey(_priority))
            {
                return false;
            }

            sortedList.Add(_priority, new KeyValuePair<TKey, TValue>(_key, _value));
            OnSortedListChanged();

            return true;
        }

        /// <summary> Attempts to remove an entry from the bucket by key. Returns false if the bucket doesn't have the given key. </summary>
        public virtual bool Remove(TKey _key)
        {
            foreach (KeyValuePair<TPriority, KeyValuePair<TKey, TValue>> pair in sortedList.ToArray())
            {
                if (pair.Value.Key.Equals(_key))
                {
                    sortedList.Remove(pair.Key);
                    OnSortedListChanged();
                    return true;
                }
            }

            return false;
        }

        public virtual bool Has(TKey _key)
        {
            foreach (KeyValuePair<TKey, TValue> pair in sortedList.Values)
            {
                if (pair.Key.Equals(_key))
                {
                    return true;
                }
            }

            return false;
        }

        private void OnSortedListChanged()
        {
            TKey cachedLastHighestPriorityKey = lastHighestPriorityKey;
            lastHighestPriorityKey = sortedList.Count > 0 ? sortedList.ElementAt(sortedList.Count - 1).Value.Key : default(TKey);
            if (lastHighestPriorityKey != null && !lastHighestPriorityKey.Equals(cachedLastHighestPriorityKey))
            {
                OnHighestPriorityKeyChanged.Invoke(lastHighestPriorityKey);
            }

            TValue cachedLastHighestPriorityValue = lastHighestPriorityValue;
            lastHighestPriorityValue = sortedList.Count > 0 ? sortedList.ElementAt(sortedList.Count - 1).Value.Value : default(TValue);
            if (lastHighestPriorityValue != null && !lastHighestPriorityValue.Equals(cachedLastHighestPriorityValue))
            {
                OnHighestPriorityValueChanged.Invoke(lastHighestPriorityValue);
            }
        }
    }
} // Minikit namespace

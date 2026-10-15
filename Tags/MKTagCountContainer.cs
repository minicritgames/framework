using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Minikit
{
    /// <summary>
    /// A runtime tag set where each tag is reference counted, so several sources can grant the same tag and it only goes away
    /// once all of them remove it. Queries are hierarchical, like <see cref="MKTagContainer"/>.
    /// </summary>
    public class MKTagCountContainer : IMKTagCollection
    {
        /// <summary> Fired when a tag becomes present, including implicit parents of the added tag (count 0 to 1) </summary>
        public UnityEvent<MKTag> OnTagAdded = new();
        /// <summary> Fired when a tag stops being present, including implicit parents of the removed tag (count 1 to 0) </summary>
        public UnityEvent<MKTag> OnTagRemoved = new();

        private Dictionary<MKTag, int> explicitCounts = new();
        private Dictionary<MKTag, int> implicitCounts = new();


        /// <summary> How many times this tag or any of its children has been granted </summary>
        public int GetCount(MKTag _tag)
        {
            return _tag is null ? 0 : implicitCounts.GetValueOrDefault(_tag);
        }

        public int GetCountExact(MKTag _tag)
        {
            return _tag is null ? 0 : explicitCounts.GetValueOrDefault(_tag);
        }

        public bool HasTag(MKTag _tag)
        {
            return GetCount(_tag) > 0;
        }

        public bool HasTagExact(MKTag _tag)
        {
            return GetCountExact(_tag) > 0;
        }

        public void AddTag(MKTag _tag, int _count = 1)
        {
            if (_tag is null
                || !_tag.isValid
                || _count <= 0)
            {
                return;
            }

            explicitCounts[_tag] = explicitCounts.GetValueOrDefault(_tag) + _count;

            List<MKTag> addedTags = null;
            foreach (MKTag tagOrParent in _tag.SelfAndParents())
            {
                int previousCount = implicitCounts.GetValueOrDefault(tagOrParent);
                implicitCounts[tagOrParent] = previousCount + _count;
                if (previousCount == 0)
                {
                    (addedTags ??= new()).Add(tagOrParent);
                }
            }

            if (addedTags != null)
            {
                foreach (MKTag addedTag in addedTags)
                {
                    OnTagAdded.Invoke(addedTag);
                }
            }
        }

        public void AddTags(IEnumerable<MKTag> _tags)
        {
            if (_tags == null)
            {
                return;
            }

            foreach (MKTag tag in _tags)
            {
                AddTag(tag);
            }
        }

        /// <summary> Removes up to <paramref name="_count"/> grants of exactly this tag. Children of the tag are unaffected. </summary>
        public void RemoveTag(MKTag _tag, int _count = 1)
        {
            if (_tag is null
                || _count <= 0
                || !explicitCounts.TryGetValue(_tag, out int explicitCount))
            {
                return;
            }

            int removedCount = explicitCount < _count ? explicitCount : _count;
            if (explicitCount - removedCount > 0)
            {
                explicitCounts[_tag] = explicitCount - removedCount;
            }
            else
            {
                explicitCounts.Remove(_tag);
            }

            List<MKTag> removedTags = null;
            foreach (MKTag tagOrParent in _tag.SelfAndParents())
            {
                int newCount = implicitCounts.GetValueOrDefault(tagOrParent) - removedCount;
                if (newCount > 0)
                {
                    implicitCounts[tagOrParent] = newCount;
                }
                else
                {
                    implicitCounts.Remove(tagOrParent);
                    (removedTags ??= new()).Add(tagOrParent);
                }
            }

            if (removedTags != null)
            {
                foreach (MKTag removedTag in removedTags)
                {
                    OnTagRemoved.Invoke(removedTag);
                }
            }
        }

        public void RemoveTags(IEnumerable<MKTag> _tags)
        {
            if (_tags == null)
            {
                return;
            }

            foreach (MKTag tag in _tags)
            {
                RemoveTag(tag);
            }
        }

        public IEnumerator<MKTag> GetEnumerator()
        {
            return explicitCounts.Keys.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            return string.Join(", ", explicitCounts.Keys);
        }
    }
} // Minikit namespace

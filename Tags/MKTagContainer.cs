using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Minikit
{
    /// <summary>
    /// A serializable set of tags with hierarchical queries: a container holding "A.B.C" also answers HasTag("A.B") and HasTag("A").
    /// Use <see cref="MKTagCountContainer"/> instead when several sources may grant the same tag at runtime.
    /// </summary>
    [Serializable]
    public class MKTagContainer : IMKTagCollection, ISerializationCallbackReceiver
    {
        public UnityEvent OnTagsChanged => onTagsChanged ??= new();

        public int count => tags.Count;
        public bool isEmpty => tags.Count == 0;

        [SerializeField] private List<MKTag> tags = new();

        [NonSerialized] private UnityEvent onTagsChanged;
        [NonSerialized] private HashSet<MKTag> implicitTags;


        public MKTagContainer()
        {
        }

        public MKTagContainer(MKTag _tag)
        {
            AddTagSilently(_tag);
        }

        public MKTagContainer(IEnumerable<MKTag> _tags)
        {
            if (_tags == null)
            {
                return;
            }

            foreach (MKTag tag in _tags)
            {
                AddTagSilently(tag);
            }
        }


        public IReadOnlyList<MKTag> GetTags()
        {
            return tags;
        }

        public bool HasTag(MKTag _tag)
        {
            return _tag is not null
                && _tag.isValid
                && GetImplicitTags().Contains(_tag);
        }

        public bool HasTagExact(MKTag _tag)
        {
            return _tag is not null
                && _tag.isValid
                && tags.Contains(_tag);
        }

        public bool AddTag(MKTag _tag)
        {
            if (!AddTagSilently(_tag))
            {
                return false;
            }

            onTagsChanged?.Invoke();
            return true;
        }

        /// <summary> Enables collection initializers: <c>new MKTagContainer { tagA, tagB }</c>. Prefer <see cref="AddTag"/> elsewhere. </summary>
        public void Add(MKTag _tag)
        {
            AddTag(_tag);
        }

        public bool AddTags(IEnumerable<MKTag> _tags)
        {
            if (_tags == null)
            {
                return false;
            }

            bool changed = false;
            foreach (MKTag tag in _tags)
            {
                changed |= AddTagSilently(tag);
            }

            if (changed)
            {
                onTagsChanged?.Invoke();
            }

            return changed;
        }

        public bool RemoveTag(MKTag _tag)
        {
            if (!RemoveTagSilently(_tag))
            {
                return false;
            }

            onTagsChanged?.Invoke();
            return true;
        }

        public bool RemoveTags(IEnumerable<MKTag> _tags)
        {
            if (_tags == null)
            {
                return false;
            }

            bool changed = false;
            foreach (MKTag tag in _tags)
            {
                changed |= RemoveTagSilently(tag);
            }

            if (changed)
            {
                onTagsChanged?.Invoke();
            }

            return changed;
        }

        public void Clear()
        {
            if (tags.Count == 0)
            {
                return;
            }

            tags.Clear();
            implicitTags = null;
            onTagsChanged?.Invoke();
        }

        public IEnumerator<MKTag> GetEnumerator()
        {
            return tags.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            return string.Join(", ", tags);
        }

        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            implicitTags = null;
        }

        private bool AddTagSilently(MKTag _tag)
        {
            if (_tag is null
                || !_tag.isValid
                || tags.Contains(_tag))
            {
                return false;
            }

            tags.Add(MKTag.Get(_tag.name));
            implicitTags = null;
            return true;
        }

        private bool RemoveTagSilently(MKTag _tag)
        {
            if (_tag is null
                || !tags.Remove(_tag))
            {
                return false;
            }

            implicitTags = null;
            return true;
        }

        private HashSet<MKTag> GetImplicitTags()
        {
            if (implicitTags == null)
            {
                implicitTags = new();
                foreach (MKTag tag in tags)
                {
                    foreach (MKTag tagOrParent in tag.SelfAndParents())
                    {
                        if (!implicitTags.Add(tagOrParent))
                        {
                            break;
                        }
                    }
                }
            }

            return implicitTags;
        }
    }
} // Minikit namespace

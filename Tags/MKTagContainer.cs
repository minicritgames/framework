using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Minikit
{
    public class MKTagContainer : IEnumerable<MKTag>
    {
        [HideInInspector] public UnityEvent OnTagsChanged = new();
        
        private List<MKTag> tags = new();


        public MKTagContainer()
        {

        }

        public MKTagContainer(MKTag _tag)
        {
            tags.Add(_tag);
        }

        public MKTagContainer(List<MKTag> _tags)
        {
            tags.AddRange(_tags);
        }


        public List<MKTag> GetTags()
        {
            return tags;
        }

        public int NumTags()
        {
            return tags.Count;
        }

        public bool HasTag(MKTag _tag)
        {
            return tags.Contains(_tag);
        }

        public bool HasAnyTags(MKTagContainer _tagContainer)
        {
            foreach (MKTag tag in _tagContainer.GetTags())
            {
                if (HasTag(tag))
                {
                    return true;
                }
            }

            return false;
        }

        public bool HasAllTags(MKTagContainer _tagContainer)
        {
            foreach (MKTag tag in _tagContainer.GetTags())
            {
                if (!HasTag(tag))
                {
                    return false;
                }
            }

            return true;
        }

        public bool IsEmpty()
        {
            return tags.Count == 0;
        }

        public int Count()
        {
            return tags.Count;
        }

        public void AddTag(MKTag _tag)
        {
            if (!tags.Contains(_tag))
            {
                tags.Add(_tag);
                OnTagsChanged.Invoke();
            }
        }

        public void AddTags(MKTagContainer _tagContainer)
        {
            AddTags(_tagContainer.GetTags());
        }

        public void AddTags(List<MKTag> _tags)
        {
            foreach (MKTag tag in _tags)
            {
                AddTag(tag);
            }
            OnTagsChanged.Invoke();
        }

        public void RemoveTag(MKTag _tag)
        {
            if (tags.Contains(_tag))
            {
                tags.Remove(_tag);
                OnTagsChanged.Invoke();
            }
        }

        public void RemoveTags(MKTagContainer _tagContainer)
        {
            RemoveTags(_tagContainer.GetTags());
        }

        public void RemoveTags(List<MKTag> _tags)
        {
            foreach (MKTag tag in _tags)
            {
                RemoveTag(tag);
            }
            OnTagsChanged.Invoke();
        }

        public IEnumerator<MKTag> GetEnumerator()
        {
            return tags.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
} // Minikit namespace 

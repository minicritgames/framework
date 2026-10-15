using System;
using System.Collections.Generic;
using UnityEngine;

namespace Minikit
{
    [Serializable]
    public enum MKTagQueryCondition
    {
        Any, // At least 1 tag in the query is matched by the tags being tested
        All, // All of the tags in the query are matched by the tags being tested
        None // None of the tags in the query are matched by the tags being tested
    }

    /// <summary>
    /// Tests a set of tags against a condition. Matching is hierarchical: a query for "A.B" is matched by a tested tag "A.B.C".
    /// An empty query always passes.
    /// </summary>
    [Serializable]
    public class MKTagQuery
    {
        [SerializeField] private MKTagQueryCondition condition = MKTagQueryCondition.Any;
        [SerializeField] private MKTagContainer tags = new();


        public MKTagQuery(MKTagQueryCondition _condition)
        {
            condition = _condition;
        }

        public MKTagQuery(MKTagQueryCondition _condition, List<MKTag> _tags)
        {
            condition = _condition;
            tags = new(_tags);
        }


        public bool Test(MKTag _tag)
        {
            if (tags.isEmpty)
            {
                return true;
            }

            return condition switch
            {
                MKTagQueryCondition.Any => _tag is not null && _tag.MatchesAny(tags),
                MKTagQueryCondition.All => _tag is not null && MatchesAll(_tag),
                MKTagQueryCondition.None => _tag is null || !_tag.MatchesAny(tags),
                _ => false
            };
        }

        public bool Test(IEnumerable<MKTag> _tags)
        {
            if (tags.isEmpty)
            {
                return true;
            }

            IMKTagCollection collection = _tags as IMKTagCollection ?? new MKTagContainer(_tags);
            return condition switch
            {
                MKTagQueryCondition.Any => collection.HasAny(tags),
                MKTagQueryCondition.All => collection.HasAll(tags),
                MKTagQueryCondition.None => !collection.HasAny(tags),
                _ => false
            };
        }

        private bool MatchesAll(MKTag _tag)
        {
            foreach (MKTag tag in tags)
            {
                if (!_tag.MatchesTag(tag))
                {
                    return false;
                }
            }

            return true;
        }
    }
} // Minikit namespace

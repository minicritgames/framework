using System;
using System.Collections.Generic;
using UnityEngine;

namespace Minikit
{
    /// <summary>
    /// Combines several tag queries under one condition: All passes when every query passes, Any when at least one does, None when none do.
    /// An empty group always passes.
    /// </summary>
    [Serializable]
    public class MKTagQueryGroup
    {
        [SerializeField] private MKTagQueryCondition condition = MKTagQueryCondition.All;
        [SerializeField] private List<MKTagQuery> queries = new();


        public MKTagQueryGroup()
        {
        }

        public MKTagQueryGroup(MKTagQueryCondition _condition, List<MKTagQuery> _queries)
        {
            condition = _condition;
            queries = new(_queries);
        }


        public MKTagQueryCondition GetCondition()
        {
            return condition;
        }

        public IReadOnlyList<MKTagQuery> GetQueries()
        {
            return queries;
        }

        public bool Test(MKTag _tag)
        {
            return Test(query => query.Test(_tag));
        }

        public bool Test(IEnumerable<MKTag> _tags)
        {
            if (queries.Count == 0)
            {
                return true;
            }

            IMKTagCollection collection = _tags as IMKTagCollection ?? new MKTagContainer(_tags);
            return Test(query => query.Test(collection));
        }

        private bool Test(Func<MKTagQuery, bool> _passes)
        {
            if (queries.Count == 0)
            {
                return true;
            }

            return condition switch
            {
                MKTagQueryCondition.Any => queries.Exists(query => _passes(query)),
                MKTagQueryCondition.All => queries.TrueForAll(query => _passes(query)),
                MKTagQueryCondition.None => !queries.Exists(query => _passes(query)),
                _ => false
            };
        }
    }
} // Minikit namespace

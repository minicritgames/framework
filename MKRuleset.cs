using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Minikit
{
    /// <summary> Rulesets are a list of functions that return either true or false. The intention is for other 
    /// objects to add/remove rules to a ruleset, and the object that owns the ruleset determines when to check 
    /// the ruleset. </summary>
    public class MKRuleset<T>
    {
        public delegate bool MKRule(T _generic);
        
        private List<MKRule> rules = new();


        public MKRuleset()
        {
        }

        
        /// <summary>
        /// Checks if all the rules passed
        /// </summary>
        public bool Check(T _generic)
        {
            foreach (MKRule rule in rules)
            {
                if (rule != null)
                {
                    if (!rule.Invoke(_generic))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public bool AddRule(MKRule _rule)
        {
            if (rules.Contains(_rule))
            {
                return false;
            }

            rules.Add(_rule);
            return true;
        }

        public bool RemoveRule(MKRule _rule)
        {
            if (!rules.Contains(_rule))
            {
                return false;
            }

            rules.Remove(_rule);
            return true;
        }
    }
} // Minikit namespace

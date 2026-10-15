using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Minikit
{
    /// <summary>
    /// A hierarchical, dot-separated tag (e.g. "Ability.Weapon.Ranged"). Always obtain tags via <see cref="Get"/>.
    /// A tag matches itself and all of its parents, so "Ability.Weapon.Ranged" matches "Ability.Weapon" and "Ability".
    /// </summary>
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class MKTag : IEquatable<MKTag>
    {
        public const char separator = '.';

        public string name => key ?? "";
        public bool isValid => !string.IsNullOrEmpty(key);
        public MKTag parent => GetInterned().ResolveParent();

        [SerializeField] [JsonProperty] private string key;

        [NonSerialized] private MKTag cachedParent;
        [NonSerialized] private bool parentResolved;

        private static Dictionary<string, MKTag> tags = new();
        public static readonly MKTag Invalid = new("");


        private MKTag(string _key)
        {
            key = _key;
        }


        public static MKTag Get(string _key)
        {
            if (string.IsNullOrEmpty(_key))
            {
                return Invalid;
            }

            if (!tags.TryGetValue(_key, out MKTag tag))
            {
                tag = new MKTag(_key);
                tags.Add(_key, tag);
            }

            return tag;
        }

        public bool MatchesTag(MKTag _other)
        {
            if (!isValid
                || _other is null
                || !_other.isValid)
            {
                return false;
            }

            return IsSameOrChildOf(key, _other.key);
        }

        public bool MatchesTagExact(MKTag _other)
        {
            return isValid
                && _other is not null
                && key == _other.key;
        }

        public bool MatchesAny(IEnumerable<MKTag> _tags)
        {
            if (_tags == null)
            {
                return false;
            }

            foreach (MKTag tag in _tags)
            {
                if (MatchesTag(tag))
                {
                    return true;
                }
            }

            return false;
        }

        public bool MatchesAnyExact(IEnumerable<MKTag> _tags)
        {
            if (_tags == null)
            {
                return false;
            }

            foreach (MKTag tag in _tags)
            {
                if (MatchesTagExact(tag))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary> This tag followed by each of its parents, nearest first </summary>
        public IEnumerable<MKTag> SelfAndParents()
        {
            for (MKTag tag = isValid ? GetInterned() : null; tag != null; tag = tag.parent)
            {
                yield return tag;
            }
        }

        public override string ToString()
        {
            return name;
        }

        public override bool Equals(object _obj)
        {
            return Equals(_obj as MKTag);
        }

        public bool Equals(MKTag _other)
        {
            if (_other is null)
            {
                return false;
            }

            if (ReferenceEquals(this, _other))
            {
                return true;
            }

            return name == _other.name;
        }

        public override int GetHashCode()
        {
            return StringComparer.Ordinal.GetHashCode(name);
        }

        public static bool operator ==(MKTag _lhs, MKTag _rhs)
        {
            if (_lhs is null)
            {
                return _rhs is null;
            }

            return _lhs.Equals(_rhs);
        }

        public static bool operator !=(MKTag _lhs, MKTag _rhs)
        {
            return !(_lhs == _rhs);
        }

        public static bool IsSameOrChildOf(string _key, string _parentKey)
        {
            return _key.StartsWith(_parentKey, StringComparison.Ordinal)
                && (_key.Length == _parentKey.Length
                    || _key[_parentKey.Length] == separator);
        }

        private MKTag GetInterned()
        {
            return isValid ? Get(key) : Invalid;
        }

        private MKTag ResolveParent()
        {
            if (!parentResolved)
            {
                int separatorIndex = name.LastIndexOf(separator);
                cachedParent = separatorIndex > 0 ? Get(key.Substring(0, separatorIndex)) : null;
                parentResolved = true;
            }

            return cachedParent;
        }
    }
} // Minikit namespace

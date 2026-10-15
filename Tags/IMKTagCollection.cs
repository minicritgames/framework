using System.Collections.Generic;

namespace Minikit
{
    /// <summary>
    /// A set of tags that can be tested hierarchically. Enumerating yields only the explicitly added tags, never their implicit parents.
    /// </summary>
    public interface IMKTagCollection : IEnumerable<MKTag>
    {
        /// <summary> True if any contained tag is <paramref name="_tag"/> or a child of it. Containing "A.B.C" means HasTag("A.B") is true. </summary>
        bool HasTag(MKTag _tag);

        /// <summary> True only if <paramref name="_tag"/> itself was added, ignoring hierarchy </summary>
        bool HasTagExact(MKTag _tag);
    }

    public static class MKTagCollectionExtensions
    {
        /// <summary> True if any of <paramref name="_tags"/> is matched hierarchically. False when <paramref name="_tags"/> is empty. </summary>
        public static bool HasAny(this IMKTagCollection _collection, IEnumerable<MKTag> _tags)
        {
            if (_tags == null)
            {
                return false;
            }

            foreach (MKTag tag in _tags)
            {
                if (_collection.HasTag(tag))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool HasAnyExact(this IMKTagCollection _collection, IEnumerable<MKTag> _tags)
        {
            if (_tags == null)
            {
                return false;
            }

            foreach (MKTag tag in _tags)
            {
                if (_collection.HasTagExact(tag))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary> True if every one of <paramref name="_tags"/> is matched hierarchically. True when <paramref name="_tags"/> is empty. </summary>
        public static bool HasAll(this IMKTagCollection _collection, IEnumerable<MKTag> _tags)
        {
            if (_tags == null)
            {
                return true;
            }

            foreach (MKTag tag in _tags)
            {
                if (!_collection.HasTag(tag))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool HasAllExact(this IMKTagCollection _collection, IEnumerable<MKTag> _tags)
        {
            if (_tags == null)
            {
                return true;
            }

            foreach (MKTag tag in _tags)
            {
                if (!_collection.HasTagExact(tag))
                {
                    return false;
                }
            }

            return true;
        }
    }
} // Minikit namespace

using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;

namespace Minikit.Editor
{
    public static class MKTagRegistry
    {
        private static List<string> sortedTags;
        private static HashSet<string> tagSet;


        public static IReadOnlyList<string> GetAllTags()
        {
            EnsureBuilt();
            return sortedTags;
        }

        public static bool IsRegistered(string _key)
        {
            EnsureBuilt();
            return !string.IsNullOrEmpty(_key)
                && tagSet.Contains(_key);
        }

        private static void EnsureBuilt()
        {
            if (tagSet != null)
            {
                return;
            }

            tagSet = new HashSet<string>(StringComparer.Ordinal);
            foreach (FieldInfo field in TypeCache.GetFieldsWithAttribute<MKNativeTagAttribute>())
            {
                if (!field.IsStatic
                    || field.FieldType != typeof(string)
                    || field.GetValue(null) is not string key
                    || string.IsNullOrEmpty(key))
                {
                    continue;
                }

                for (string current = key; !string.IsNullOrEmpty(current); current = GetParentKey(current))
                {
                    if (!tagSet.Add(current))
                    {
                        break;
                    }
                }
            }

            sortedTags = new List<string>(tagSet);
            sortedTags.Sort(CompareHierarchical);
        }

        private static string GetParentKey(string _key)
        {
            int separatorIndex = _key.LastIndexOf(MKTag.separator);
            return separatorIndex > 0 ? _key.Substring(0, separatorIndex) : null;
        }

        private static int CompareHierarchical(string _a, string _b)
        {
            string[] aSegments = _a.Split(MKTag.separator);
            string[] bSegments = _b.Split(MKTag.separator);
            int count = Math.Min(aSegments.Length, bSegments.Length);
            for (int i = 0; i < count; i++)
            {
                int comparison = string.Compare(aSegments[i], bSegments[i], StringComparison.OrdinalIgnoreCase);
                if (comparison != 0)
                {
                    return comparison;
                }
            }

            return aSegments.Length.CompareTo(bSegments.Length);
        }
    }
} // Minikit.Editor namespace

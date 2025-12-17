using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Minikit
{
    public static class MKAssetDatabase
    {
        public static List<T> FindAllScriptableObjects<T>() where T : ScriptableObject
        {
            string[] guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");

            List<T> assets = new();
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                T asset = AssetDatabase.LoadAssetAtPath<T>(path);
                if (asset != null)
                {
                    assets.Add(asset);
                }
            }

            return assets;
        }
    }
} // Minikit namespace

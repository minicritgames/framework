using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Minikit
{
    public static class MKAssetDatabase
    {
        public static List<T> FindAllAssets<T>(string _sourceFolder = "") where T : Object
        {
            string[] guids = FindAllAssetGUIDs<T>(_sourceFolder);

            // Load all the assets
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

        public static string[] FindAllAssetGUIDs<T>(string _sourceFolder = "") where T : Object
        {
            return AssetDatabase.FindAssets($"t:{typeof(T).Name}", new [] { _sourceFolder });
        }
        
        public static List<T> FindAllScriptableObjects<T>() where T : ScriptableObject
        {
            return FindAllAssets<T>($"Assets/{BRProject.projectFolderName}/ScriptableObjects");
        }
    }
} // Minikit namespace

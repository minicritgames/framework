using System.IO;
using UnityEditor;
using UnityEngine;

namespace Minikit.Editor
{
    public static class MKMenu_ScriptableObject
    {
        private const string menuPathDuplicateSO = "Assets/(Minikit) Duplicate ScriptableObject";
        
        
        [MenuItem(menuPathDuplicateSO, true)]
        private static bool ValidateDuplicateScriptableObject()
        {
            if (Selection.objects == null
                || Selection.objects.Length != 1)
            {
                return false;
            }

            return Selection.activeObject is ScriptableObject
                   && AssetDatabase.Contains(Selection.activeObject);
        }
        
        [MenuItem(menuPathDuplicateSO, false, int.MaxValue)]
        private static void DuplicateScriptableObject()
        {
            ScriptableObject sourceSO = Selection.activeObject as ScriptableObject;
            if (sourceSO == null)
            {
                return;
            }

            string sourcePath = AssetDatabase.GetAssetPath(sourceSO);
            string dir = Path.GetDirectoryName(sourcePath);
            string file = Path.GetFileNameWithoutExtension(sourcePath);
            string ext = Path.GetExtension(sourcePath);
            string newPath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(dir!, $"{file}_Copy{ext}"));

            ScriptableObject cloneSO = ScriptableObject.CreateInstance(sourceSO.GetType());
            EditorUtility.CopySerialized(sourceSO, cloneSO);
            AssetDatabase.CreateAsset(cloneSO, newPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorGUIUtility.PingObject(cloneSO);
            Selection.activeObject = cloneSO;
        }
    }
} // Minikit.Framework namespace

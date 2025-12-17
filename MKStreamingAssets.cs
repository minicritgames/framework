using System.IO;
using UnityEngine;

namespace Minikit
{
    public static class MKStreamingAssets
    {
#if UNITY_EDITOR
        public static string GetStreamingAssetsRelativePath(Object _object, string _projectSubfolder = "", string _streamingAssetsSubfolder = "", bool _includeStreamingAssets = true)
        {
            Directory.CreateDirectory(Application.streamingAssetsPath);
            
            string assetPath = UnityEditor.AssetDatabase.GetAssetPath(_object);
            string sourceBase = $"Assets/{BRProject.projectFolderName}/{_projectSubfolder}";
            if (!assetPath.StartsWith(sourceBase))
            {
                Debug.LogError($"Asset not under expected root folder: {sourceBase}\nAsset was under: {assetPath}");
                return null;
            }
            
            string relativePath = assetPath.Substring(sourceBase.Length);  
            string destinationPath = Path.Combine(
                _includeStreamingAssets ? Application.streamingAssetsPath : string.Empty,
                _streamingAssetsSubfolder,
                relativePath
            );

            return destinationPath;
        }
#endif // UNITY_EDITOR
    }
} // Minikit namespace

using System.IO;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace Minikit
{
    public static class MKJson
    {
        public static JsonSerializerSettings settings = new()
        {
            Formatting = Formatting.Indented,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            Converters = MKJsonConverterReflector.converters
        };


        public static void GenerateJSONFromScriptableObject(ScriptableObject _so, string _path, bool _refresh = true)
        {
            // Make sure the directory exists
            Directory.CreateDirectory(Path.GetDirectoryName(_path) ?? string.Empty);
            
            // Convert to JSON and write to a .json file
            string path = Path.ChangeExtension(_path, ".json");
            string json = JsonConvert.SerializeObject(_so, settings);
            File.WriteAllText(path, json);
            
            // Refresh asset database so we can see the new file
            if (_refresh)
            {
                RefreshJSONAssetDatabase();
            }
        }

        public static void RefreshJSONAssetDatabase()
        {
            AssetDatabase.Refresh();
        }
    }
} // Minikit namespace

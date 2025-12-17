using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

namespace Minikit
{
    [MKJsonConverterNonGlobal]
    public class MKJsonConverter_AddressableScriptableObject : MKJsonConverter
    {
        public override void WriteJson(JsonWriter _writer, object _value, JsonSerializer _serializer)
        {
            if (_value is not ScriptableObject so)
            {
                _writer.WriteNull();
                return;
            }

            string assetPath = AssetDatabase.GetAssetPath(so);
            string sourceBase = $"Assets/{BRProject.projectFolderName}/ScriptableObjects/";
            if (!assetPath.StartsWith(sourceBase))
            {
                Debug.LogError($"ScriptableObject not under expected root folder: {sourceBase}\nScriptableObject was under: {assetPath}");
                _writer.WriteNull();
                return;
            }
            
            string relativePath = assetPath.Substring(sourceBase.Length);
            string streamingAssetPath = Path.ChangeExtension(relativePath, ".json");
            
            _writer.WriteValue(streamingAssetPath);
        }

        public override object ReadJson(JsonReader _reader, Type _objectType, object _existingValue, JsonSerializer _serializer)
        {
            string streamingAssetPath = _reader.Value as string;

            if (string.IsNullOrEmpty(streamingAssetPath))
            {
                return null;
            }
            
            return null;
        }

        public override bool CanConvert(Type _objectType)
        {
            return typeof(ScriptableObject).IsAssignableFrom(_objectType);
        }
    }
} // Minikit namespace

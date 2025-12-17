using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace Minikit
{
    [MKJsonConverterNonGlobal]
    public class MKJsonConverter_AddressableScriptableObject : MKJsonConverter
    {
        public override void WriteJson(JsonWriter _writer, object _value, JsonSerializer _serializer)
        {
#if UNITY_EDITOR
            if (_value is not ScriptableObject so)
            {
                _writer.WriteNull();
                return;
            }

            if (MKStreamingAssets.GetStreamingAssetsRelativePath(so, "ScriptableObjects/", "Data/", false) is not string path)
            {
                _writer.WriteNull();
                return;
            }
            path = Path.ChangeExtension(path, ".json");

            _writer.WriteValue(path);
#else
            _writer.WriteNull();
            return;
#endif // UNITY_EDITOR
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

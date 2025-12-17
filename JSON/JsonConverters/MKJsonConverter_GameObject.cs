using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Minikit
{
    public class MKJsonConverter_GameObject : MKJsonConverter
    {
        public override void WriteJson(JsonWriter _writer, object _value, JsonSerializer _serializer)
        {
            if (_value is not GameObject gameObject)
            {
                _writer.WriteNull();
                return;
            }

            _writer.WriteValue(gameObject.name);
        }

        public override object ReadJson(JsonReader _reader, Type _objectType, object _existingValue, JsonSerializer _serializer)
        {
            string address = _reader.Value as string;

            if (string.IsNullOrEmpty(address))
            {
                return null;
            }

            // Load synchronously (invalid in WebGL but fine for desktop/mobile)
            AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(address);
            return handle.WaitForCompletion();
        }

        public override bool CanConvert(Type _objectType)
        {
            return _objectType == typeof(GameObject);
        }
    }
} // Minikit namespace

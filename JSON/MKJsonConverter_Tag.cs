using System;
using Newtonsoft.Json;
using UnityEngine;

namespace Minikit
{
    public class MKJsonConverter_Tag : MKJsonConverter
    {
        public override void WriteJson(JsonWriter _writer, object _value, JsonSerializer _serializer)
        {
            if (_value is not MKTag tag)
            {
                _writer.WriteNull();
                return;
            }

            _writer.WriteValue(tag.key);
        }

        public override object ReadJson(JsonReader _reader, Type _objectType, object _existingValue, JsonSerializer _serializer)
        {
            string key = _reader.Value?.ToString();
                
            return string.IsNullOrEmpty(key) ? null : MKTag.Get(key);
        }

        public override bool CanConvert(Type _objectType)
        {
            return typeof(MKTag).IsAssignableFrom(_objectType);
        }
    }
} // Minikit namespace

using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Minikit
{
    public class MKJsonConverter_Color : MKJsonConverter
    {
        private const string rKey = "r";
        private const string gKey = "g";
        private const string bKey = "b";
        private const string aKey = "a";
            
            
        public override void WriteJson(JsonWriter _writer, object _value, JsonSerializer _serializer)
        {
            Color color = (Color)_value;

            _writer.WriteStartObject();
            _writer.WritePropertyName(rKey);
            _writer.WriteValue(color.r);
            _writer.WritePropertyName(gKey);
            _writer.WriteValue(color.g);
            _writer.WritePropertyName(bKey);
            _writer.WriteValue(color.b);
            _writer.WritePropertyName(aKey);
            _writer.WriteValue(color.a);
            _writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader _reader, Type _objectType, object _existingValue, JsonSerializer _serializer)
        {
            JObject jo = JObject.Load(_reader);

            float r = jo[rKey]?.Value<float>() ?? 0f;
            float g = jo[gKey]?.Value<float>() ?? 0f;
            float b = jo[bKey]?.Value<float>() ?? 0f;
            float a = jo[aKey]?.Value<float>() ?? 0f;

            return new Color(r, g, b, a);
        }

        public override bool CanConvert(Type _objectType)
        {
            return _objectType == typeof(Color);
        }
    }
} // Minikit namespace
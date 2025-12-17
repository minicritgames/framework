using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Minikit
{
    public class MKJsonConverter_Vector2 : MKJsonConverter
    {
        private const string xKey = "x";
        private const string yKey = "y";
            
            
        public override void WriteJson(JsonWriter _writer, object _value, JsonSerializer _serializer)
        {
            Vector2 vector2 = (Vector2)_value;

            _writer.WriteStartObject();
            _writer.WritePropertyName(xKey);
            _writer.WriteValue(vector2.x);
            _writer.WritePropertyName(yKey);
            _writer.WriteValue(vector2.y);
            _writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader _reader, Type _objectType, object _existingValue, JsonSerializer _serializer)
        {
            JObject jo = JObject.Load(_reader);

            float x = jo[xKey]?.Value<float>() ?? 0f;
            float y = jo[yKey]?.Value<float>() ?? 0f;

            return new Vector2(x, y);
        }

        public override bool CanConvert(Type _objectType)
        {
            return _objectType == typeof(Vector2);
        }
    }
} // Minikit namespace

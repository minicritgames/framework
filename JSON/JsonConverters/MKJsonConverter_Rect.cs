using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Minikit
{
    public class MKJsonConverter_Rect : MKJsonConverter
    {
        private const string xKey = "x";
        private const string yKey = "y";
        private const string widthKey = "width";
        private const string heightKey = "height";
            
            
        public override void WriteJson(JsonWriter _writer, object _value, JsonSerializer _serializer)
        {
            Rect rect = (Rect)_value;

            _writer.WriteStartObject();
            _writer.WritePropertyName(xKey);
            _writer.WriteValue(rect.x);
            _writer.WritePropertyName(yKey);
            _writer.WriteValue(rect.y);
            _writer.WritePropertyName(widthKey);
            _writer.WriteValue(rect.width);
            _writer.WritePropertyName(heightKey);
            _writer.WriteValue(rect.height);
            _writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader _reader, Type _objectType, object _existingValue, JsonSerializer _serializer)
        {
            JObject jo = JObject.Load(_reader);

            float x = jo[xKey]?.Value<float>() ?? 0f;
            float y = jo[yKey]?.Value<float>() ?? 0f;
            float width = jo[widthKey]?.Value<float>() ?? 0f;
            float height = jo[heightKey]?.Value<float>() ?? 0f;

            return new Rect(x, y, width, height);
        }

        public override bool CanConvert(Type _objectType)
        {
            return _objectType == typeof(Rect);
        }
    }
} // Minikit namespace

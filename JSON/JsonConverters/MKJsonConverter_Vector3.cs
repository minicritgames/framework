using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Minikit
{
    public class MKJsonConverter_Vector3 : MKJsonConverter
    {
        private const string xKey = "x";
        private const string yKey = "y";
        private const string zKey = "z";
            
            
        public override void WriteJson(JsonWriter _writer, object _value, JsonSerializer _serializer)
        {
            Vector3 vector3 = (Vector3)_value;

            _writer.WriteStartObject();
            _writer.WritePropertyName(xKey);
            _writer.WriteValue(vector3.x);
            _writer.WritePropertyName(yKey);
            _writer.WriteValue(vector3.y);
            _writer.WritePropertyName(zKey);
            _writer.WriteValue(vector3.z);
            _writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader _reader, Type _objectType, object _existingValue, JsonSerializer _serializer)
        {
            JObject jo = JObject.Load(_reader);

            float x = jo[xKey]?.Value<float>() ?? 0f;
            float y = jo[yKey]?.Value<float>() ?? 0f;
            float z = jo[zKey]?.Value<float>() ?? 0f;

            return new Vector3(x, y, z);
        }

        public override bool CanConvert(Type _objectType)
        {
            return _objectType == typeof(Vector3);
        }
    }
} // Minikit namespace

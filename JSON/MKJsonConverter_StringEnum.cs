using System;
using System.Reflection;
using Minikit;
using Newtonsoft.Json;
using UnityEngine;

namespace Minikit
{
    /// <summary> Marks an enum to use its string (name) when serializing </summary>
    [AttributeUsage(AttributeTargets.Enum)]
    public class MKJsonEnumAttribute : Attribute
    {
    }
    
    public class MKJsonConverter_StringEnum : MKJsonConverter
    {
        public override void WriteJson(JsonWriter _writer, object _value, JsonSerializer _serializer)
        {
            if (!_value.GetType().IsEnum)
            {
                _writer.WriteNull();
                return;
            }

            _writer.WriteValue(_value.ToString());
        }

        public override object ReadJson(JsonReader _reader, Type _objectType, object _existingValue, JsonSerializer _serializer)
        {
            string key = _reader.Value?.ToString();

            if (!string.IsNullOrEmpty(key))
            {
                Enum.TryParse(_objectType, key, out object enumValue);
                return enumValue as Enum;
            }

            return null;
        }

        public override bool CanConvert(Type _objectType)
        {
            return _objectType.IsEnum
                   && _objectType.GetCustomAttribute<MKJsonEnumAttribute>() != null;
        }
    }
} // Minikit namespace

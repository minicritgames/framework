using System;
using System.Reflection;
using Minikit;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UnityEngine;

public class MKJsonContractResolver_ScriptableObjectProperties : DefaultContractResolver
{
    public static readonly MKJsonContractResolver_ScriptableObjectProperties instance = new();
    
    private static readonly JsonConverter converter = new MKJsonConverter_AddressableScriptableObject();

    
    protected override JsonProperty CreateProperty(MemberInfo _member, MemberSerialization _memberSerialization)
    {
        JsonProperty property = base.CreateProperty(_member, _memberSerialization);

        Type propertyType = property.PropertyType ?? typeof(void);
        
        // Only apply the converter to properties that are ScriptableObject types
        if (converter.CanConvert(propertyType))
        {
            property.Converter = converter;
        }
        
        // If it's a list
        if (typeof(System.Collections.IEnumerable).IsAssignableFrom(propertyType)
            && propertyType.IsGenericType)
        {
            Type elementType = propertyType.GetGenericArguments()[0];
            if (typeof(ScriptableObject).IsAssignableFrom(elementType))
            {
                // Apply the converter to the list
                property.ItemConverter = converter;
                return property;
            }
        }

        return property;
    }
}

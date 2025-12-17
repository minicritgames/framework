using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace Minikit
{
    public static class MKJson
    {
        public static JsonSerializerSettings settings = new()
        {
            Formatting = Formatting.Indented,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            Converters = MKJsonConverterReflector.converters,
            ContractResolver = MKJsonContractResolver_ScriptableObjectProperties.instance
        };
    }
} // Minikit namespace

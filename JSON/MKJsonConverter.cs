using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Minikit
{
    /// <summary> The base class for all converters that should be bundled together for use via the JsonConverterReflector </summary>
    public abstract class MKJsonConverter : JsonConverter 
    {
    }

    public static class MKJsonConverterReflector
    {
        private static readonly List<MKJsonConverter> converterInstances = new();
        
        // Returns an IList of all MKJsonConverters, usable for de/serialization
        public static IList<JsonConverter> converters => converterInstances.Cast<JsonConverter>().ToList();
        
        
        static MKJsonConverterReflector()
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (type.IsSubclassOf(typeof(MKJsonConverter))
                        && !type.IsAbstract) // Ignore abstract classes since we don't want to register them
                    {
                        converterInstances.Add(Activator.CreateInstance(type) as MKJsonConverter);
                    }
                }
            }
        }
    }
} // Minikit namespace

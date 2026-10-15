using System;
using System.Collections.Generic;
using System.Reflection;

namespace Minikit
{
    public static class MKReflection
    {
        public static List<Assembly> GetAssembliesDependingOn(Assembly _assembly)
        {
            List<Assembly> dependentAssemblies = new();
            string assemblyName = _assembly.GetName().Name;
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly == _assembly)
                {
                    dependentAssemblies.Add(assembly);
                    continue;
                }

                foreach (AssemblyName referencedAssemblyName in assembly.GetReferencedAssemblies())
                {
                    if (referencedAssemblyName.Name == assemblyName)
                    {
                        dependentAssemblies.Add(assembly);
                        break;
                    }
                }
            }

            return dependentAssemblies;
        }

        public static List<Type> GetConcreteSubclassesOf(Type _baseType)
        {
            List<Type> subclasses = new();
#if UNITY_EDITOR
            foreach (Type type in UnityEditor.TypeCache.GetTypesDerivedFrom(_baseType))
            {
                if (!type.IsAbstract)
                {
                    subclasses.Add(type);
                }
            }
#else
            // A subclass can only compile against a direct reference to its base's assembly, so other assemblies can't hold one
            foreach (Assembly assembly in GetAssembliesDependingOn(_baseType.Assembly))
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (!type.IsAbstract
                        && type.IsSubclassOf(_baseType))
                    {
                        subclasses.Add(type);
                    }
                }
            }
#endif

            subclasses.Sort((_a, _b) => string.CompareOrdinal(_a.FullName, _b.FullName));
            return subclasses;
        }

        public static List<Type> GetConcreteSubclassesOf<T>()
        {
            return GetConcreteSubclassesOf(typeof(T));
        }
    }
} // Minikit namespace

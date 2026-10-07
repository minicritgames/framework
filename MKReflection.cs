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
    }
} // Minikit namespace

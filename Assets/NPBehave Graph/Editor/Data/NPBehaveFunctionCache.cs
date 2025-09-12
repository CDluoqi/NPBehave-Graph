using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NPBehave;
using UnityEngine;

namespace UnityEditor.BehaveGraph
{
    [InitializeOnLoad]
    public class NPBehaveFunctionCache
    {
        private static Dictionary<Type, List<FunctionNameAttribute>> m_KnownFunctionLookupTable;
        public static Dictionary<Type, List<FunctionNameAttribute>> KnownFunctionLookupTable => m_KnownFunctionLookupTable;
        static NPBehaveFunctionCache()
        {
            ReCacheKnownFunctions();
        }
        
        private static void ReCacheKnownFunctions()
        {
            m_KnownFunctionLookupTable  = new Dictionary<Type, List<FunctionNameAttribute>>();
            
            IEnumerable<Type> derivedTypes = GetDerivedTypes<NPBehaveGraphRunner>();
            foreach (var type in derivedTypes)
            {
                List<FunctionNameAttribute> functions = new List<FunctionNameAttribute>();
                MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                foreach (MethodInfo method in methods)
                {
                    FunctionNameAttribute attribute = method.GetCustomAttribute<FunctionNameAttribute>();
                    if (attribute != null)
                    {
                        functions.Add(attribute);
                    }
                }
                m_KnownFunctionLookupTable.Add(type, functions);
            }
        }

        private static IEnumerable<Type> GetDerivedTypes<T>()
        {
            return new List<Type>(TypeCache.GetTypesDerivedFrom<T>());
        }
    }
}


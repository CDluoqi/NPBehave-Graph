using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor.BehaveGraph
{
    [InitializeOnLoad]
    internal static class NPBehaveNodeClassCache
    {
        private static Dictionary<Type, List<ContextFilterableAttribute>> m_KnownTypeLookupTable;

        public static IEnumerable<Type> knownNodeTypes
        {
            get => m_KnownTypeLookupTable.Keys;
        }
        
        private static void ReCacheKnownNodeTypes()
        {
            m_KnownTypeLookupTable = new Dictionary<Type, List<ContextFilterableAttribute>>();
            foreach (Type nodeType in TypeCache.GetTypesDerivedFrom<AbstractBehaveNode>())
            {
                if (!nodeType.IsAbstract)
                {
                    List<ContextFilterableAttribute> filterableAttributes = new List<ContextFilterableAttribute>();
                    foreach (Attribute attribute in Attribute.GetCustomAttributes(nodeType))
                    {
                        Type attributeType = attribute.GetType();
                        if (!attributeType.IsAbstract && attribute is ContextFilterableAttribute contextFilterableAttribute)
                        {
                            filterableAttributes.Add(contextFilterableAttribute);
                        }
                    }
                    m_KnownTypeLookupTable.Add(nodeType, filterableAttributes);
                }
            }
        }
        
        public static T GetAttributeOnNodeType<T>(Type nodeType) where T : ContextFilterableAttribute
        {
            var filterableAttributes = GetFilterableAttributesOnNodeType(nodeType);
            foreach (var attr in filterableAttributes)
            {
                if (attr is T searchTypeAttr)
                {
                    return searchTypeAttr;
                }
            }
            return null;
        }
        
        public static IEnumerable<ContextFilterableAttribute> GetFilterableAttributesOnNodeType(Type nodeType)
        {
            if (nodeType == null)
            {
                throw new ArgumentNullException("Cannot get attributes on a null Type");
            }

            if (m_KnownTypeLookupTable.TryGetValue(nodeType, out List<ContextFilterableAttribute> filterableAttributes))
            {
                return filterableAttributes;
            }
            else
            {
                throw new ArgumentException($"The passed in Type {nodeType.FullName} was not found in the loaded assemblies as a child class of AbstractMaterialNode");
            }
        }
        
        static NPBehaveNodeClassCache()
        {
            ReCacheKnownNodeTypes();
        }
    }
    
    
}


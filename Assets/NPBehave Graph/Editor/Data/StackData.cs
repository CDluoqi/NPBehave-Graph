using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.BehaveGraph.Serialization;
using UnityEngine;

namespace UnityEditor.BehaveGraph
{
    [Serializable]
    sealed class StackData
    {
        [SerializeField]
        List<JsonRef<NPBehaveBlockNode>> m_Blocks = new List<JsonRef<NPBehaveBlockNode>>();
        
        public List<JsonRef<NPBehaveBlockNode>> blocks => m_Blocks;
    }
}



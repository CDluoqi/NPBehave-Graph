using System;
using UnityEditor.BehaveGraph.Serialization;
using UnityEngine;
using UnityEngine.Serialization;

namespace UnityEditor.BehaveGraph
{
    class GraphObject : ScriptableObject, ISerializationCallbackReceiver
    {
        private string m_JSONnodeData;
        
        [NonSerialized]
        GraphData m_Graph;
        
        public GraphData graph
        {
            get => m_Graph;
            set
            {
                if (m_Graph != null)
                    m_Graph.owner = null;
                m_Graph = value;
                if (m_Graph != null)
                    m_Graph.owner = this;
            }
        }
        
        public void OnBeforeSerialize()
        {
            if (graph != null)
            {
                m_JSONnodeData = MultiJson.Serialize(graph);
            }
        }

        public void OnAfterDeserialize()
        {
            
        }
        
        GraphData DeserializeGraph()
        {
            var deserializedGraph = new GraphData();
            MultiJson.Deserialize(deserializedGraph, m_JSONnodeData);
            return deserializedGraph;
        }
        
        public void Validate()
        {
            if (graph != null)
            {
                graph.OnEnable();
                graph.ValidateGraph();
            }
        }
        
        void OnEnable()
        {
            if (graph == null && !string.IsNullOrEmpty(m_JSONnodeData))
            {
                graph = DeserializeGraph();
            }
            Validate();
        }
        
        void OnDestroy()
        {
            graph?.OnDisable();
        }
    }
}

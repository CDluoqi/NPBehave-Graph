using System;
using UnityEngine;

namespace UnityEditor.BehaveGraph
{
    class GraphObject : ScriptableObject, ISerializationCallbackReceiver
    {
        [NonSerialized]
        GraphData m_Graph;
        
        public GraphData graph
        {
            get { return m_Graph; }
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
            
        }

        public void OnAfterDeserialize()
        {
            
        }
        
        GraphData DeserializeGraph()
        {
            var deserializedGraph = new GraphData();
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
            if (graph == null)
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

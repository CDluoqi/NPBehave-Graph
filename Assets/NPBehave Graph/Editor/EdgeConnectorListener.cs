using UnityEngine;
using UnityEditor.Experimental.GraphView;

namespace UnityEditor.BehaveGraph
{
    class EdgeConnectorListener : IEdgeConnectorListener
    {
        readonly GraphData m_Graph;
        readonly SearchWindowProvider m_SearchWindowProvider;
        readonly EditorWindow m_editorWindow;

        public EdgeConnectorListener(GraphData graph, SearchWindowProvider searchWindowProvider, EditorWindow editorWindow)
        {
            m_Graph = graph;
            m_SearchWindowProvider = searchWindowProvider;
            m_editorWindow = editorWindow;
        }

        public void OnDropOutsidePort(UnityEditor.Experimental.GraphView.Edge edge, Vector2 position)
        {
            
        }

        public void OnDrop(GraphView graphView, UnityEditor.Experimental.GraphView.Edge edge)
        {
            var leftSlot = edge.output.GetSlot();
            var rightSlot = edge.input.GetSlot();
            if (leftSlot != null && rightSlot != null)
            {
                m_Graph.Connect(leftSlot.slotReference, rightSlot.slotReference);
            }
        }
    }
}

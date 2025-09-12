using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NPBehave;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEditor.BehaveGraph
{
    public class NPBehaveGraphView : GraphView
    {
        List<NPBehaveStackNodeView> stackNodeViews { get; set; }
        
        public NPBehaveGraphView()
        {
            stackNodeViews = new List<NPBehaveStackNodeView>();
            styleSheets.Add(Resources.Load<StyleSheet>("Styles/NPBehaveGraphView"));
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {;
            base.BuildContextualMenu(evt);
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            return ports.ToList().Where(endPort => endPort.direction != startPort.direction && endPort.node != startPort.node).ToList();
        }

        internal void AddStackNodeView(NPBehaveStackNodeView nodeView)
        {
            stackNodeViews.Add(nodeView);
            AddElement(nodeView);
        }
        
        internal NPBehaveStackNodeView GetStackNodeView(StackData stackData)
        {
            return stackNodeViews.FirstOrDefault(s => s.stackData == stackData);
        }
        
    }
}




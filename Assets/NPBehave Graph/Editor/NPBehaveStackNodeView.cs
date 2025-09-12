using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor.BehaveGraph.Drawing.Controls;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Rendering;
using UnityEngine.UIElements;
using UnityEngine;

namespace UnityEditor.BehaveGraph
{
    sealed class NPBehaveStackNodeView : StackNode, IBehaveNodeView
    {
        StackData m_StackData;
        public StackData stackData => m_StackData;
        
        EditorWindow m_EditorWindow;

        private VisualElement m_SlotContainer;
        IEdgeConnectorListener m_ConnectorListener;

        public NPBehaveStackNodeView(NPBehaveStackNode inNode, EditorWindow editorWindow, IEdgeConnectorListener connectorListener, ISearchView searchView)
        {
            name = "stackNodeViewRoot";
            m_StackData = inNode.stackData;
            m_EditorWindow = editorWindow;
            m_ConnectorListener = connectorListener;
            node = inNode;
            
            VisualElement titleContainer = new VisualElement {name = "titleContainer"};

            var titleLabel = new Label
            {
                name = "titleLabel",
                text = inNode.name,
            };
            titleContainer.Add(titleLabel);
            
            headerContainer.Add(titleContainer);
            headerContainer.Add(titleContainer);
            
            m_SlotContainer  = new VisualElement {name = "slotContainer" };
            
            headerContainer.Add(m_SlotContainer);
            var slots = new List<NPBehaveSlot>();
            inNode.GetSlots(slots);
            AddSlots(slots);
            
            var controlsContainer = new  VisualElement { name = "controls" };
            {
                VisualElement controlsDivider = new VisualElement { name = "divider" };
                controlsDivider.AddToClassList("horizontal");
                controlsContainer.Add(controlsDivider);
                VisualElement controlItems = new VisualElement { name = "items" };
                controlsContainer.Add(controlItems);

                foreach (var propertyInfo in node.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                {
                    foreach (IControlAttribute attribute in propertyInfo.GetCustomAttributes(typeof(IControlAttribute),false))
                    {
                        var child = attribute.InstantiateControl(node, propertyInfo, searchView);
                        child.style.backgroundColor = new StyleColor(Color.clear);
                        controlItems.Add(child);
                    }
                }

                if (controlItems.childCount > 0)
                {
                    headerContainer.Add(controlsContainer);
                }

                
            }
            
            SetPosition(new Rect(inNode.drawState.position.x, inNode.drawState.position.y, 0, 0));
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            if (evt.target is NPBehaveNodeView) return;
            InsertCreateNodeAction(evt, childCount, 0);
            evt.menu.InsertSeparator(null, 1);
        }
        
        void InsertCreateNodeAction(ContextualMenuPopulateEvent evt, int separatorIndex, int itemIndex)
        {
            var mousePosition = evt.mousePosition + m_EditorWindow.position.position;
            var graphView = GetFirstAncestorOfType<NPBehaveGraphView>();

            evt.menu.InsertAction(itemIndex, "Add Child", (e) =>
            {
                var context = new NodeCreationContext
                {
                    screenMousePosition = mousePosition,
                    target = this,
                    index = separatorIndex,
                };
                graphView.nodeCreationRequest(context);
            });
        }
        
        void AddSlots(IEnumerable<NPBehaveSlot> slots)
        {
            foreach (var slot in slots)
                AddSlot(slot);
            RefreshPorts();
        }

        Port AddSlot(NPBehaveSlot slot)
        {
            if (slot.hidden)
                return null;
            BehavePort port = BehavePort.Create(slot, m_ConnectorListener);
            
            m_SlotContainer.Add(port);
            
            return port;
        }
        
        public void InsertBlock(NPBehaveNodeView nodeView)
        {
            AddElement(nodeView);
        }

        public Node gvNode => this;
        public AbstractBehaveNode node { get; private set; }
        public VisualElement colorElement => this;

        public void SetColor(Color newColor)
        {
            throw new System.NotImplementedException();
        }

        public void ResetColor()
        {
            throw new System.NotImplementedException();
        }

        public void UpdatePortInputTypes()
        {

        }

        public void UpdateDropdownEntries()
        {
            throw new System.NotImplementedException();
        }

        public void OnModified(ModificationScope scope)
        {
            throw new System.NotImplementedException();
        }

        public void AttachMessage(string errString, ShaderCompilerMessageSeverity severity)
        {
            throw new System.NotImplementedException();
        }

        public void ClearMessage()
        {
            throw new System.NotImplementedException();
        }

        public bool FindPort(SlotReference slotRef, out BehavePort port)
        {
            port = m_SlotContainer.Query<BehavePort>().ToList()
                .First(p => p.slot.slotReference.Equals(slotRef));

            return port != null;
        }
        
        public void Dispose()
        {
            node = null;
            userData = null;
        }
    }
}


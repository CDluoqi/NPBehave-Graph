using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor.BehaveGraph.Drawing.Controls;
using UnityEditor.BehaveGraph.Serialization;
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

        private Action<NPBehaveStackNodeView, bool> m_HoverChanged;

        public NPBehaveStackNodeView(NPBehaveStackNode inNode, EditorWindow editorWindow, IEdgeConnectorListener connectorListener, ISearchView searchView, Action<NPBehaveStackNodeView, bool> hoverChanged)
        {
            name = "stackNodeViewRoot";
            m_StackData = inNode.stackData;
            m_EditorWindow = editorWindow;
            m_ConnectorListener = connectorListener;
            m_HoverChanged = hoverChanged;
            node = inNode;
            
            RegisterCallback((MouseOverEvent _) => m_HoverChanged?.Invoke(this, true));
            RegisterCallback((MouseOutEvent _) =>m_HoverChanged?.Invoke(this, false));
            
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
            InsertCreateEmptyNodeAction(evt, childCount, 1);
            evt.menu.InsertSeparator(null, 2);
        }
        
        public override bool DragEnter(DragEnterEvent evt, IEnumerable<ISelectable> selection, IDropTarget enteredTarget, ISelection dragSource)
        {
            foreach (NPBehaveNodeView nodeView in selection.OfType<NPBehaveNodeView>())
            {
                nodeView.IsDragEnterStackNode = true;
            }
            return base.DragEnter(evt, selection, enteredTarget, dragSource);
        }

        public override bool DragLeave(DragLeaveEvent evt, IEnumerable<ISelectable> selection, IDropTarget leftTarget, ISelection dragSource)
        {
            foreach (NPBehaveNodeView nodeView in selection.OfType<NPBehaveNodeView>())
            {
                nodeView.IsDragEnterStackNode = false;
            }
            return base.DragLeave(evt, selection, leftTarget, dragSource);
        }

        public override bool DragPerform(DragPerformEvent evt, IEnumerable<ISelectable> selection, IDropTarget dropTarget, ISelection dragSource)
        {
            return base.DragPerform(evt, selection, dropTarget, dragSource);
        }

        public void InsertElements(int insertIndex, IEnumerable<GraphElement> elements)
        {
            NPBehaveBlockNode[] blockNodes = elements.Select(x => x.userData as NPBehaveBlockNode).ToArray();

            int count = elements.Count();
            var refs = new JsonRef<NPBehaveBlockNode>[count];
            for (int i = 0; i < count; i++)
            {
                refs[i] = blockNodes[i];
                blockNodes[i].stackData = stackData;
            }
            stackData.blocks.InsertRange(insertIndex, refs);
        }

        public void RemoveElements(IEnumerable<GraphElement> elements)
        {
            NPBehaveNodeView[] blockNodes = elements.OfType<NPBehaveNodeView>().Where(nodeView => nodeView.userData is NPBehaveBlockNode).ToArray();
            for (int i = 0; i < blockNodes.Length; i++)
            {
                if (blockNodes[i].userData is NPBehaveBlockNode blockNode)
                {
                    stackData.blocks.Remove(blockNode);
                    blockNode.stackData = null;
                }
            }
        }

        protected override bool AcceptsElement(GraphElement element, ref int proposedIndex, int maxIndex)
        {
            return element.userData is NPBehaveBlockNode;
        }
        
        protected override void OnSeparatorContextualMenuEvent(ContextualMenuPopulateEvent evt, int separatorIndex)
        {
            base.OnSeparatorContextualMenuEvent(evt, separatorIndex);
            InsertCreateNodeAction(evt, separatorIndex, 0);
            InsertCreateEmptyNodeAction(evt, separatorIndex, 1);
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

        void InsertCreateEmptyNodeAction(ContextualMenuPopulateEvent evt, int separatorIndex, int itemIndex)
        {
            var graphView = GetFirstAncestorOfType<NPBehaveGraphView>();

            evt.menu.InsertAction(itemIndex, "Add Empty Child", (e) =>
            {
                graphView.blockNodeCreationRequest(this, separatorIndex);
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


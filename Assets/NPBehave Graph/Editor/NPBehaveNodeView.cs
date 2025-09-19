using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor.BehaveGraph.Drawing.Controls;
using UnityEditor.BehaveGraph.Serialization;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UIElements;
using Node = UnityEditor.Experimental.GraphView.Node;

namespace UnityEditor.BehaveGraph
{
    sealed class NPBehaveNodeView : Node, IBehaveNodeView
    {
        VisualElement m_TitleContainer;
        VisualElement m_ControlItems;
        VisualElement m_ControlsDivider;
        VisualElement m_DropdownItems;
        IEdgeConnectorListener m_ConnectorListener;
        private ISearchView m_SearchView;
        
        private bool m_IsDragEnterStackNode = false;

        public bool IsDragEnterStackNode
        {
            get => m_IsDragEnterStackNode;
            set => m_IsDragEnterStackNode = value;
        }
        
        public NPBehaveNodeView()
        {
            name = "nodeView";
        }

        public bool AddingToStack()
        {
            NPBehaveStackNodeView stack = GetFirstAncestorOfType<NPBehaveStackNodeView>();
            return stack != null;
        }

        public void Initialize(AbstractBehaveNode inNode, IEdgeConnectorListener connectorListener, ISearchView searchView)
        {
            if (inNode == null)
                return;
            
            title = inNode.name;
            m_ConnectorListener = connectorListener;
            m_SearchView = searchView;
            node = inNode;

            if (inNode is NPRoot)
            {
                capabilities &= ~Capabilities.Deletable; 
            }
            
            styleSheets.Add(Resources.Load<StyleSheet>("Styles/NPBehaveNodeView"));
            var contents = this.Q("contents");
            
            var controlsContainer = new  VisualElement { name = "controls" };
            {
                m_ControlsDivider = new VisualElement { name = "divider" };
                m_ControlsDivider.AddToClassList("horizontal");
                controlsContainer.Add(m_ControlsDivider);
                m_ControlItems = new VisualElement { name = "items" };
                controlsContainer.Add(m_ControlItems);

                foreach (var propertyInfo in node.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                {
                    foreach (IControlAttribute attribute in propertyInfo.GetCustomAttributes(typeof(IControlAttribute),false))
                    {
                        m_ControlItems.Add(attribute.InstantiateControl(node, propertyInfo, m_SearchView));
                    }
                }
            }
            
            if (m_ControlItems.childCount > 0)
                contents.Add(controlsContainer);
            
            var slots = new List<NPBehaveSlot>();
            inNode.GetSlots(slots);
            AddSlots(slots);
            
            m_TitleContainer = this.Q("title");

            if (inNode is NPBehaveBlockNode blockData)
            {
                AddToClassList("blockData");
                m_TitleContainer.RemoveFromHierarchy();
            }
            else
            {
                SetPosition(new Rect(inNode.drawState.position.x, inNode.drawState.position.y, 0, 0));
            }
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
            
            if (slot.isInputSlot)
            {
                inputContainer.Add(port);
            }
            else
            {
                outputContainer.Add(port);
            }
            
            return port;
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
            port = inputContainer.Query<BehavePort>().ToList()
                .Concat(outputContainer.Query<BehavePort>().ToList())
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


using System;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace UnityEditor.BehaveGraph
{
    sealed class BehavePort : Port
    {
        BehavePort(Orientation portOrientation, Direction portDirection, Capacity portCapacity, Type type)
            : base(portOrientation, portDirection, portCapacity, type)
        {
            styleSheets.Add(Resources.Load<StyleSheet>("Styles/BehavePort"));
        }
        
        NPBehaveSlot m_Slot;
        
        public static BehavePort Create(NPBehaveSlot slot, IEdgeConnectorListener connectorListener)
        {
            var port = new BehavePort(Orientation.Horizontal, slot.isInputSlot ? Direction.Input : Direction.Output, Capacity.Single, null)
            {
                m_EdgeConnector = new EdgeConnector<UnityEditor.Experimental.GraphView.Edge>(connectorListener),
            };
            port.AddManipulator(port.m_EdgeConnector);
            port.slot = slot;
            port.portName = slot.displayName;
            port.portColor = NPBehaveSlot.slotColor;
            return port;
        }
        
        public NPBehaveSlot slot
        {
            get { return m_Slot; }
            set
            {
                if (ReferenceEquals(value, m_Slot))
                    return;
                if (value == null)
                    throw new NullReferenceException();
                if (m_Slot != null && value.isInputSlot != m_Slot.isInputSlot)
                    throw new ArgumentException("Cannot change direction of already created port");
                m_Slot = value;
                portName = slot.displayName;
            }
        }

        public Action<Port> OnDisconnect;

        public override void Disconnect(UnityEditor.Experimental.GraphView.Edge edge)
        {
            base.Disconnect(edge);
            OnDisconnect?.Invoke(this);
        }
        
    }
    
    static class BehavePortExtensions
    {
        public static NPBehaveSlot GetSlot(this Port port)
        {
            var behavePort = port as BehavePort;
            if (behavePort == null)
            {
                Debug.LogError("dfasdf");
            }

            if (behavePort.slot == null)
            {
                Debug.LogError("dfasdfasdf43");
            }
            return behavePort != null ? behavePort.slot : null;
        }
    }
}

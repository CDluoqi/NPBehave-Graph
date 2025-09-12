using System;
using UnityEditor.BehaveGraph.Serialization;
using UnityEngine;

namespace UnityEditor.BehaveGraph
{
    [Serializable]
    struct SlotReference : IEquatable<SlotReference>, IComparable<SlotReference>
    {
        [SerializeField]
        JsonRef<AbstractBehaveNode> m_Node;

        [SerializeField]
        int m_SlotId;

        public SlotReference(AbstractBehaveNode node, int slotId)
        {
            m_Node = node;
            m_SlotId = slotId;
        }

        public AbstractBehaveNode node => m_Node;

        // public Guid nodeGuid => m_Node.value.guid;

        public int slotId => m_SlotId;

        public NPBehaveSlot slot => m_Node.value?.FindSlot<NPBehaveSlot>(m_SlotId);

        public bool Equals(SlotReference other) => m_SlotId == other.m_SlotId && m_Node.value == other.m_Node.value;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj.GetType() == GetType() && Equals((SlotReference)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (m_SlotId * 397) ^ m_Node.GetHashCode();
            }
        }

        public int CompareTo(SlotReference other)
        {
            var nodeIdComparison = m_Node.value.objectId.CompareTo(other.m_Node.value.objectId);
            if (nodeIdComparison != 0)
            {
                return nodeIdComparison;
            }

            return m_SlotId.CompareTo(other.m_SlotId);
        }
    }
}
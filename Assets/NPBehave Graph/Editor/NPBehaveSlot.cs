using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.BehaveGraph.Serialization;
using UnityEditor.Graphs;
using UnityEngine;

namespace UnityEditor.BehaveGraph
{
    [Serializable]
    abstract class NPBehaveSlot : JsonObject
    {
        internal static Color slotColor = new Color(0.9647059f, 1.0f, 0.6039216f);
        
        const string k_NotInit = "Not Initialized";
        
        [SerializeField]
        int m_Id;
        
        [SerializeField]
        string m_DisplayName = k_NotInit;
        
        [SerializeField]
        SlotType m_SlotType = SlotType.InputSlot;

        [SerializeField]
        bool m_Hidden;
        
        protected NPBehaveSlot() { }

        protected NPBehaveSlot(int slotId, SlotType slotType, bool hidden = false)
        {
            m_Id = slotId;
            m_DisplayName = displayName;
            m_SlotType = slotType;
            m_DisplayName = "Node";
            m_Hidden = hidden;
        }
        
        protected NPBehaveSlot(int slotId, string displayName, SlotType slotType, bool hidden = false)
        {
            m_Id = slotId;
            m_DisplayName = displayName;
            m_SlotType = slotType;
            m_Hidden = hidden;
        }
        
        public SlotReference slotReference
        {
            get { return new SlotReference(owner, m_Id); }
        }
        
        public AbstractBehaveNode owner { get; set; }
        
        public bool hidden
        {
            get { return m_Hidden; }
            set { m_Hidden = value; }
        }
        
        public int id
        {
            get { return m_Id; }
        }
        
        public bool isInputSlot
        {
            get { return m_SlotType == SlotType.InputSlot; }
        }

        public bool isOutputSlot
        {
            get { return m_SlotType == SlotType.OutputSlot; }
        }

        public SlotType slotType
        {
            get { return m_SlotType; }
        }
        
        public virtual string displayName
        {
            get { return m_DisplayName;}
            set { m_DisplayName = value; }
        }
        
        public abstract bool isDefaultValue { get; }
        
        public abstract void CopyValuesFrom(NPBehaveSlot foundSlot);
        
        public bool Equals(NPBehaveSlot other)
        {
            return m_Id == other.m_Id && owner == other.owner;
        }
        
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((NPBehaveSlot)obj);
        }
    }
}


using System;
using System.Collections;
using System.Collections.Generic;
using NPBehave;
using UnityEditor.BehaveGraph.Serialization;
using UnityEngine;

namespace UnityEditor.BehaveGraph
{
    internal delegate void OnNodeModified(AbstractBehaveNode node, ModificationScope scope);
    
    abstract class AbstractBehaveNode : JsonObject
    {
        [SerializeField]
        private string m_Name;
        
        [SerializeField]
        private DrawState m_DrawState;
        
        [SerializeField]
        List<JsonData<NPBehaveSlot>> m_Slots = new List<JsonData<NPBehaveSlot>>();
        
        OnNodeModified m_OnModified;
        
        public void RegisterCallback(OnNodeModified callback)
        {
            m_OnModified += callback;
        }

        public void UnregisterCallback(OnNodeModified callback)
        {
            m_OnModified -= callback;
        }
        
        public void Dirty(ModificationScope scope)
        {
            if (m_OnModified != null)
                m_OnModified(this, scope);
        }
        
        public string name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }

        public string[] synonyms;
        
        public virtual bool canDeleteNode => true;
        
        public virtual NPBehaveNodeType nodeType => NPBehaveNodeType.Unknown;
        
        public DrawState drawState
        {
            get { return m_DrawState; }
            set
            {
                m_DrawState = value;
            }
        }

        protected AbstractBehaveNode()
        {
            
        }
        
        public virtual void UpdateNodeAfterDeserialization()
        { }
        
        public NPBehaveSlot AddSlot(NPBehaveSlot slot)
        {
            if (slot == null)
            {
                throw new ArgumentException($"Trying to add null slot to node {this}");
            }
            NPBehaveSlot foundSlot = FindSlot<NPBehaveSlot>(slot.id);

            if (slot == foundSlot)
                return foundSlot;
            
            int firstIndex = m_Slots.FindIndex(s => s.value.id == slot.id);
            if (firstIndex >= 0)
            {
                m_Slots[firstIndex] = slot; }
            else
                m_Slots.Add(slot);
            
            slot.owner = this;

            return slot;
        }
        
        public void GetInputSlots<T>(List<T> foundSlots) where T : NPBehaveSlot
        {
            foreach (var slot in m_Slots)
            {
                if (slot.value.isInputSlot && slot is T)
                    foundSlots.Add((T)slot);
            }
        }

        public virtual void GetInputSlots<T>(NPBehaveSlot startingSlot, List<T> foundSlots) where T : NPBehaveSlot
        {
            GetInputSlots(foundSlots);
        }

        public void GetOutputSlots<T>(List<T> foundSlots) where T : NPBehaveSlot
        {
            foreach (var slot in m_Slots)
            {
                if (slot.value.isOutputSlot && slot is T materialSlot)
                {
                    foundSlots.Add(materialSlot);
                }
            }
        }

        public virtual void GetOutputSlots<T>(NPBehaveSlot startingSlot, List<T> foundSlots) where T : NPBehaveSlot
        {
            GetOutputSlots(foundSlots);
        }

        public void GetSlots<T>(List<T> foundSlots) where T : NPBehaveSlot
        {
            foreach (var slot in m_Slots.SelectValue())
            {
                if (slot is T materialSlot)
                {
                    foundSlots.Add(materialSlot);
                }
            }
        }
        
        public T FindSlot<T>(int slotId) where T : NPBehaveSlot
        {
            foreach (var slot in m_Slots.SelectValue())
            {
                if (slot.id == slotId && slot is T)
                    return (T)slot;
            }
            return default(T);
        }

        public T FindInputSlot<T>(int slotId) where T : NPBehaveSlot
        {
            foreach (var slot in m_Slots.SelectValue())
            {
                if (slot.isInputSlot && slot.id == slotId && slot is T)
                    return (T)slot;
            }
            return default(T);
        }

        public T FindOutputSlot<T>(int slotId) where T : NPBehaveSlot
        {
            foreach (var slot in m_Slots.SelectValue())
            {
                if (slot.isOutputSlot && slot.id == slotId && slot is T)
                    return (T)slot;
            }
            return default(T);
        }
        
        protected void EnqueSlotsForSerialization()
        {
            foreach (var slot in m_Slots)
            {
                slot.OnBeforeSerialize();
            }
        }
        
        public SlotReference GetMainSlotReference()
        {
            foreach (var slot in m_Slots.SelectValue())
            {
                return slot.slotReference;
            }
            throw new ArgumentException("Slot could not be found", "slotId");
        }

        public virtual string ParamToJson()
        {
            return null;
        }
    }
}



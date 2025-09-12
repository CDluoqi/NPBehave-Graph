using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor.BehaveGraph
{
    class NPBehaveBlockNode : AbstractBehaveNode
    {
        public NPBehaveBlockNode()
        {
            UpdateNodeAfterDeserialization();
        }
        
        const int SlotId = 0;
        
        public sealed override void UpdateNodeAfterDeserialization()
        {
            AddSlot(new NPBehaveOutputSlot(SlotId));
        }
        
        [NonSerialized]
        StackData m_StackData;
        
        public StackData stackData
        {
            get => m_StackData;
            set => m_StackData = value;
        }
        
        public int index => stackData.blocks.IndexOf(this);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor.BehaveGraph
{
    class NPBehaveStackNode : AbstractBehaveNode
    {
        public NPBehaveStackNode()
        {
            UpdateNodeAfterDeserialization();
            m_StackData = new StackData();
        }
        
        const int SlotId = 0;
        
        public sealed override void UpdateNodeAfterDeserialization()
        {
            AddSlot(new NPBehaveInputSlot(SlotId));
        }
        
        [SerializeField]
        StackData m_StackData;
        
        public StackData stackData
        {
            get => m_StackData;
            set => m_StackData = value;
        }
    }
}

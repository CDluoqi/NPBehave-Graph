using System.Collections;
using System.Collections.Generic;
using UnityEditor.Graphs;
using UnityEngine;

namespace UnityEditor.BehaveGraph
{
    class NPBehaveInputSlot : NPBehaveSlot
    {
        public NPBehaveInputSlot() { }

        public NPBehaveInputSlot(int slotId, bool hidden = false)
            :base(slotId, SlotType.InputSlot, hidden)
        {
            
        }

        public override bool isDefaultValue { get; }
        public override void CopyValuesFrom(NPBehaveSlot foundSlot)
        {
            throw new System.NotImplementedException();
        }
    }
}


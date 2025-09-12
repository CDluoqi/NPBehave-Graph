namespace UnityEditor.BehaveGraph
{
    class NPDecorator : AbstractBehaveNode
    {
        public NPDecorator()
        {
            UpdateNodeAfterDeserialization();
        }
        
        const int SlotInId = 0;
        const int SlotOutId = 1;
        
        public sealed override void UpdateNodeAfterDeserialization()
        {
            AddSlot(new NPBehaveInputSlot(SlotInId));
            AddSlot(new NPBehaveOutputSlot(SlotOutId));
        }
    }
}


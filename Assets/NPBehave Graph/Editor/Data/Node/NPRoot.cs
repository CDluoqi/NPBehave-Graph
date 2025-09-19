using NPBehave;

namespace UnityEditor.BehaveGraph
{
    class NPRoot : AbstractBehaveNode
    {
        public override NPBehaveNodeType nodeType => NPBehaveNodeType.Root;
        public NPRoot()
        {
            name = "Root";
            synonyms = new string[] { "time max" };
            UpdateNodeAfterDeserialization();
        }
        const int SlotId = 0;
        
        public sealed override void UpdateNodeAfterDeserialization()
        {
            AddSlot(new NPBehaveOutputSlot(SlotId));
        }
    }
}
        


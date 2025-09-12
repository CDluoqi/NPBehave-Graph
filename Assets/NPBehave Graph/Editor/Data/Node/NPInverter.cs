using NPBehave;

namespace UnityEditor.BehaveGraph
{
    [Title("Decorator", "Inverter")]
    class NPInverter : NPDecorator
    {
        public override NPBehaveNodeType nodeType => NPBehaveNodeType.Inverter;
        public NPInverter()
        {
            name = "Inverter";
            synonyms = new string[] { "inverter"};
        }
    }
}
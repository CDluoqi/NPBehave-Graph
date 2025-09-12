using NPBehave;

namespace UnityEditor.BehaveGraph
{
    [Title("Decorator", "Succeeder")]
    class NPSucceeder : NPDecorator
    {
        public override NPBehaveNodeType nodeType => NPBehaveNodeType.Succeeder;
        public NPSucceeder()
        {
            name = "Succeeder";
            synonyms = new string[] { "succeeder"};
        }
    }
}

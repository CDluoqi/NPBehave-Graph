using NPBehave;

namespace UnityEditor.BehaveGraph
{
    [Title("Decorator", "Failer")]
    class NPFailer : NPDecorator
    {
        public override NPBehaveNodeType nodeType => NPBehaveNodeType.Failer;
        public NPFailer()
        {
            name = "Failer";
            synonyms = new string[] { "failer"};
        }
    }
}

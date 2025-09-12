using NPBehave;

namespace UnityEditor.BehaveGraph
{
    [Title("Composite", "RandomSelector")]
    class NPRandomSelector : NPComposite
    {
        public override NPBehaveNodeType nodeType => NPBehaveNodeType.RandomSelector;
        public NPRandomSelector()
        {
            name = "RandomSelector";
            synonyms = new string[] { "randomSelector"};
        }
    }
}
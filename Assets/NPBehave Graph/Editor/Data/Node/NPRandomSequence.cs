using NPBehave;

namespace UnityEditor.BehaveGraph
{
    [Title("Composite", "RandomSequence")]
    class NPRandomSequence : NPComposite
    {
        public override NPBehaveNodeType nodeType => NPBehaveNodeType.RandomSequence;
        public NPRandomSequence()
        {
            name = "RandomSequence";
            synonyms = new string[] { "randomSequence"};
        }
    }
}

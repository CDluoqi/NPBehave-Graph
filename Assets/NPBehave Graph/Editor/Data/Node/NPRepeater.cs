using NPBehave;
using UnityEditor.BehaveGraph.Drawing.Controls;
using UnityEngine;

namespace UnityEditor.BehaveGraph
{
    [Title("Decorator", "Repeater")]
    class NPRepeater : NPDecorator
    {
        public override NPBehaveNodeType nodeType => NPBehaveNodeType.Repeater;
        public NPRepeater()
        {
            name = "Repeater";
            synonyms = new string[] { "repeater"};
        }

        [SerializeField]
        private int m_LoopCount = -1;

        [IntegerControl("Count")]
        public int LoopCount
        {
            get => m_LoopCount;

            set
            {
                if (m_LoopCount == value)
                {
                    return;
                }
                m_LoopCount = value;
                Dirty(ModificationScope.Graph);
            }
        }

        public override string ParamToJson()
        {
            NPRepeaterParam param = new NPRepeaterParam()
            {
                loopCount = LoopCount
            };
            return JsonUtility.ToJson(param);
        }
    }
}

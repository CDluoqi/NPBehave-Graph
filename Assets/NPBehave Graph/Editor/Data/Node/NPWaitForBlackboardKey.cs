using NPBehave;
using UnityEditor.BehaveGraph.Drawing.Controls;
using UnityEngine;

namespace UnityEditor.BehaveGraph
{
    [Title("Task", "Wait(black board key)")]
    class NPWaitForBlackBoardKey : NPTask
    {
        public override NPBehaveNodeType nodeType => NPBehaveNodeType.Wait;
        public NPWaitForBlackBoardKey()
        {
            name = "Wait(black board key)";
            synonyms = new string[] { "wait", "black", "board", "key"};
        }

        [SerializeField]
        private string m_BlackboardKey = "";

        [TextControl("Key")]
        public string BlackboardKey
        {
            get => m_BlackboardKey;
            set
            {
                if (m_BlackboardKey == value)
                {
                    return;
                }
                m_BlackboardKey = value;
                Dirty(ModificationScope.Graph);
            }
        }

        [SerializeField]
        private float m_RandomVariance = -1;

        [FloatControl("RandomVar")]
        public float RandomVariance
        {
            get => m_RandomVariance;
            
            set
            {
                m_RandomVariance = value;
                Dirty(ModificationScope.Graph);
            }
        }

        public override string ParamToJson()
        {
            NPWaitParam param = new NPWaitParam
            {
                blackboardKey = BlackboardKey,
                randomVariation = RandomVariance,
                waitNodeType = WaitNodeType.BlackboardKey
            };
            return JsonUtility.ToJson(param);
        }
    }
}
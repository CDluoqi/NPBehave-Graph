using NPBehave;
using UnityEditor.BehaveGraph.Drawing.Controls;
using UnityEngine;

namespace UnityEditor.BehaveGraph
{
    [Title("Task", "Wait(second)")]
    class NPWait : NPTask
    {
        public override NPBehaveNodeType nodeType => NPBehaveNodeType.Wait;
        public NPWait()
        {
            name = "Wait(second)";
            synonyms = new string[] { "wait" };
        }

        [SerializeField]
        private float m_Seconds;

        [FloatControl("Seconds")]
        public float Seconds
        {
            get => m_Seconds;
            set
            {
                m_Seconds = value;
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
                seconds = Seconds,
                randomVariation = RandomVariance,
                waitNodeType = WaitNodeType.Normal
            };
            return JsonUtility.ToJson(param);
        }
    }
}
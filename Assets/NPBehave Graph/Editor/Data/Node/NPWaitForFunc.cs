using NPBehave;
using UnityEditor.BehaveGraph.Drawing.Controls;
using UnityEngine;

namespace UnityEditor.BehaveGraph
{
    [Title("Task", "Wait(function)")]
    class NPWaitForFunc : NPTask
    {
        public override NPBehaveNodeType nodeType => NPBehaveNodeType.Wait;
        public NPWaitForFunc()
        {
            name = "Wait(function)";
            synonyms = new string[] { "wait", "function"};
        }

        [SerializeField]
        private string m_FunctionName = "";

        [FunctionControl("Func", FuncPurpose.Wait)]
        public string FunctionName
        {
            get => m_FunctionName;
            set
            {
                if (m_FunctionName == value)
                {
                    return;
                }
                m_FunctionName = value;
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
                blackboardKey = FunctionName,
                randomVariation = RandomVariance,
                waitNodeType = WaitNodeType.BlackboardKey
            };
            return JsonUtility.ToJson(param);
        }
    }
}
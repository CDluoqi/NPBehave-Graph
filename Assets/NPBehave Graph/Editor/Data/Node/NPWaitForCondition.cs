using NPBehave;
using UnityEditor.BehaveGraph.Drawing.Controls;
using UnityEngine;

namespace UnityEditor.BehaveGraph
{
    [Title("Decorator", "WaitForCondition")]
    class NPWaitForCondition : NPDecorator
    {
        public override NPBehaveNodeType nodeType => NPBehaveNodeType.WaitForCondition;
        public NPWaitForCondition()
        {
            name = "WaitForCondition";
            synonyms = new string[] { "wait", "condition" };
        }
        
        [SerializeField]
        private float m_CheckInterval = -1;

        [FloatControl("CheckInterval")]
        public float CheckInterval
        {
            get => m_CheckInterval;
            set
            {
                m_CheckInterval = value; 
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
        
        [SerializeField]
        private string m_FunctionName = "";
        
        [FunctionControl("Func", FuncPurpose.WaitForCondition)]
        public string FunctionName
        {
            get => m_FunctionName;
            set
            {
                if (m_FunctionName == value)
                    return;

                m_FunctionName = value;
                Dirty(ModificationScope.Graph);
            }
        }

        public override string ParamToJson()
        {
            NPWaitForConditionParam param = new NPWaitForConditionParam
            {
                functionName = FunctionName,
                checkInterval = CheckInterval,
                randomVariation = RandomVariance
            };
            return JsonUtility.ToJson(param);
        }
        
    }
}


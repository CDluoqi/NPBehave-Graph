using NPBehave;
using UnityEditor.BehaveGraph.Drawing.Controls;
using UnityEngine;

namespace UnityEditor.BehaveGraph
{
    [Title("Decorator", "Service")]
    class NPService : NPDecorator
    {
        public override NPBehaveNodeType nodeType => NPBehaveNodeType.Service;
        public NPService()
        {
            name = "Service";
            synonyms = new string[] { "service"};
        }
        
        [SerializeField]
        private string m_FunctionName = "";
        
        [FunctionControl("Func", FuncPurpose.Service)]
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
        
        [SerializeField]
        private float m_Interval = -1;

        [FloatControl("Interval")]
        public float Interval
        {
            get => m_Interval;
            set
            {
                m_Interval = value;
                Dirty(ModificationScope.Graph);
            }
        }
        
        [SerializeField]
        private float m_RandomVariation = -1;
        [FloatControl("RandomVar")]
        public float RandomVariation
        {
            get => m_RandomVariation;
            set
            {
                m_RandomVariation = value;
                Dirty(ModificationScope.Graph);
            }
        }
        
        public override string ParamToJson()
        {
            NPServiceParam param = new NPServiceParam()
            {
                interval = m_Interval,
                randomVariation = m_RandomVariation,
                functionName = m_FunctionName
            };
            return JsonUtility.ToJson(param);
        }
    }
}

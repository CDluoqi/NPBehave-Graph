using NPBehave;
using UnityEditor.BehaveGraph.Drawing.Controls;
using UnityEngine;

namespace UnityEditor.BehaveGraph
{
    [Title("Decorator", "Condition")]
    class NPCondition : NPDecorator
    {
        public override NPBehaveNodeType nodeType => NPBehaveNodeType.Condition;
        public NPCondition()
        {
            name = "Condition";
            synonyms = new string[] { "condition"};
        }
        
        [SerializeField]
        private Stops m_StopsOnChange  = Stops.NONE;
        
        [EnumControl("StopsOnChange")]
        public Stops StopsOnChange
        {
            get => m_StopsOnChange;
            set
            {
                if (m_StopsOnChange == value)
                    return;

                m_StopsOnChange = value;
                Dirty(ModificationScope.Graph);
            }
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
        
        [FunctionControl("Func", FuncPurpose.Condition)]
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
            NPConditionParam param = new NPConditionParam
            {
                functionName = FunctionName,
                stopsOnChange = StopsOnChange,
                checkInterval = CheckInterval,
                randomVariation = RandomVariance
            };
            return JsonUtility.ToJson(param);
        }
    }
}

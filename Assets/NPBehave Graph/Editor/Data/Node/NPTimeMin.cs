using NPBehave;
using UnityEditor.BehaveGraph.Drawing.Controls;
using UnityEngine;

namespace UnityEditor.BehaveGraph
{
    [Title("Decorator", "TimeMin")]
    class NPTimeMin : NPDecorator
    {
        public override NPBehaveNodeType nodeType => NPBehaveNodeType.TimeMin;
        public NPTimeMin()
        {
            name = "TimeMin";
            synonyms = new string[] { "time", "min" };
        }
        
        
        [SerializeField]
        private float m_Limit = 0;

        [FloatControl("Lim")]
        public float Limit
        {
            get => m_Limit;
            set
            {
                m_Limit = value; 
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

        [SerializeField]
        private bool m_WaitOnFailure = false;

        
        ToggleData m_ToggleData = new ToggleData(false);
        
        [ToggleControl("WaitOnFailure")]
        public ToggleData WaitOnFailure
        {
            get
            {
                m_ToggleData.isOn = m_WaitOnFailure;
                return m_ToggleData;
            }
            set
            {
                if (m_WaitOnFailure == value.isOn)
                {
                    return;
                }
                m_WaitOnFailure = value.isOn;
                m_ToggleData.isOn = m_WaitOnFailure;
            }
        }

        public override string ParamToJson()
        {
            NPTimeMinParam param = new NPTimeMinParam
            {
                limit = Limit,
                randomVariation = RandomVariation,
                waitOnFailure = WaitOnFailure.isOn
            };
            return JsonUtility.ToJson(param);
        }
    }
}

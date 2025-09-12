using NPBehave;
using UnityEditor.BehaveGraph.Drawing.Controls;
using UnityEngine;

namespace UnityEditor.BehaveGraph
{
    [Title("Decorator", "TimeMax")]
    class NPTimeMax : NPDecorator
    {
        public override NPBehaveNodeType nodeType => NPBehaveNodeType.TimeMax;
        public NPTimeMax()
        {
            name = "TimeMax";
            synonyms = new string[] { "time","max" };
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
        private bool m_WaitForChildButFailOnLimitReached = false;

        
        ToggleData m_ToggleData = new ToggleData(false);
        
        [ToggleControl("WaitChild")]
        public ToggleData WaitForChildButFailOnLimitReached
        {
            get
            {
                m_ToggleData.isOn = m_WaitForChildButFailOnLimitReached;
                return m_ToggleData;
            }
            set
            {
                if (m_WaitForChildButFailOnLimitReached == value.isOn)
                {
                    return;
                }
                m_WaitForChildButFailOnLimitReached = value.isOn;
                m_ToggleData.isOn = m_WaitForChildButFailOnLimitReached;
            }
        }

        public override string ParamToJson()
        {
            NPTimeMaxParam param = new NPTimeMaxParam
            {
                limit = Limit,
                randomVariation = RandomVariation,
                waitForChildButFailOnLimitReached = WaitForChildButFailOnLimitReached.isOn
            };

            return JsonUtility.ToJson(param);
        }
    }
}

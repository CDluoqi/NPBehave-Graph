using NPBehave;
using UnityEditor.BehaveGraph.Drawing.Controls;
using UnityEngine;

namespace UnityEditor.BehaveGraph
{
    [Title("Decorator", "Cooldown")]
    class NPCooldown : NPDecorator
    {
        public override NPBehaveNodeType nodeType => NPBehaveNodeType.Cooldown;
        public NPCooldown()
        {
            name = "Cooldown";
            synonyms = new string[] { "cooldown"};
        }
        
        [SerializeField]
        private float m_CooldownTime = 1;

        [FloatControl("Time")]
        public float CooldownTime
        {
            get => m_CooldownTime;
            set
            {
                m_CooldownTime = value; 
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
        private bool m_StartAfterDecoratee = false;
        
        ToggleData m_StartAfterDecorateeToggleData = new ToggleData(false);
        
        [ToggleControl("StartAfterDecoratee")]
        public ToggleData StartAfterToggleData
        {
            get
            {
                m_StartAfterDecorateeToggleData.isOn = m_StartAfterDecoratee;
                return m_StartAfterDecorateeToggleData;
            }
            set
            {
                if (m_StartAfterDecoratee == value.isOn)
                {
                    return;
                }
                m_StartAfterDecoratee = value.isOn;
                m_StartAfterDecorateeToggleData.isOn = m_StartAfterDecoratee;
            }
        }
        
        [SerializeField]
        private bool m_ResetOnFailiure = false;
        
        ToggleData m_ResetOnFailiureToggleData = new ToggleData(false);
        
        [ToggleControl("ResetOnFail")]
        public ToggleData ResetOnFailiureToggleData
        {
            get
            {
                m_ResetOnFailiureToggleData.isOn = m_ResetOnFailiure;
                return m_ResetOnFailiureToggleData;
            }
            set
            {
                if (m_ResetOnFailiure == value.isOn)
                {
                    return;
                }
                m_ResetOnFailiure = value.isOn;
                m_ResetOnFailiureToggleData.isOn = m_ResetOnFailiure;
            }
        }
        
        [SerializeField]
        private bool m_FailOnCooldown = false;
        
        ToggleData m_FailOnCooldownToggleData = new ToggleData(false);
        
        [ToggleControl("FailOnCooldown")]
        public ToggleData FailOnCooldownToggleData
        {
            get
            {
                m_FailOnCooldownToggleData.isOn = m_FailOnCooldown;
                return m_FailOnCooldownToggleData;
            }
            set
            {
                if (m_FailOnCooldown == value.isOn)
                {
                    return;
                }
                m_FailOnCooldown = value.isOn;
                m_FailOnCooldownToggleData.isOn = m_FailOnCooldown;
            }
        }

        public override string ParamToJson()
        {
            NPCooldownParam param = new NPCooldownParam()
            {
                cooldownTime = CooldownTime,
                randomVariation = RandomVariance,
                startAfterDecoratee = StartAfterToggleData.isOn,
                resetOnFailiure = ResetOnFailiureToggleData.isOn,
                failOnCooldown = FailOnCooldownToggleData.isOn
            };
            return JsonUtility.ToJson(param);
        }
    }
}

using NPBehave;
using UnityEditor.BehaveGraph.Drawing.Controls;
using UnityEngine;
using UnityEngine.AI;

namespace UnityEditor.BehaveGraph
{
    [Title("Task", "NavWalkTo")]
    class NPNavWalkTo : NPTask
    {
        public override NPBehaveNodeType nodeType => NPBehaveNodeType.NavWalkTo;
        public NPNavWalkTo()
        {
            name = "NavWalkTo";
            synonyms = new string[] { "nav" };
        }

        //[SerializeField]
        //private NavMeshAgent m_Agent;
        
        [SerializeField]
        private string m_BlackboardKey = "";
        
        [TextControl("Key")]
        public string BlackboardKey 
        {
            get { return m_BlackboardKey; }
            set 
            {
                if (m_BlackboardKey == value)
                    return;

                m_BlackboardKey = value;
                Dirty(ModificationScope.Graph);
            }
        }
        
        [SerializeField]
        private float m_Tolerance  = 1;

        [FloatControl("Tolerance")]
        public float Tolerance
        {
            get => m_Tolerance;
            set
            {
                m_Tolerance = value; 
                Dirty(ModificationScope.Graph);
            }
        }
        
        [SerializeField]
        private bool m_StopOnTolerance = false;
        
        ToggleData m_StopOnToleranceToggleData = new ToggleData(false);
        
        [ToggleControl("StopOn")]
        public ToggleData StopOnToleranceToggleData
        {
            get
            {
                m_StopOnToleranceToggleData.isOn = m_StopOnTolerance;
                return m_StopOnToleranceToggleData;
            }
            set
            {
                if (m_StopOnTolerance == value.isOn)
                {
                    return;
                }
                m_StopOnTolerance = value.isOn;
                m_StopOnToleranceToggleData.isOn = m_StopOnTolerance;
            }
        }
        
        [SerializeField]
        private float m_UpdateFrequency  = 0.1f;

        [FloatControl("UpdateFrequency")]
        public float UpdateFrequency
        {
            get => m_UpdateFrequency;
            set
            {
                m_UpdateFrequency = value; 
                Dirty(ModificationScope.Graph);
            }
        }
        
        [SerializeField]
        private float m_UpdateVariance   = 0.025f;

        [FloatControl("UpdateVariance")]
        public float UpdateVariance
        {
            get => m_UpdateVariance;
            set
            {
                m_UpdateVariance = value; 
                Dirty(ModificationScope.Graph);
            }
        }

        public override string ParamToJson()
        {
            NPNavWalkToParam param = new NPNavWalkToParam()
            {
                blackboardKey = BlackboardKey,
                tolerance = Tolerance,
                stopOnTolerance = StopOnToleranceToggleData.isOn,
                updateFrequency = UpdateFrequency,
                updateVariance = UpdateVariance
            };
            return JsonUtility.ToJson(param);
        }
    }
}

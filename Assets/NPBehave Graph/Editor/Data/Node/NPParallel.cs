using System.Collections;
using System.Collections.Generic;
using NPBehave;
using UnityEditor.BehaveGraph.Drawing.Controls;
using UnityEngine;


namespace UnityEditor.BehaveGraph
{
    [Title("Composite", "Parallel")]
    class NPParallel : NPComposite
    {
        public override NPBehaveNodeType nodeType => NPBehaveNodeType.Parallel;
        public NPParallel()
        {
            name = "Parallel";
            synonyms = new string[] { "parallel"};
        }

        [SerializeField]
        private Parallel.Policy m_SuccessPolicy;
        
        [EnumControl("SuccessPolicy")]
        public Parallel.Policy SuccessPolicy
        {
            get => m_SuccessPolicy;
            set
            {
                if (m_SuccessPolicy == value)
                    return;
                m_SuccessPolicy = value;
                Dirty(ModificationScope.Graph);
                
            }
        }
        
        [SerializeField]
        private Parallel.Policy m_FailurePolicy;
        
        [EnumControl("FailurePolicy", 79)]
        public Parallel.Policy FailurePolicy
        {
            get => m_FailurePolicy;
            set
            {
                if (m_FailurePolicy == value)
                    return;
                m_FailurePolicy = value;
                Dirty(ModificationScope.Graph);
            }
        }
        
        public override string ParamToJson()
        {
            NPParallelParam param = new NPParallelParam()
            {
                failurePolicy = m_FailurePolicy,
                successPolicy = m_SuccessPolicy,
            };
            return JsonUtility.ToJson(param);
        }
        
    }
}

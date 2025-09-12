using NPBehave;
using UnityEditor.BehaveGraph.Drawing.Controls;
using UnityEngine;

namespace UnityEditor.BehaveGraph
{
    [Title("Task", "WaitUntilStopped")]
    class NPWaitUntilStopped : NPTask
    {
        public override NPBehaveNodeType nodeType => NPBehaveNodeType.WaitUntilStopped;
        public NPWaitUntilStopped()
        {
            name = "WaitUntilStopped";
            synonyms = new string[] { "wait until stopped" };
        }

        [SerializeField]
        private bool m_SuccessWhenStopped = false;
        
        ToggleData m_ToggleData = new ToggleData(false);
        
        [ToggleControl("SuccessWhenStopped")]
        public ToggleData SuccessWhenStopped
        {
            get
            {
                m_ToggleData.isOn = m_SuccessWhenStopped;
                return m_ToggleData;
            }
            set
            {
                if (m_SuccessWhenStopped == value.isOn)
                {
                    return;
                }
                m_SuccessWhenStopped = value.isOn;
                m_ToggleData.isOn = m_SuccessWhenStopped;
            }
        }

        public override string ParamToJson()
        {
            NPWaitUntilStoppedParam param = new NPWaitUntilStoppedParam()
            {
                successWhenStopped = m_SuccessWhenStopped
            };
            return JsonUtility.ToJson(param);
        }
    }
}

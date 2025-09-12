using NPBehave;
using UnityEditor.BehaveGraph.Drawing.Controls;
using UnityEngine;

namespace UnityEditor.BehaveGraph
{
    [Title("Task", "Action")]
    class NPAction : NPTask
    {
        public override NPBehaveNodeType nodeType => NPBehaveNodeType.Action;
        public NPAction()
        {
            name = "Action";
            synonyms = new string[] { "action" };
        }
        
        [SerializeField]
        private string m_FunctionName = "";
        
        [FunctionControl("Func", FuncPurpose.Action)]
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
            NPActionParam param = new NPActionParam() { functionName = m_FunctionName };
            return JsonUtility.ToJson(param);
        }

    }
}


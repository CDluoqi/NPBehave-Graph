using NPBehave;
using UnityEditor.BehaveGraph.Drawing.Controls;
using UnityEngine;

namespace UnityEditor.BehaveGraph
{
    [Title("Decorator", "BlackboardQuery")]
    class NPBlackboardQuery : NPDecorator
    {
        public override NPBehaveNodeType nodeType => NPBehaveNodeType.BlackboardQuery;
        public NPBlackboardQuery()
        {
            name = "BlackboardQuery";
            synonyms = new string[] { "blackboard", "query" };
        }

        [SerializeField]
        private string[] m_Keys = new []{"key"};
        [StringListControl("Stops")]
        public string[] Keys 
        {
            get => m_Keys;
            set 
            {
                if (m_Keys.Length == value.Length)
                {
                    int count = m_Keys.Length;
                    for (int i = 0; i < m_Keys.Length; i++)
                    {
                        count = count - 1;
                        if (m_Keys[i] != value[i])
                        {
                            break;
                        }
                    }

                    if (count == 0)
                    {
                        return;
                    }
                }

                m_Keys = value;
                Dirty(ModificationScope.Graph);
            }
        }
        
        [SerializeField]
        private Stops m_StopsOnChange = Stops.NONE;
        [EnumControl("Stops")]
        public Stops StopsOnChange 
        {
            get => m_StopsOnChange;
            set 
            {
                if(m_StopsOnChange == value)
                    return;
                m_StopsOnChange = value;
                Dirty(ModificationScope.Graph);
            }
        }
        
        [SerializeField]
        private string m_FunctionName = "";
        
        [FunctionControl("Query", FuncPurpose.BlackboardQuery)]
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
            NPBlackboardQueryParam param = new NPBlackboardQueryParam
            {
                keys = m_Keys,
                stopsOnChange = m_StopsOnChange,
                queryFuncName = m_FunctionName
            };
            return JsonUtility.ToJson(param);
        }
    }
}
using NPBehave;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEngine;
using UnityEditor.BehaveGraph.Drawing.Controls;
namespace UnityEditor.BehaveGraph
{
    [Title("Decorator", "BlackboardCondition")]
    class NPBlackboardCondition : NPDecorator
    {
        public override NPBehaveNodeType nodeType => NPBehaveNodeType.BlackboardCondition;
        public NPBlackboardCondition()
        {
            name = "BlackboardCondition";
            synonyms = new string[] { "decorator", "blackboardCondition" };
        }
        
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
        private Operator m_Operator = Operator.IS_EQUAL;
        
        [EnumControl("Operator")]
        public Operator Operator
        {
            get { return m_Operator; }
            set
            {
                if (m_Operator == value)
                    return;

                m_Operator = value;
                Dirty(ModificationScope.Graph);
            }
        }

        [SerializeField]
        private string m_Value = "";
        [TextControl("Value")]
        public string Value 
        {
            get { return m_Value; }
            set 
            {
                if (m_Value == value)
                    return;

                m_Value = value;
                Dirty(ModificationScope.Graph);
            }
        }

        [SerializeField]
        private Stops m_StopsOnChange = Stops.NONE;
        [EnumControl("Stops")]
        public Stops StopsOnChange 
        {
            get { return m_StopsOnChange; }
            set 
            {
                if(m_StopsOnChange == value)
                    return;
                m_StopsOnChange = value;
                Dirty(ModificationScope.Graph);
            }
        }

        public override string ParamToJson()
        {
            NPBlackboardConditionParam  param = new NPBlackboardConditionParam
            {
                key = BlackboardKey,
                operators = Operator,
                valueString = Value,
                stopsOnChange = StopsOnChange
            };
            return JsonUtility.ToJson(param);
        }
    }
}

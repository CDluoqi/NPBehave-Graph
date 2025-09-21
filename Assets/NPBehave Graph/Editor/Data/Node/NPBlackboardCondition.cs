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
        private BaseType  m_ValueType = BaseType.String;
        
        [SerializeField]
        private string m_StringValue = "";
        [SerializeField]
        private int m_IntValue = 0;
        [SerializeField]
        private bool m_BoolValue = false;
        [SerializeField]
        private float m_FloatValue = 0;

        private MultiTypeValue m_MultiTypeValue = new MultiTypeValue();
        
        [MultiTypeControl("Type")]
        public MultiTypeValue MultiTypeValue 
        {
            get
            {
                m_MultiTypeValue.ValueType = m_ValueType;
                m_MultiTypeValue.StringValue = m_StringValue;
                m_MultiTypeValue.IntValue = m_IntValue;
                m_MultiTypeValue.FloatValue = m_IntValue;
                return m_MultiTypeValue;
            }
            set 
            {
                m_ValueType = value.ValueType;
                m_StringValue = value.StringValue;
                m_IntValue = value.IntValue;
                m_FloatValue = value.FloatValue;
                m_BoolValue = value.BoolValue;
                m_MultiTypeValue = value;
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
                valueString = m_StringValue,
                valueInt = m_IntValue,
                valueBool = m_BoolValue,
                valueFloat = m_FloatValue,
                stopsOnChange = StopsOnChange
            };
            return JsonUtility.ToJson(param);
        }
    }
}

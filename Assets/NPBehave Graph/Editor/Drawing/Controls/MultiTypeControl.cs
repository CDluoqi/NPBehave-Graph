using System;
using System.Linq;
using System.Reflection;
using NPBehave;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Assertions;

namespace UnityEditor.BehaveGraph.Drawing.Controls
{
    struct MultiTypeValue
    {
        public BaseType ValueType;
        public string StringValue;
        public int IntValue;
        public bool BoolValue;
        public float FloatValue;
    }
    
    [AttributeUsage(AttributeTargets.Property)]
    class MultiTypeControl: Attribute, IControlAttribute
    {
        string m_Label;
        public MultiTypeControl(string label = null)
        {
            m_Label = label;
        }

        public VisualElement InstantiateControl(AbstractBehaveNode node, PropertyInfo propertyInfo, ISearchView searchView)
        {
            if (!MultiControlView.validTypes.Contains(propertyInfo.PropertyType))
                return null;
            return new MultiControlView(m_Label, node, propertyInfo);
        }
    }
    
    class MultiControlView : VisualElement
    {
        public static Type[] validTypes = { typeof(MultiTypeValue) };
        AbstractBehaveNode m_Node;
        PropertyInfo m_PropertyInfo;
        MultiTypeValue m_Value;
        
        VisualElement boolField;
        VisualElement intField;
        VisualElement floatField;
        VisualElement stringField;

        VisualElement valueContainer;
        
        VisualElement currentVisualElement;
        
        int m_UndoGroup = -1;
        public MultiControlView(string label, AbstractBehaveNode node, PropertyInfo propertyInfo)
        {
            styleSheets.Add(Resources.Load<StyleSheet>("Styles/NPControls/MultiControlView"));
            m_Node = node;
            m_PropertyInfo = propertyInfo;
            m_Value = GetValue();
            label = label ?? ObjectNames.NicifyVariableName(propertyInfo.Name);
            var typeContainer = new VisualElement(){name = "typeContainer"};
            var thisLabel = new Label(label);
            typeContainer.Add(thisLabel);
            typeContainer.Add(CreateTypeField());
            Add(typeContainer);
            
            valueContainer = new VisualElement(){name = "valueContainer"};
            var valueLabel = new Label("Value");
            valueContainer.Add(valueLabel);
            
            boolField = CreateBoolField();
            intField = CreateIntField();
            floatField = CreateFloatField();
            stringField = CreateStringField();
            
            Add(valueContainer);

            UpdateVisible(m_Value.ValueType);
        }

        void UpdateVisible(BaseType valueType)
        {
            if (currentVisualElement != null)
            {
                valueContainer.Remove(currentVisualElement);
            }

            switch (valueType)
            {
                case BaseType.Bool:
                    currentVisualElement = boolField;
                    valueContainer.Add(boolField);
                    break;
                case BaseType.Float:
                    currentVisualElement = floatField;
                    valueContainer.Add(floatField);
                    break;
                case BaseType.Integer:
                    currentVisualElement = intField;
                    valueContainer.Add(intField);
                    break;
                case BaseType.String:
                    currentVisualElement = stringField;
                    valueContainer.Add(stringField);
                    break;
            }
        }

        EnumField CreateTypeField()
        {
            var enumField = new EnumField(m_Value.ValueType);
            enumField.RegisterValueChangedCallback((evt) =>
            {
                var value = GetValue();
                var newValue = (BaseType)(evt.newValue);
                if (newValue != value.ValueType)
                {
                    value.ValueType = newValue;
                    m_PropertyInfo.SetValue(m_Node, value, null);
                    UpdateVisible(newValue);
                    MarkDirtyRepaint();
                }
            });
            return enumField;
        }

        TextField CreateStringField()
        {
            var field = new TextField { value = m_Value.StringValue };
            field.RegisterCallback<MouseDownEvent>(Repaint);
            field.RegisterCallback<MouseMoveEvent>(Repaint);
            field.RegisterValueChangedCallback(evt =>
            {
                var value = GetValue();
                value.StringValue = evt.newValue;
                m_PropertyInfo.SetValue(m_Node, value, null);
                m_UndoGroup = -1;
            });

            // Pressing escape while we are editing causes it to revert to the original value when we gained focus
            field.Q("unity-text-input").RegisterCallback<KeyDownEvent>(evt =>
            {
                if (evt.keyCode == KeyCode.Escape && m_UndoGroup > -1)
                {
                    Undo.RevertAllDownToGroup(m_UndoGroup);
                    m_UndoGroup = -1;
                    evt.StopPropagation();
                }
                MarkDirtyRepaint();
            });
            field.Q("unity-text-input").RegisterCallback<FocusOutEvent>(evt =>
            {

                MarkDirtyRepaint();
            });
            return field;
        }

        Toggle CreateBoolField()
        {
            Toggle toggle = new Toggle();
            toggle.RegisterValueChangedCallback((evt) =>
            {
                var value = GetValue();
                value.BoolValue = evt.newValue;
                m_PropertyInfo.SetValue(m_Node, value, null);
                MarkDirtyRepaint();
            });
            toggle.value = m_Value.BoolValue;
            return toggle;
        }

        IntegerField CreateIntField()
        {
            var intField = new IntegerField { value = m_Value.IntValue };
            intField.RegisterValueChangedCallback((evt) =>
            {
                var value = GetValue();
                value.IntValue = evt.newValue;
                m_PropertyInfo.SetValue(m_Node, value, null);
                MarkDirtyRepaint();
            });
            return intField;
        }

        FloatField CreateFloatField()
        {
            var floatField = new FloatField() { value = m_Value.IntValue };
            floatField.RegisterValueChangedCallback((evt) =>
            {
                var value = GetValue();
                value.FloatValue = evt.newValue;
                m_PropertyInfo.SetValue(m_Node, value, null);
                MarkDirtyRepaint();
            });
            return floatField;
        }

        MultiTypeValue GetValue()
        {
            var value = m_PropertyInfo.GetValue(m_Node, null);
            Assert.IsNotNull(value);
            return (MultiTypeValue)value;
        }

        void Repaint<T>(MouseEventBase<T> evt) where T : MouseEventBase<T>, new()
        {
            evt.StopPropagation();
            this.MarkDirtyRepaint();
        }
    }
}

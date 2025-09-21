using System;
using System.Reflection;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace UnityEditor.BehaveGraph.Drawing.Controls
{
    [AttributeUsage(AttributeTargets.Property)]
    class EnumControlAttribute : Attribute, IControlAttribute
    {
        string m_Label;
        int m_LabelWidth;

        public EnumControlAttribute(string label = null, int labelWidth = -1)
        {
            m_Label = label;
            m_LabelWidth = labelWidth;
        }

        public VisualElement InstantiateControl(AbstractBehaveNode node, PropertyInfo propertyInfo, ISearchView searchView)
        {
            return new EnumControlView(m_Label, node, propertyInfo, m_LabelWidth);
        }
    }

    class EnumControlView : VisualElement
    {
        AbstractBehaveNode m_Node;
        PropertyInfo m_PropertyInfo;

        public EnumControlView(string label, AbstractBehaveNode node, PropertyInfo propertyInfo, int labelWidth = -1)
        {
            styleSheets.Add(Resources.Load<StyleSheet>("Styles/NPControls/EnumControlView"));
            m_Node = node;
            m_PropertyInfo = propertyInfo;
            if (!propertyInfo.PropertyType.IsEnum)
                throw new ArgumentException("Property must be an enum.", "propertyInfo");
            var labelElement = new Label(label ?? ObjectNames.NicifyVariableName(propertyInfo.Name));
            Add(labelElement);
            if (labelWidth > 0)
            {
                labelElement.style.width = labelWidth;
            }
            var enumField = new EnumField((Enum)m_PropertyInfo.GetValue(m_Node, null));
            enumField.RegisterValueChangedCallback(OnValueChanged);
            Add(enumField);
        }

        void OnValueChanged(ChangeEvent<Enum> evt)
        {
            var value = (Enum)m_PropertyInfo.GetValue(m_Node, null);
            if (!evt.newValue.Equals(value))
            {
                m_PropertyInfo.SetValue(m_Node, evt.newValue, null);
            }
        }
    }
}
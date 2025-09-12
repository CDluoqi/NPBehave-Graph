using System;
using System.Reflection;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace UnityEditor.BehaveGraph.Drawing.Controls
{
    [AttributeUsage(AttributeTargets.Property)]
    class FloatControlAttribute : Attribute, IControlAttribute
    {
        string m_Label;

        public FloatControlAttribute(string label = null)
        {
            m_Label = label;
        }

        public VisualElement InstantiateControl(AbstractBehaveNode node, PropertyInfo propertyInfo, ISearchView searchView = null)
        {
            return new FloatControlView(m_Label, node, propertyInfo);
        }
    }

    class FloatControlView : VisualElement
    {
        AbstractBehaveNode m_Node;
        PropertyInfo m_PropertyInfo;

        public FloatControlView(string label, AbstractBehaveNode node, PropertyInfo propertyInfo)
        {
            styleSheets.Add(Resources.Load<StyleSheet>("Styles/NPControls/FloatControlView"));
            m_Node = node;
            m_PropertyInfo = propertyInfo;
            if (propertyInfo.PropertyType != typeof(float))
                throw new ArgumentException("Property must be of type float.", "propertyInfo");
            label = label ?? ObjectNames.NicifyVariableName(propertyInfo.Name);

            if (!string.IsNullOrEmpty(label))
                Add(new Label(label));

            var floatField = new FloatField() { value = (float)m_PropertyInfo.GetValue(m_Node, null) };
            floatField.RegisterValueChangedCallback(OnChange);

            Add(floatField);
        }

        void OnChange(ChangeEvent<float> evt)
        {
            m_PropertyInfo.SetValue(m_Node, evt.newValue, null);
            this.MarkDirtyRepaint();
        }
    }
}

using System;
using System.Reflection;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace UnityEditor.BehaveGraph.Drawing.Controls
{
    [AttributeUsage(AttributeTargets.Property)]
    class IntegerControlAttribute : Attribute, IControlAttribute
    {
        string m_Label;

        public IntegerControlAttribute(string label = null)
        {
            m_Label = label;
        }

        public VisualElement InstantiateControl(AbstractBehaveNode node, PropertyInfo propertyInfo, ISearchView searchView)
        {
            return new IntegerControlView(m_Label, node, propertyInfo);
        }
    }

    class IntegerControlView : VisualElement
    {
        AbstractBehaveNode m_Node;
        PropertyInfo m_PropertyInfo;

        public IntegerControlView(string label, AbstractBehaveNode node, PropertyInfo propertyInfo)
        {
            styleSheets.Add(Resources.Load<StyleSheet>("Styles/NPControls/IntegerControlView"));
            m_Node = node;
            m_PropertyInfo = propertyInfo;
            if (propertyInfo.PropertyType != typeof(int))
                throw new ArgumentException("Property must be of type integer.", "propertyInfo");
            label = label ?? ObjectNames.NicifyVariableName(propertyInfo.Name);

            if (!string.IsNullOrEmpty(label))
                Add(new Label(label));

            var intField = new IntegerField { value = (int)m_PropertyInfo.GetValue(m_Node, null) };
            intField.RegisterValueChangedCallback(OnChange);

            Add(intField);
        }

        void OnChange(ChangeEvent<int> evt)
        {
            m_PropertyInfo.SetValue(m_Node, evt.newValue, null);
            this.MarkDirtyRepaint();
        }
    }
}
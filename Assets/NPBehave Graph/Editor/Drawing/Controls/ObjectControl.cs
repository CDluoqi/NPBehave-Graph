using System;
using System.Reflection;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace UnityEditor.BehaveGraph.Drawing.Controls 
{
    [AttributeUsage(AttributeTargets.Property)]
    class ObjectControlAttribute : Attribute, IControlAttribute
    {
        string m_Label;

        public ObjectControlAttribute(string label = null) 
        {
            m_Label = label;
        }

        public VisualElement InstantiateControl(AbstractBehaveNode node, PropertyInfo propertyInfo, ISearchView searchView)
        {
            return new ObjectControlView(m_Label, node, propertyInfo);
        }
    }

    class ObjectControlView : VisualElement 
    {
        AbstractBehaveNode m_Node;
        PropertyInfo m_PropertyInfo;

        public ObjectControlView(string label, AbstractBehaveNode node, PropertyInfo propertyInfo) 
        {
            if (!typeof(Object).IsAssignableFrom(propertyInfo.PropertyType))
                throw new ArgumentException("Property must be assignable to UnityEngine.Object.");
            
            name = "controlAttribute";
            m_Node = node;
            m_PropertyInfo = propertyInfo;
            label = label ?? propertyInfo.Name;

            if (!string.IsNullOrEmpty(label))
                Add(new Label { text = label });

            var value = (Object)m_PropertyInfo.GetValue(m_Node, null);
            var objectField = new ObjectField { objectType = propertyInfo.PropertyType, value = value };
            objectField.RegisterValueChangedCallback(OnValueChanged);
            Add(objectField);
        }

        void OnValueChanged(ChangeEvent<Object> evt)
        {
            var value = (Object)m_PropertyInfo.GetValue(m_Node, null);
            if (evt.newValue != value)
            {
                m_PropertyInfo.SetValue(m_Node, evt.newValue, null);
            }
        }
    }
}


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
    
    [AttributeUsage(AttributeTargets.Property)]
    class FunctionControlAttribute : Attribute, IControlAttribute
    {
        string m_Label;
        FuncPurpose m_funcPurpose;
        public FunctionControlAttribute(string label = null, FuncPurpose funcPurpose = FuncPurpose.Any)
        {
            m_Label = label;
            m_funcPurpose  = funcPurpose;
        }

        public VisualElement InstantiateControl(AbstractBehaveNode node, PropertyInfo propertyInfo, ISearchView searchView)
        {
            if (!FunctionControlView.validTypes.Contains(propertyInfo.PropertyType))
                return null;
            return new FunctionControlView(m_Label, node, propertyInfo, searchView, m_funcPurpose);
        }
    }

    class FunctionControlView : VisualElement
    {
        public static Type[] validTypes = { typeof(string) };
        AbstractBehaveNode m_Node;
        PropertyInfo m_PropertyInfo;
        string m_Value;
        int m_UndoGroup = -1;
        TextField field;
        public FunctionControlView(string label, AbstractBehaveNode node, PropertyInfo propertyInfo, ISearchView searchView, FuncPurpose funcPurpose)
        {
            styleSheets.Add(Resources.Load<StyleSheet>("Styles/NPControls/FunctionControlView"));
            m_Node = node;
            m_PropertyInfo = propertyInfo;
            label = label ?? ObjectNames.NicifyVariableName(propertyInfo.Name);
            var thisLabel = new Label(label);
            Add(thisLabel);
            m_Value = GetValue();
            string value = null;
            field = new TextField { value = m_Value };
            field.RegisterCallback<MouseDownEvent>(Repaint);
            field.RegisterCallback<MouseMoveEvent>(Repaint);
            field.RegisterValueChangedCallback(evt =>
            {
                value = GetValue();
                value = evt.newValue;
                m_PropertyInfo.SetValue(m_Node, value, null);
                m_UndoGroup = -1;
            });
            
            field.Q("unity-text-input").RegisterCallback<KeyDownEvent>(evt =>
            {
                if (evt.keyCode == KeyCode.Escape && m_UndoGroup > -1)
                {
                    Undo.RevertAllDownToGroup(m_UndoGroup);
                    m_UndoGroup = -1;
                    evt.StopPropagation();
                }
                this.MarkDirtyRepaint();
            });
            field.Q("unity-text-input").RegisterCallback<FocusOutEvent>(evt =>
            {
                this.MarkDirtyRepaint();
            });
            Add(field);
            
            var findButton = new Button(() =>
            {
                searchView.FindFunction(SetValue, funcPurpose);
            })
            {
                name = "findButton"
            };
            findButton.style.backgroundColor = new StyleColor(new Color(0.2f, 0.2f, 0.2f, 1f));
            findButton.RegisterCallback<PointerEnterEvent>(evt =>
            {
                findButton.style.backgroundColor = new StyleColor(new Color(0.25f, 0.25f, 0.25f, 1f));
            });
            findButton.RegisterCallback<PointerLeaveEvent>(evt =>
            {
                findButton.style.backgroundColor = new StyleColor(new Color(0.2f, 0.2f, 0.2f, 1f));
            });
            
            field.Add(findButton);
        }

        void SetValue(string value)
        {
            field.SetValueWithoutNotify(value);
            m_PropertyInfo.SetValue(m_Node, value, null);
        }

        string GetValue()
        {
            var value = m_PropertyInfo.GetValue(m_Node, null);
            Assert.IsNotNull(value);
            return (string)value;
        }

        void Repaint<T>(MouseEventBase<T> evt) where T : MouseEventBase<T>, new()
        {
            evt.StopPropagation();
            this.MarkDirtyRepaint();
        }
    }
}



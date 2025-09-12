using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Assertions;

namespace UnityEditor.BehaveGraph.Drawing.Controls 
{
    [AttributeUsage(AttributeTargets.Property)]
    class StringListControlAttribute : Attribute, IControlAttribute
    {
        string m_Label;
        public StringListControlAttribute(string label = null)
        {
            m_Label = label;
        }

        public VisualElement InstantiateControl(AbstractBehaveNode node, PropertyInfo propertyInfo, ISearchView searchView)
        {
            if (!StringListControlView.validTypes.Contains(propertyInfo.PropertyType))
                return null;
            return new StringListControlView(m_Label, node, propertyInfo);
        }
    }

    class StringListControlView : VisualElement
    {
        public static Type[] validTypes = { typeof(string[]) };
        AbstractBehaveNode m_Node;
        PropertyInfo m_PropertyInfo;
        List<string> m_ValueList = new List<string>();
        List<int> m_VisualElementList = new List<int>();
        int m_UndoGroup = -1;

        private VisualElement m_ListContainer;
        public StringListControlView(string label, AbstractBehaveNode node, PropertyInfo propertyInfo)
        {
            styleSheets.Add(Resources.Load<StyleSheet>("Styles/NPControls/StringListControlView"));
            m_Node = node;
            m_PropertyInfo = propertyInfo;
            label = label ?? ObjectNames.NicifyVariableName(propertyInfo.Name);
            var thisLabel = new Label(label);
            Add(thisLabel);
            m_ListContainer = new VisualElement(){name = "ListContainer"};
            Add(m_ListContainer);
            
            m_ValueList.AddRange(GetValue());
            foreach (string value in m_ValueList)
            {
                CreateField(value);
            }
            
            Button addBtn = new Button(() =>
            {
                m_ValueList.Add("key");
                CreateField("key");
                m_PropertyInfo.SetValue(m_Node, m_ValueList.ToArray(), null);
            });
            
            addBtn.name = "addButton";
            addBtn.text = "+";
            Add(addBtn);
        }

        private void CreateField(string value)
        {
            TextField field = new TextField { value = value };
            int hashCode = field.GetHashCode();
            m_VisualElementList.Add(hashCode);
            field.RegisterCallback<MouseDownEvent>(Repaint);
            field.RegisterCallback<MouseMoveEvent>(Repaint);
            field.RegisterValueChangedCallback(evt =>
            {
                int index = m_VisualElementList.IndexOf(hashCode);
                m_ValueList[index] = evt.newValue;
                m_PropertyInfo.SetValue(m_Node, m_ValueList.ToArray(), null);
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
            m_ListContainer.Add(field);
            
            var deleteButton = new Button(() =>
            {
                int index = m_VisualElementList.IndexOf(hashCode);
                m_VisualElementList.RemoveAt(index);
                m_ValueList.RemoveAt(index);
                m_PropertyInfo.SetValue(m_Node, m_ValueList.ToArray(), null);
                m_ListContainer.Remove(field);
            })
            {
                name = "deleteButton"
            };
            
            field.Add(deleteButton);
        }



        string[] GetValue()
        {
            var value = m_PropertyInfo.GetValue(m_Node, null);
            Assert.IsNotNull(value);
            return (string[])value;
        }

        void Repaint<T>(MouseEventBase<T> evt) where T : MouseEventBase<T>, new()
        {
            evt.StopPropagation();
            this.MarkDirtyRepaint();
        }
    }
}

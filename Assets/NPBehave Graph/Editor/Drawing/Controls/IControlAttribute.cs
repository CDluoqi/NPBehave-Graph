using System.Reflection;

using UnityEngine.UIElements;

namespace UnityEditor.BehaveGraph.Drawing.Controls
{
    interface IControlAttribute
    {
        VisualElement InstantiateControl(AbstractBehaveNode node, PropertyInfo propertyInfo, ISearchView searchView = null);
    }
}
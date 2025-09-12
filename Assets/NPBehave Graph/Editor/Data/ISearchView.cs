using System;
using NPBehave;

namespace UnityEditor.BehaveGraph
{
    public interface ISearchView
    {
        void FindFunction(Action<string> selectedAction, FuncPurpose purpose = FuncPurpose.Any);
    }
}


using System;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEditor.BehaveGraph
{
    interface IBehaveNodeView : IDisposable
    {
        Node gvNode { get; }
        AbstractBehaveNode node { get; }
        VisualElement colorElement { get; }
        void SetColor(Color newColor);
        void ResetColor();
        void UpdatePortInputTypes();
        void UpdateDropdownEntries();
        void OnModified(ModificationScope scope);
        void AttachMessage(string errString, ShaderCompilerMessageSeverity severity);
        void ClearMessage();
        bool FindPort(SlotReference slot, out BehavePort port);
    }
}

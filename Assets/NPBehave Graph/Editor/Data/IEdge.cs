using System;

namespace UnityEditor.BehaveGraph
{
    interface IEdge : IEquatable<IEdge>
    {
        SlotReference outputSlot { get; }
        SlotReference inputSlot { get; }
    }
}
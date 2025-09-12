using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NPBehave
{
    public enum FuncPurpose
    {
        Any,
        Action,
        Wait,
        BlackboardQuery,
        Condition,
        ObserverOnStart,
        ObserverOnStop,
        Service,
        WaitForCondition
    }
    
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class FunctionNameAttribute : Attribute
    {
        public string Name { get; }
        public FuncPurpose  Purpose { get; }

        public string Help { get; }

        public FunctionNameAttribute(string name, FuncPurpose purpose = FuncPurpose.Any, string help = "")
        {
            Name = name;
            Purpose = purpose;
            Help = help;
        }
    }
}



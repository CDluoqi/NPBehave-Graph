using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace NPBehave
{
    public static class NPBehaveFunctionType
    {
        public  static bool IsActionFunc(string functionType)
        {
            Debug.LogError("IsActionFunc " + functionType);
            if (functionType == "System.Func`1[System.Boolean]") return true;
            if(functionType == "System.Func`2[System.Boolean,NPBehave.Action+Result]") return true;
            if(functionType == "System.Func`2[NPBehave.Action+Request,NPBehave.Action+Result]") return true;
            return false; 
        }
        
        public static Type GetFuncType(MethodInfo method)
        {
            Type[] parameterTypes = method.GetParameters().Select(p => p.ParameterType).ToArray();
            Type returnType = method.ReturnType;
            
            Type[] genericArguments = parameterTypes.Concat(new[] { returnType }).ToArray();
            Type genericFuncType = null;

            switch (parameterTypes.Length) 
            {
                case 0: genericFuncType = typeof(Func<>);break; 
                case 1: genericFuncType = typeof(Func<,>);break; 
                case 2: genericFuncType = typeof(Func<,,>);break;
                case 3: genericFuncType = typeof(Func<,,,>);break;
                case 4: genericFuncType = typeof(Func<,,,,>);break;
                case 5: genericFuncType = typeof(Func<,,,,,>);break;
                case 6: genericFuncType = typeof(Func<,,,,,,>);break; 
                case 7: genericFuncType = typeof(Func<,,,,,,,>);break; 
                case 8: genericFuncType = typeof(Func<,,,,,,,,>);break; 
                case 9: genericFuncType = typeof(Func<,,,,,,,,,>);break;
                case 10: genericFuncType = typeof(Func<,,,,,,,,,,>);break;
                case 11: genericFuncType = typeof(Func<,,,,,,,,,,,>);break;
                case 12: genericFuncType = typeof(Func<,,,,,,,,,,,,>);break;
                case 13: genericFuncType = typeof(Func<,,,,,,,,,,,,,>);break;
                case 14: genericFuncType = typeof(Func<,,,,,,,,,,,,,,>);break;
                case 15: genericFuncType = typeof(Func<,,,,,,,,,,,,,,,>);break;
                case 16: genericFuncType = typeof(Func<,,,,,,,,,,,,,,,,>);break;
                default:throw new NotSupportedException($"Func with {parameterTypes.Length} parameters is not supported.");
            }
            Type funcType = genericFuncType.MakeGenericType(genericArguments);
            return funcType;
        }

        public static Type GetActionType(MethodInfo method)
        {
            Type[] parameterTypes = method.GetParameters().Select(p => p.ParameterType).ToArray();
            
            Type genericFuncType = null;

            switch (parameterTypes.Length) 
            {
                case 0: return typeof(System.Action);
                case 1: genericFuncType = typeof(Action<>);break; 
                case 2: genericFuncType = typeof(Action<,,>);break;
                case 3: genericFuncType = typeof(Action<,,,>);break;
                case 4: genericFuncType = typeof(Action<,,,,>);break;
                case 5: genericFuncType = typeof(Action<,,,,,>);break;
                case 6: genericFuncType = typeof(Action<,,,,,,>);break; 
                case 7: genericFuncType = typeof(Action<,,,,,,,>);break; 
                case 8: genericFuncType = typeof(Action<,,,,,,,,>);break; 
                case 9: genericFuncType = typeof(Action<,,,,,,,,,>);break;
                case 10: genericFuncType = typeof(Action<,,,,,,,,,,>);break;
                case 11: genericFuncType = typeof(Action<,,,,,,,,,,,>);break;
                case 12: genericFuncType = typeof(Action<,,,,,,,,,,,,>);break;
                case 13: genericFuncType = typeof(Action<,,,,,,,,,,,,,>);break;
                case 14: genericFuncType = typeof(Action<,,,,,,,,,,,,,,>);break;
                case 15: genericFuncType = typeof(Action<,,,,,,,,,,,,,,,>);break;
                default:throw new NotSupportedException($"Func with {parameterTypes.Length} parameters is not supported.");
            }
            Type funcType = genericFuncType.MakeGenericType(parameterTypes);
            return funcType;
        }
    }
}


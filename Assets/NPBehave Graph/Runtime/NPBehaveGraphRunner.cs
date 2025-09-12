using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NPBehave;
using UnityEngine;

namespace NPBehave
{
    public class NPBehaveGraphRunner : MonoBehaviour
    {
        [SerializeField] 
        private NPBehaveTreeAsset behaveTree;
        
        Dictionary<string, List<object>> actionMap;

        void Start()
        {
            InitActionMap();
            
            Root behaviorTree = CreateBehaveTree();
            behaviorTree.Start();
            Debugger debugger = gameObject.AddComponent<Debugger>();
            debugger.BehaviorTree = behaviorTree;
        }

        protected virtual void InitActionMap()
        {
            actionMap = new Dictionary<string, List<object>>();
            
            MethodInfo[] methods = GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            foreach (MethodInfo method in methods)
            {
                FunctionNameAttribute attribute = method.GetCustomAttribute<FunctionNameAttribute>();
                if (attribute != null)
                {
                    Type funcType = method.ReturnType != typeof(void) ? NPBehaveFunctionType.GetFuncType(method) : NPBehaveFunctionType.GetActionType(method);
                    Delegate functionDelegate = method.CreateDelegate(funcType, this);
                    if (!actionMap.TryGetValue(attribute.Name, out var methodList))
                    {
                        methodList = new List<object>();
                        actionMap.Add(attribute.Name, methodList);
                    }
                    methodList.Add(functionDelegate);
                }
            }

        }

        Root CreateBehaveTree()
        {
            NodeConfig nodeConfig = JsonUtility.FromJson<NodeConfig>(behaveTree.Code);
            Root root = new Root(new Action(() => { }));
            return root;
        }

        Node CreateNode(NodeConfig nodeConfig)
        {
            switch (nodeConfig.nodeType)
            {
                case NPBehaveNodeType.Selector:return CreateSelector(nodeConfig);
                case NPBehaveNodeType.Sequence:return CreateSequence(nodeConfig);
                case NPBehaveNodeType.Parallel:return CreateParallel(nodeConfig);
                case NPBehaveNodeType.RandomSelector:return CreateRandomSelector(nodeConfig);
                case NPBehaveNodeType.RandomSequence:return CreateRandomSequence(nodeConfig);
                case NPBehaveNodeType.Action:return CreateActionNode(nodeConfig);
                case NPBehaveNodeType.NavWalkTo:return CreateNavWalkTo(nodeConfig);
                case NPBehaveNodeType.Wait:return CreateWaitNode(nodeConfig);
                case NPBehaveNodeType.WaitUntilStopped:return CreateWaitUntilStoppedNode(nodeConfig);
                case NPBehaveNodeType.BlackboardCondition:return CreateBlackboardCondition(nodeConfig);
                case NPBehaveNodeType.BlackboardQuery:return CreateBlackboardQuery(nodeConfig);;
                case NPBehaveNodeType.Condition:return CreateCondition(nodeConfig);
                case NPBehaveNodeType.Cooldown:return CreateCooldown(nodeConfig);
                case NPBehaveNodeType.Failer: return CreateFailerNode(nodeConfig);
                case NPBehaveNodeType.Inverter:return CreateInverterNode(nodeConfig);
                case NPBehaveNodeType.Observer:return CreateObserverNode(nodeConfig);
                case NPBehaveNodeType.Random:return CreateRandomNode(nodeConfig);
                case NPBehaveNodeType.Repeater:return CreateRepeaterNode(nodeConfig);
                case NPBehaveNodeType.Service:return CreateServiceNode(nodeConfig);
                case NPBehaveNodeType.Succeeder:return CreateSucceeder(nodeConfig);
                case NPBehaveNodeType.TimeMax:return CreateTimeMax(nodeConfig);
                case NPBehaveNodeType.TimeMin:return CreateTimeMin(nodeConfig);
                case NPBehaveNodeType.WaitForCondition:return CreateWaitForCondition(nodeConfig);
            }
            Debug.LogError("Unknown node type " +  nodeConfig.nodeType);
            return null;
        }

        List<object> GetFunctionListByName(string functionName)
        {
            return actionMap.GetValueOrDefault(functionName);
        }

        T GetFunction<T>(List<object> functionList)
        {
            if (functionList != null)
            {
                foreach (var function in functionList)
                {
                    if (function is T typeFunc)
                    {
                        return typeFunc;
                    }
                }
            }
            return default(T);
        }

        Node[] CreateChildren(NodeConfig nodeConfig)
        {
            List<Node> childNodes = new List<Node>();
            foreach (var child in nodeConfig.nodes)
            {
                var childNode = CreateNode(child);
                if (childNode != null)
                {
                    childNodes.Add(childNode);
                }
            }

            return childNodes.Count == 0 ? null : childNodes.ToArray();
        }

        Selector CreateSelector(NodeConfig nodeConfig)
        {
            Node[] childrenList = CreateChildren(nodeConfig);
            if (childrenList == null)
            {
                Debug.LogError("Cannot create selector with out children");
                return null;
            }
            return new Selector(childrenList);
        }
        
        Sequence CreateSequence(NodeConfig nodeConfig)
        {
            Node[] childrenList = CreateChildren(nodeConfig);
            if (childrenList == null)
            {
                Debug.LogError("Cannot create sequence with out children");
                return null;
            }
            return new Sequence(childrenList);
        }

        Parallel CreateParallel(NodeConfig nodeConfig)
        {
            if (!TryParseParam(nodeConfig.param, out NPParallelParam param))
            {
                Debug.LogError("Parallel can't parse param");
                return null;
            }
            Node[] childrenList = CreateChildren(nodeConfig);
            if (childrenList == null)
            {
                Debug.LogError("Cannot create sequence with out children");
                return null;
            }
            return new Parallel(param.successPolicy, param.failurePolicy, CreateChildren(nodeConfig));
        }
        
        RandomSelector CreateRandomSelector(NodeConfig nodeConfig)
        {
            Node[] childrenList = CreateChildren(nodeConfig);
            if (childrenList == null)
            {
                Debug.LogError("Cannot create random selector with out children");
                return null;
            }
            return new RandomSelector(childrenList);
        }
        
        RandomSequence CreateRandomSequence(NodeConfig nodeConfig)
        {
            Node[] childrenList = CreateChildren(nodeConfig);
            if (childrenList == null)
            {
                Debug.LogError("Cannot create random sequence with out children");
                return null;
            }
            return new RandomSequence(childrenList);
        }

        Action CreateActionNode(NodeConfig nodeConfig)
        {
            if (!TryParseParam(nodeConfig.param, out NPActionParam param))
            {
                Debug.LogError("Action can't parse param");
                return null;
            }
            
            List<object> functionList = GetFunctionListByName(param.functionName);
            System.Action actionFunc = GetFunction<System.Action>(functionList);
            if (actionFunc != null)
            {
                Action action = new Action(actionFunc);
                return action;
            }
            
            Func<bool> singleFrameFunc = GetFunction<Func<bool>>(functionList);
            if (singleFrameFunc != null)
            {
                Action action = new Action(singleFrameFunc);
                return action;
            }
            
            Func<bool, Action.Result> multiframeFunc = GetFunction<Func<bool, Action.Result>>(functionList);
            if (multiframeFunc != null)
            {
                Action action = new Action(multiframeFunc);
                return action;
            }
            
            Func<Action.Request, Action.Result> multiframeFunc2 = GetFunction<Func<Action.Request, Action.Result>>(functionList);
            if (multiframeFunc2 != null)
            {
                Action action = new Action(multiframeFunc2);
                return action;
            }
            
            Debug.LogError("Action func not found! param:" + nodeConfig.param);
            return new Action(() => { });
        }

        NavMoveTo CreateNavWalkTo(NodeConfig nodeConfig)
        {
            if (!TryParseParam(nodeConfig.param, out NPNavWalkToParam param))
            {
                Debug.LogError("NavMoveTo can't parse param");
                return null;
            }

            return new NavMoveTo(null, param.blackboardKey, param.tolerance, param.stopOnTolerance,
                param.updateFrequency, param.updateVariance);
        }

        Wait CreateWaitNode(NodeConfig nodeConfig)
        {
            if (!TryParseParam(nodeConfig.param, out NPWaitParam param))
            {
                Debug.LogError("Wait can't parse param");
                return null;
            }

            switch (param.waitNodeType)
            {
                case WaitNodeType.Normal:
                    return param.randomVariation < 0 ? new Wait(param.seconds) : new Wait(param.seconds, param.randomVariation);
                case WaitNodeType.BlackboardKey:
                    return param.randomVariation < 0 ? new Wait(param.blackboardKey) : new Wait(param.blackboardKey, param.randomVariation);
                case WaitNodeType.Func:
                    List<object> functionList = GetFunctionListByName(param.functionName);
                    Func<float> func = GetFunction<Func<float>>(functionList);
                    if (func == null)
                    {
                        Debug.LogWarning("Wait func not found! param:" + nodeConfig.param);
                    }
                    return param.randomVariation < 0 ? new Wait(func, param.randomVariation) : new Wait(func);
            }
            return null;
        }

        WaitUntilStopped CreateWaitUntilStoppedNode(NodeConfig nodeConfig)
        {
            if (!TryParseParam(nodeConfig.param, out NPWaitUntilStoppedParam param))
            {
                Debug.LogError("Wait can't parse param");
                return null;
            }
            return new WaitUntilStopped(param.successWhenStopped);
        }
        
        BlackboardCondition  CreateBlackboardCondition(NodeConfig nodeConfig)
        {
            if (!TryParseParam(nodeConfig.param, out NPBlackboardConditionParam param))
            {
                Debug.LogError("BlackboardCondition can't parse param");
                return null;
            }
            
            if (!TryGetDecoratee(nodeConfig, out Node decoratee))
            {
                Debug.LogError("BlackboardCondition decoratee can't be null!");
                return null;
            }

            if (param.valueString != null)
            {
                return new BlackboardCondition(param.key, param.operators, param.valueString, param.stopsOnChange, decoratee);
            }
            return new BlackboardCondition(param.key, param.operators, param.stopsOnChange, decoratee);
        }

        BlackboardQuery CreateBlackboardQuery(NodeConfig nodeConfig)
        {
            if (!TryParseParam(nodeConfig.param, out NPBlackboardQueryParam param))
            {
                Debug.LogError("BlackboardQuery can't parse param");
                return null;
            }
            
            if (!TryGetDecoratee(nodeConfig, out Node decoratee))
            {
                Debug.LogError("BlackboardQuery decoratee can't be null!");
                return null;
            }

            List<object> functionList = GetFunctionListByName(param.queryFuncName);
            Func<bool> func = GetFunction<Func<bool>>(functionList);
            if (func == null)
            {
                Debug.LogWarning("BlackboardQuery func not found! param:" + nodeConfig.param);
                func = () => false;
            }
            
            return new BlackboardQuery(param.keys, param.stopsOnChange, func, decoratee);
        }
        
        Condition CreateCondition(NodeConfig nodeConfig)
        {
            if (!TryParseParam(nodeConfig.param, out NPConditionParam param))
            {
                Debug.LogError("Condition can't parse param");
                return null;
            }
            
            if (!TryGetDecoratee(nodeConfig, out Node decoratee))
            {
                Debug.LogError("Condition decoratee can't be null!");
                return null;
            }
            
            Func<bool> conditionFunc = null;
            if (!string.IsNullOrEmpty(param.functionName))
            {
                List<object> functionList = GetFunctionListByName(param.functionName);
                conditionFunc = GetFunction<Func<bool>>(functionList);
            }
            if (conditionFunc == null)
            {
                Debug.LogWarning("Condition func not found! param:" + nodeConfig.param);
                conditionFunc = () => false;
            }
            if (param.checkInterval >= 0 &&  param.randomVariation >= 0)
            {
                return new Condition(conditionFunc, param.stopsOnChange, param.checkInterval, param.randomVariation, decoratee);
            }
            return new Condition(conditionFunc, param.stopsOnChange, decoratee);
        }
        
        Cooldown CreateCooldown(NodeConfig nodeConfig)
        {
            if (!TryParseParam(nodeConfig.param, out NPCooldownParam param))
            {
                Debug.LogError("Cooldown can't parse param");
                return null;
            }
            
            if (!TryGetDecoratee(nodeConfig, out Node decoratee))
            {
                Debug.LogError("Cooldown decoratee can't be null!");
                return null;
            }

            if (param.randomVariation >= 0)
            {
                return new Cooldown(param.cooldownTime, param.randomVariation, param.startAfterDecoratee, param.resetOnFailiure, param.failOnCooldown, decoratee);
            }

            return new Cooldown(param.cooldownTime, param.startAfterDecoratee, param.resetOnFailiure, param.failOnCooldown, decoratee);
        }

        Failer CreateFailerNode(NodeConfig nodeConfig)
        {
            if (!TryGetDecoratee(nodeConfig, out Node decoratee))
            {
                Debug.LogError("Failer decoratee can't be null!");
                return null;
            }
            return new Failer(decoratee);
        }
        
        Inverter CreateInverterNode(NodeConfig nodeConfig)
        {
            if (!TryGetDecoratee(nodeConfig, out Node decoratee))
            {
                Debug.LogError("Inverter decoratee can't be null!");
                return null;
            }
            return new Inverter(decoratee);
        }
        
        Observer CreateObserverNode(NodeConfig nodeConfig)
        {
            if (!TryParseParam(nodeConfig.param, out NPObserverParam param))
            {
                Debug.LogError("Wait can't parse param");
                return null;
            }
            
            if (!TryGetDecoratee(nodeConfig, out Node decoratee))
            {
                Debug.LogError("Observer decoratee can't be null!");
                return null;
            }
            
            System.Action onStartFunc = null;
            Action<bool> onStopFunc = null;

            List<object> functionList = GetFunctionListByName(param.OnStartFunc);
            onStartFunc = GetFunction<System.Action>(functionList);
            if (onStartFunc == null)
            {
                Debug.LogWarning("Observer Start func not found! param:" + nodeConfig.param);
            }
            
            functionList = GetFunctionListByName(param.OnStopFunc);
            onStopFunc = GetFunction<Action<bool>>(functionList);
            if (onStopFunc == null)
            {
                Debug.LogWarning("Observer Stop func not found! param:" + nodeConfig.param);
            }
            
            return new Observer(onStartFunc, onStopFunc, decoratee);
        }

        Random CreateRandomNode(NodeConfig nodeConfig)
        {
            if (!TryParseParam(nodeConfig.param, out NPRandomParam param))
            {
                Debug.LogError("Random can't parse param");
                return null;
            }
            
            if (!TryGetDecoratee(nodeConfig, out Node decoratee))
            {
                Debug.LogError("Random decoratee can't be null!");
                return null;
            }
            return new Random(param.probability, decoratee);
        }
        
        Repeater CreateRepeaterNode(NodeConfig nodeConfig)
        {
            if (!TryParseParam(nodeConfig.param, out NPRepeaterParam param))
            {
                Debug.LogError("Repeater can't parse param");
                return null;
            }
            
            if (!TryGetDecoratee(nodeConfig, out Node decoratee))
            {
                Debug.LogError("Repeater decoratee can't be null!");
                return null;
            }
            
            return new Repeater(param.loopCount, decoratee);
        }

        Service CreateServiceNode(NodeConfig nodeConfig)
        {
            if (!TryParseParam(nodeConfig.param, out NPServiceParam param))
            {
                Debug.LogError("Service can't parse param");
                return null;
            }
            
            if (!TryGetDecoratee(nodeConfig, out Node decoratee))
            {
                Debug.LogError("Service decoratee can't be null!");
                return null;
            }

            System.Action serviceFunc = null;
            if (!string.IsNullOrEmpty(param.functionName))
            {
                List<object> functionList = GetFunctionListByName(param.functionName);
                serviceFunc = GetFunction<System.Action>(functionList);
                if (serviceFunc == null)
                {
                    Debug.LogWarning("Server func not found! param:" + nodeConfig.param);
                }
            }

            if (param.interval >= 0 && param.randomVariation >= 0)
            {
                return new Service(param.interval, param.randomVariation, serviceFunc, decoratee);
            }

            if (param.interval >= 0)
            {
                return new Service(param.interval, serviceFunc, decoratee);
            }
            
            return new Service(serviceFunc, decoratee);
        }

        Succeeder CreateSucceeder(NodeConfig nodeConfig)
        {
            if (!TryGetDecoratee(nodeConfig, out Node decoratee))
            {
                Debug.LogError("Succeeder decoratee can't be null!");
                return null;
            }
            return new Succeeder(decoratee);
        }

        TimeMax CreateTimeMax(NodeConfig nodeConfig)
        {
            if (!TryParseParam(nodeConfig.param, out NPTimeMaxParam param))
            {
                Debug.LogError("TimeMax can't parse param");
                return null;
            }
            
            if (!TryGetDecoratee(nodeConfig, out Node decoratee))
            {
                Debug.LogError("TimeMax decoratee can't be null!");
                return null;
            }
            
            if (param.randomVariation < 0)
            {
                return new TimeMax(param.limit, param.waitForChildButFailOnLimitReached, decoratee);
            }
            
            return new TimeMax(param.limit, param.randomVariation, param.waitForChildButFailOnLimitReached, decoratee);
        }
        
        TimeMin CreateTimeMin(NodeConfig nodeConfig)
        {
            if (!TryParseParam(nodeConfig.param, out NPTimeMinParam param))
            {
                Debug.LogError("TimeMin can't parse param");
                return null;
            }
            
            if (!TryGetDecoratee(nodeConfig, out Node decoratee))
            {
                Debug.LogError("TimeMin decoratee can't be null!");
                return null;
            }
            
            if (param.randomVariation < 0)
            {
                return new TimeMin(param.limit, param.waitOnFailure, decoratee);
            }
            
            return new TimeMin(param.limit, param.randomVariation, param.waitOnFailure, decoratee);
        }
        
        WaitForCondition CreateWaitForCondition(NodeConfig nodeConfig)
        {
            if (!TryParseParam(nodeConfig.param, out NPWaitForConditionParam param))
            {
                Debug.LogError("WaitForCondition can't parse param");
                return null;
            }
            
            if (!TryGetDecoratee(nodeConfig, out Node decoratee))
            {
                Debug.LogError("WaitForCondition decoratee can't be null!");
                return null;
            }
            
            Func<bool> conditionFunc = null;
            if (!string.IsNullOrEmpty(param.functionName))
            {
                List<object> functionList = GetFunctionListByName(param.functionName);
                conditionFunc = GetFunction<Func<bool>>(functionList);
            }
            if (conditionFunc == null)
            {
                Debug.LogWarning("WaitForCondition func not found! param:" + nodeConfig.param);
                conditionFunc = () => false;
            }
            if (param.checkInterval >= 0 &&  param.randomVariation >= 0)
            {
                return new WaitForCondition(conditionFunc, param.checkInterval, param.randomVariation, decoratee);
            }
            return new WaitForCondition(conditionFunc, decoratee);
        }

        bool TryParseParam<T>(string json, out T result)
        {
            if (!string.IsNullOrEmpty(json))
            {
                result = JsonUtility.FromJson<T>(json);
                return true;
            }
            result = default(T);
            return false;
        }

        bool TryGetDecoratee(NodeConfig nodeConfig, out Node node)
        {
            node = null;
            if (nodeConfig.nodes.Length == 0)
            {
                return false;
            }
            node = CreateNode(nodeConfig.nodes[0]);
            return node != null;
        }

    }
}

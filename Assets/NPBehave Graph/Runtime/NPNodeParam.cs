using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NPBehave
{
    [Serializable]
    public class NPActionParam
    {
        public string functionName;
    }
    
    [Serializable]
    public class NPServiceParam
    {
        public float interval;
        public float randomVariation;
        public string functionName;
    }

    [Serializable]
    public class NPParallelParam
    {
        public Parallel.Policy successPolicy;
        public Parallel.Policy failurePolicy;
    }
    
    [Serializable]
    public class NPNavWalkToParam
    {
        public string blackboardKey;
        public float tolerance;
        public bool stopOnTolerance;
        public float updateFrequency;
        public float updateVariance;
    }

    public enum WaitNodeType
    {
        Normal,
        BlackboardKey,
        Func
    }
    
    [Serializable]
    public class NPWaitParam
    {
        public WaitNodeType waitNodeType;
        public float seconds;
        public float randomVariation;
        public string blackboardKey;
        public string functionName;
    }
    
    [Serializable]
    public class NPWaitUntilStoppedParam
    {
        public bool successWhenStopped;
    }
    
    [Serializable]
    public class NPBlackboardConditionParam
    {
        public string key;
        public Operator operators;
        public string valueString;
        public int valueInt;
        public bool valueBool;
        public Stops stopsOnChange;
    }
    
    [Serializable]
    public class NPBlackboardQueryParam
    {
        public string[] keys;
        public Stops stopsOnChange;
        public string queryFuncName;
    }
    
    [Serializable]
    public class NPConditionParam
    {
        public string functionName;
        public Stops stopsOnChange;
        public float checkInterval;
        public float randomVariation;
    }
    
    [Serializable]
    public class NPCooldownParam
    {
        public float cooldownTime;
        public float randomVariation;
        public bool startAfterDecoratee;
        public bool resetOnFailiure;
        public bool failOnCooldown;
    }
    
    [Serializable]
    public class NPObserverParam
    {
        public string OnStartFunc;
        public string OnStopFunc;
    }

    [Serializable]
    public class NPRandomParam
    {
        public float probability;
    }

    [Serializable]
    public class NPRepeaterParam
    {
        public int loopCount;
    }

    public class NPTimeMaxParam
    {
        public float limit;
        public float randomVariation;
        public bool waitForChildButFailOnLimitReached;
    }
    
    public class NPTimeMinParam
    {
        public float limit;
        public float randomVariation;
        public bool waitOnFailure;
    }
    
    public class NPWaitForConditionParam
    {
        public string functionName;
        public float checkInterval;
        public float randomVariation;
    }
}


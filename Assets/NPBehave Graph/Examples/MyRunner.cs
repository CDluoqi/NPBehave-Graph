using System.Collections;
using System.Collections.Generic;
using NPBehave;
using UnityEngine;

public class MyRunner : NPBehaveGraphRunner
{
    [FunctionName("LogTest0", FuncPurpose.Condition)]
    private bool LogTest0()
    {
        return true;
    }
    
    [FunctionName("LogTest1", FuncPurpose.Action, "This text is used to explain how to use this function")]
    private Action.Result LogTest1(bool aborted)
    {
        if (aborted)
        {
            Debug.LogError("FAILED");
            return Action.Result.FAILED;
        }
        //Debug.LogError("PROGRESS");
        return Action.Result.PROGRESS;
    }

    private int runCount = 0;
    [FunctionName("Service", FuncPurpose.Service)]
    private void Service()
    {
        string near = blackboard.Get<string>("Near");
        near = near == "true" ? "false" : "true";
        blackboard.Set("Near", near);
    }

    [FunctionName("Condition", FuncPurpose.Condition)]
    private void Condition()
    {
        return;
    }
    
    [FunctionName("ObserverOnStart", FuncPurpose.ObserverOnStart)]
    private void ObserverOnStart()
    {
        Debug.LogError("ObserverOnStart");
        return;
    }
    
    [FunctionName("ObserverOnStop", FuncPurpose.ObserverOnStop)]
    private void ObserverOnStop(bool result)
    {
        Debug.LogError("ObserverOnStop " + result);
        return;
    }
    
    [FunctionName("WaitForCondition", FuncPurpose.WaitForCondition)]
    private bool WaitForCondition()
    {
        Debug.LogError("WaitForCondition ");
        return true;
    }

    [FunctionName("BlackboardQuery", FuncPurpose.BlackboardQuery)]
    private bool BlackboardQuery()
    {
        return true;
    }
}

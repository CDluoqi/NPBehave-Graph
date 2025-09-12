namespace NPBehave
{
    public enum NPBehaveNodeType
    {
        Unknown,
        
        
        Root,
        Selector,
        Sequence,
        Parallel,
        RandomSelector,
        RandomSequence,
        
        Action,
        NavWalkTo,
        Wait,
        WaitUntilStopped,
        
        BlackboardCondition,
        BlackboardQuery,
        Condition,
        Cooldown,
        Failer,
        Inverter,
        Observer,
        Random,
        Repeater,
        Service,
        Succeeder,
        TimeMax,
        TimeMin,
        WaitForCondition
    }
}

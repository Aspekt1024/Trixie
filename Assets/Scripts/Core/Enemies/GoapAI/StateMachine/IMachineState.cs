using System;
using System.Collections.Generic;

public interface IMachineState
{
    List<ISmTransition> Transitions { get; set; }

    void Enter();
    void Exit();
    void Init(GoapStateMachine stateMachine);
    bool IsActive();

    int GetPriority();
}

public interface ISmTransition
{
    Type TransitionCheck(IMachineState state);
    int GetPriority();
}

// you can inherit your FSM's transition from this, but feel free to implement your own (note: must implement ISmTransition and IComparable<ISmTransition>)
public class SmTransition : ISmTransition, IComparable<ISmTransition>
{
    private readonly int priority;
    private readonly Func<IMachineState, Type> checkFunc;

    public SmTransition(int priority, Func<IMachineState, Type> checkFunc)
    {
        this.priority = priority;
        this.checkFunc = checkFunc;
    }

    public Type TransitionCheck(IMachineState state)
    {
        return checkFunc(state);
    }

    public int GetPriority()
    {
        return priority;
    }

    public int CompareTo(ISmTransition other)
    {
        return -GetPriority().CompareTo(other.GetPriority());
    }
}
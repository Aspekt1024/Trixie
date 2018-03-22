using System;
using UnityEngine;
using Aspekt.AI;
using TestUnitLabels;

public class MoveToTargetAction : AIAction
{
    public Transform target;

    private MoveState moveState;
    
    public override void Enter(AIStateMachine stateMachine, Action SuccessCallback, Action FailureCallback)
    {
        base.Enter(stateMachine, SuccessCallback, FailureCallback);

        target = GameObject.Find("Cube").transform;
            
        moveState = stateMachine.AddState<MoveState>();
        moveState.SetTarget(target);

        stateMachine.OnComplete += Success;
    }

    protected override void Exit()
    {
        stateMachine.OnComplete -= Success;
    }

    protected override void Update()
    {
        // Use if target changes
        //moveState.SetTarget(target);
    }

    protected override void SetPreconditions()
    {

    }

    protected override void SetEffects()
    {
        AddEffect(AILabels.TargetReached.ToString(), true);
    }
}

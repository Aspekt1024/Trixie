using System;
using UnityEngine;
using Aspekt.AI;
using TestUnitLabels;

public class SellWoodAction : AIAction
{
    public Transform target;

    private MoveState moveState;

    public override void Enter(AIStateMachine stateMachine, Action SuccessCallback, Action FailureCallback)
    {
        base.Enter(stateMachine, SuccessCallback, FailureCallback);

        target = GameObject.Find("Store").transform;

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
        AddPrecondition(AILabels.HasWood.ToString(), true);
    }

    protected override void SetEffects()
    {
        AddEffect(AILabels.HasWood.ToString(), false);
        AddEffect(AILabels.ObjectHeld.ToString(), false);
        AddEffect(AILabels.SellWood.ToString(), true);
    }
}

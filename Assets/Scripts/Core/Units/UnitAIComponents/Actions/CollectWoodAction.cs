using System;
using UnityEngine;
using Aspekt.AI;
using TestUnitLabels;

public class CollectWoodAction : AIAction
{
    public Transform target;

    private MoveState moveState;
    
    public override void Enter(AIStateMachine stateMachine, Action SuccessCallback, Action FailureCallback)
    {
        base.Enter(stateMachine, SuccessCallback, FailureCallback);

        GameObject woodObject = GameObject.Find("Wood");
        target = woodObject.transform;
        moveState = stateMachine.AddState<MoveState>();
        moveState.SetTarget(target);

        stateMachine.OnComplete += Success;
    }

    public override bool CheckProceduralPrecondition()
    {
        GameObject woodObject = GameObject.Find("Wood");
        return woodObject != null;
    }

    protected override void Exit()
    {
        Destroy(target.gameObject);
        stateMachine.OnComplete -= Success;
    }

    protected override void Update()
    {
        // Use if target changes
        //moveState.SetTarget(target);
    }

    protected override void SetPreconditions()
    {
        AddPrecondition(AILabels.ObjectHeld.ToString(), false);
    }

    protected override void SetEffects()
    {
        AddEffect(AILabels.ObjectHeld.ToString(), true);
        AddEffect(AILabels.HasWood.ToString(), true);
    }
}

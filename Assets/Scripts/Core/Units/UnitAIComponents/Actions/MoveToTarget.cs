using System;
using UnityEngine;
using Aspekt.AI;
using TestUnitLabels;
using TrixieCore.Units;

public class MoveToTarget : AIAction
{
    public float ReachedDistance = 10f;

    private MoveState moveState;
    
    public event Action<EnergyTypes.Colours> OnShootPreparation = delegate { };

    public override void Enter(AIStateMachine stateMachine, Action SuccessCallback, Action FailureCallback)
    {
        base.Enter(stateMachine, SuccessCallback, FailureCallback);
        moveState = stateMachine.AddState<MoveState>();
        moveState.SetMovementBehaviour(agent.BaseUnit.GetMovementBehaviour());
        moveState.SetTarget(Player.Instance.transform);

        stateMachine.OnComplete += MovementComplete;
    }

    private void MovementComplete()
    {
        if (agent.GetMemory().ConditionMet(SauceLabels.CanSeeTarget, true))
        {
            Success();
        }
        else
        {
            Failure();
        }
    }

    protected override void Exit()
    {
        stateMachine.OnComplete -= MovementComplete;
    }

    protected override void Update()
    {
        if (moveState != null)
        {
            moveState.SetTarget(Player.Instance.transform);

            if (NearTarget() && agent.GetMemory().ConditionMet(SauceLabels.CanSeeTarget, true))
            {
                moveState.SetTargetReached();
            }
        }

    }

    protected override void SetPreconditions()
    {
    }

    protected override void SetEffects()
    {
        AddEffect(SauceLabels.CanShootTarget, true);
        AddEffect(SauceLabels.CanSeeTarget, true);
    }

    private bool NearTarget()
    {
        return Vector2.Distance(Player.Instance.transform.position, agent.BaseUnit.transform.position) < ReachedDistance;
    }
}

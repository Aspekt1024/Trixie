using System;
using UnityEngine;
using Aspekt.AI;
using TestUnitLabels;

public class AttackAction2 : AIAction
{
    public Transform target;
    
    private MoveState moveState;

    public event Action<EnergyTypes.Colours> OnShootPreparation = delegate { };

    public override void Enter(AIStateMachine stateMachine, Action SuccessCallback, Action FailureCallback)
    {
        base.Enter(stateMachine, SuccessCallback, FailureCallback);

        target = Player.Instance.transform;
        
        moveState = stateMachine.AddState<MoveState>();
        moveState.SetMovementBehaviour(GetMovementBehaviour());
        moveState.SetTarget(target);

        stateMachine.OnComplete += Attack;
    }

    private void Attack()
    {
        Success();
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
        AddEffect(AILabels.TargetReached, true);
    }

    private IAIMovementBehaviour GetMovementBehaviour()
    {
        AIAgent agent = GetComponentInParent<AIAgent>();
        return agent.Owner.GetComponent<TrixieCore.Units.BaseUnit>().GetMovementBehaviour();
    }
}

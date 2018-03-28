using System;
using UnityEngine;
using Aspekt.AI;
using TestUnitLabels;
using TrixieCore.Units;

public class RangedAttackColouredAction : AIAction
{
    public event Action<EnergyTypes.Colours> OnShootPreparation = delegate { };

    public override void Enter(AIStateMachine stateMachine, Action SuccessCallback, Action FailureCallback)
    {
        base.Enter(stateMachine, SuccessCallback, FailureCallback);

        agent.BaseUnit.GetAbility<ShootComponent>().Shoot(Player.Instance.gameObject);

        OnShootComplete();
    }

    private void OnShootComplete()
    {
        if (Player.Instance.IsAlive)
        {
            Failure();
        }
        else
        {
            Success();
        }

    }

    protected override void Update()
    {
    }

    public override bool CheckProceduralPrecondition()
    {
        bool result = !agent.BaseUnit.GetAbility<ShootComponent>().IsOnCooldown();
        return result;
    }

    protected override void SetPreconditions()
    {
        AddPrecondition(SauceLabels.CanShootTarget, true);
        AddPrecondition(SauceLabels.CanSeeTarget, true);
        AddPrecondition(SauceLabels.HasCorrectProjectColour, true);
    }

    protected override void SetEffects()
    {
        AddEffect(SauceLabels.TargetAttacked, true);
    }
    
}

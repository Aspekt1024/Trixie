using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Aspekt.AI;
using TestUnitLabels;
using System;

namespace TrixieCore.Units
{
    public class EnableShieldAction : AIAction
    {
        protected override void SetEffects()
        {
            AddEffect(SauceLabels.IsShielded, true);
            AddEffect(SauceLabels.AttackGoalComplete, true);
        }

        protected override void SetPreconditions()
        {
            AddPrecondition(SauceLabels.IsShielded, false);
            AddPrecondition(SauceLabels.CanShield, true);
        }

        protected override void Run()
        {
        }

        public override void Enter(AIStateMachine stateMachine, Action SuccessCallback, Action FailureCallback)
        {
            base.Enter(stateMachine, SuccessCallback, FailureCallback);

            agent.BaseUnit.GetAbility<EnemyShield>().Activate();

            Success();
        }
    }
}

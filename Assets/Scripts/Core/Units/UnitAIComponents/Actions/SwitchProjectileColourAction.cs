using Aspekt.AI;
using System;
using TestUnitLabels;

namespace TrixieCore.Units
{
    public class SwitchProjectileColourAction : AIAction
    {
        private ShootComponent shootComponent;

        public override void Enter(AIStateMachine stateMachine, Action SuccessCallback, Action FailureCallback)
        {
            base.Enter(stateMachine, SuccessCallback, FailureCallback);

            shootComponent = agent.BaseUnit.GetAbility<ShootComponent>();
        }

        protected override void Run(float deltaTime)
        {
            if (agent.GetMemory().ConditionMet(SauceLabels.ProjectileColour, EnergyTypes.Colours.Blue))
            {
                var projectileSettings = shootComponent.ProjectileSettings;
                shootComponent.ProjectileSettings.ProjectileColour = EnergyTypes.Colours.Green;
                agent.GetMemory().Set(SauceLabels.ProjectileColour, EnergyTypes.Colours.Green);
            }
            else
            {
                shootComponent.ProjectileSettings.ProjectileColour = EnergyTypes.Colours.Blue;
                agent.GetMemory().Set(SauceLabels.ProjectileColour, EnergyTypes.Colours.Blue);
            }
            Success();
        }

        protected override void SetEffects()
        {
            AddEffect(SauceLabels.HasCorrectProjectileColour, true);
        }

        protected override void SetPreconditions()
        {
        }
    }
}

﻿using System;
using UnityEngine;
using Aspekt.AI;
using TestUnitLabels;
using System.Collections.Generic;

namespace TrixieCore.Units
{
    public class RangedAttackColouredAction : AIAction
    {
        public float IndicationTime = 0.3f;
        public float ShootCooldown = 1f;
        
        private float delayTimer;
        private ShootComponent shootComponent;

        public override void Enter(AIStateMachine stateMachine, Action SuccessCallback, Action FailureCallback)
        {
            base.Enter(stateMachine, SuccessCallback, FailureCallback);

            shootComponent = agent.BaseUnit.GetAbility<ShootComponent>();
            agent.BaseUnit.GetEffect<ColourIndicator>().Show(shootComponent.ProjectileSettings.ProjectileColour);
            delayTimer = 0f;
        }

        protected override void Run(float deltaTime)
        {
            agent.BaseUnit.LookAtPosition(Trixie.Instance.transform.position);

            delayTimer += deltaTime;
            if (delayTimer >= IndicationTime)
            {
                var newProjectiles = shootComponent.Shoot(Trixie.Instance.gameObject);
                foreach (Projectile projectile in newProjectiles)
                {
                    projectile.OnDestroyed += ProjectileDestroyed;
                }
                Success();
            }
        }

        private void ProjectileDestroyed(Projectile projectile, bool destroyedBySameColouredShield)
        {
            if (destroyedBySameColouredShield)
            {
                agent.GetMemory().Set(SauceLabels.HasCorrectProjectileColour, false);
            }
            projectile.OnDestroyed -= ProjectileDestroyed;
        } 

        public override bool CheckProceduralPrecondition()
        {
            bool result = Time.time > agent.BaseUnit.GetAbility<ShootComponent>().GetTimeLastShot() + ShootCooldown;
            return result;
        }

        protected override void SetPreconditions()
        {
            AddPrecondition(SauceLabels.CanShootTarget, true);
            AddPrecondition(SauceLabels.CanSeeTarget, true);
            AddPrecondition(SauceLabels.HasCorrectProjectileColour, true);
        }

        protected override void SetEffects()
        {
            AddEffect(SauceLabels.AttackGoalComplete, true);
        }
    }
}

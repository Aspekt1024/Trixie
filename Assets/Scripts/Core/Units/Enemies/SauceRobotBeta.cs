using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Aspekt.AI;

namespace TrixieCore.Units
{
    public class SauceRobotBeta : BaseUnit, IDamageable, IStunnable
    {
        private AIAgent agent;
        private StunHandler stun;
        private HealthComponent health;
        
        public void Stun(Vector2 direction, float duration, EnergyTypes.Colours energyType)
        {
            if (GetAbility<EnemyShield>().IsActive || health.IsDead()) return;

            agent.Pause();
            stun.BeginStun(duration);
        }

        public void Unstunned()
        {
            if (health.IsAlive())
            {
                agent.Unpause();
            }
        }

        public void TakeDamage(int damage, Vector2 direction, EnergyTypes.Colours damageType)
        {
            if (GetAbility<EnemyShield>().IsActive) return;

            health.TakeDamage(damage);
            if (health.IsDead())
            {
                Destroy();
            }
        }

        public bool IsDestroyed()
        {
            return health.IsDead();
        }

        private void Start()
        {
            agent = AI.GetComponent<AIAgent>();
            agent.Activate();

            health = GetComponent<HealthComponent>();
            stun = GetComponent<StunHandler>();
        }

        protected override void Destroy()
        {
            base.Destroy();
            agent.Stop();
            effects.GetEffect<ExplosionEffect>().Explode();
        }
    }
}


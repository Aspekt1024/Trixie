using Aspekt.PlayerController;
using TrixieCore.Units;
using UnityEngine;

namespace TrixieCore
{
    public class Trixie : Player
    {
        public new static Trixie Instance { get; private set; }

        private PlayerHealthComponent health;

        protected override void Startup()
        {
            SetState(StateLabels.IsAlive, true);
            health = GetComponent<PlayerHealthComponent>();
            Instance = this;
        }

        public void Damage(IDamager obj)
        {
            health.TakeDamage();
        }

        public void HitWithObject(UnitAbility ability)
        {
            Debug.Log("player hit by " + ability.GetType());
        }
    }
}

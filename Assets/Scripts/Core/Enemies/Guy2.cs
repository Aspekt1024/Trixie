using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrixieCore
{
    public class Guy2 : BaseEnemy
    {

        public float ShieldCooldown = 5f;

        private BasicPatrolComponent patrolComponent;
        public EnemyShield Shield;
        
        private void Start()
        {
            patrolComponent = GetComponent<BasicPatrolComponent>();

            Shield.ShieldColour = EnergyTypes.Colours.Red;
            Shield.SetCooldownDuration(ShieldCooldown);
            Shield.Deactivate();
        }

        protected override void Update()
        {
            base.Update();

            if (IsDead()) return;

            hasAggro = Vector2.Distance(Player.Instance.transform.position, transform.position) < AggroRadius;

            if (Shield.IsActive() || !hasAggro) return;
            Shield.Activate();
            
        }

        protected override IEnumerator ShowDamaged(Vector2 direction)
        {
            patrolComponent.Deactivate();
            SetSpriteColour(new Color(1f, 0f, 0f, 0.5f));
            body.velocity = (direction + Vector2.up * 4).normalized * 14f;

            AudioMaster.PlayAudio(AudioMaster.AudioClips.Hit1);

            yield return new WaitForSeconds(0.8f);

            SetSpriteColour(Color.white);
            patrolComponent.Activate();
        }

        protected override void DestroyEnemy()
        {
            base.DestroyEnemy();
            patrolComponent.DeactivateImmediate();
        }

        public override void DamageEnemy(Vector2 direction, EnergyTypes.Colours energyType, int damage = 1)
        {
            if (!Shield.IsActive())
            {
                base.DamageEnemy(direction, energyType, damage);
            }
        }

    }
}


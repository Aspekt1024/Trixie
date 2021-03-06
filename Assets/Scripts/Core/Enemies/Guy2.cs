﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrixieCore.Units
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
            Shield.Deactivate();
        }

        protected override void Update()
        {
            base.Update();

            if (IsDestroyed()) return;

            if (isStunned)
            {
                patrolComponent.Deactivate();
            }

            hasAggro = Vector2.Distance(Trixie.Instance.transform.position, transform.position) < AggroRadius;
            
            if (Shield.IsActive || !hasAggro) return;
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
        }

        protected override void DestroyEnemy()
        {
            base.DestroyEnemy();
            patrolComponent.DeactivateImmediate();
        }

        public override void TakeDamage(int damage, Vector2 direction, EnergyTypes.Colours energyType)
        {
            if (!Shield.IsActive)
            {
                base.TakeDamage(damage, direction, energyType);
            }
        }
    }
}


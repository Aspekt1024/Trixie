using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrixieCore;


namespace TrixieCore.Units
{
    public class EnemyGuy : BaseEnemy
    {

        private BasicPatrolComponent patrolComponent;

        private void Start()
        {
            patrolComponent = GetComponent<BasicPatrolComponent>();
            patrolComponent.Activate();

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
            patrolComponent.Deactivate();
        }

    }

}

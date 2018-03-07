using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrixieCore
{
    public class MeleeCollider : MonoBehaviour
    {

        private MeleeComponent meleeComponent;
        private PolygonCollider2D meleeCollider;

        private void Start()
        {
            meleeComponent = Player.Instance.GetComponent<MeleeComponent>();
            meleeCollider = GetComponent<PolygonCollider2D>();
            meleeCollider.enabled = false;
        }

        public void EnableCollider()
        {
            meleeCollider.enabled = true;
        }

        public void DisableCollider()
        {
            meleeCollider.enabled = false;
        }

        public EnergyTypes.Colours GetColour()
        {
            return meleeComponent.GetMeleeColour();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.layer == TrixieLayers.GetMask(Layers.Enemy))
            {
                if (collision.tag == "Enemy")
                {
                    HitEnemy(collision.GetComponent<BaseEnemy>());
                }
                else if (collision.tag == "Shield")
                {
                    collision.GetComponent<EnemyShield>().HitShield(meleeComponent.GetMeleeColour());
                }
            }
        }

        private void HitEnemy(BaseEnemy enemy)
        {
            Vector2 direction = enemy.transform.position - transform.position;
            switch (meleeComponent.GetMeleeColour())
            {
                case EnergyTypes.Colours.Blue:
                    enemy.DamageEnemy(direction, meleeComponent.GetMeleeColour(), 1);
                    break;
                case EnergyTypes.Colours.Red:
                    enemy.DamageEnemy(direction, meleeComponent.GetMeleeColour(), 1);
                    enemy.Stun(direction, 1.5f);
                    break;
                case EnergyTypes.Colours.Green:
                    enemy.Stun(direction, 1);
                    break;
                default:
                    break;
            }
        }
    }
}

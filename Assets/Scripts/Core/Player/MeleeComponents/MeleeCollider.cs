using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrixieCore.Units;

namespace TrixieCore
{
    public class MeleeCollider : MonoBehaviour
    {
        private MeleeComponent meleeComponent;
        private PolygonCollider2D meleeCollider;

        private void Start()
        {
            meleeComponent = Trixie.Instance.GetComponent<MeleeComponent>();
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
                    HitEnemy(collision);
                }
            }
        }

        private void HitEnemy(Collider2D unit)
        {
            Vector2 direction = unit.transform.position - transform.position;
            switch (meleeComponent.GetMeleeColour())
            {
                case EnergyTypes.Colours.Blue:
                    unit.GetComponent<IDamageable>().TakeDamage(1, direction, meleeComponent.GetMeleeColour());
                    break;
                case EnergyTypes.Colours.Red:
                    unit.GetComponent<IDamageable>().TakeDamage(1, direction, meleeComponent.GetMeleeColour());
                    unit.GetComponent<IStunnable>().Stun(direction, 1.5f, meleeComponent.GetMeleeColour());
                    break;
                case EnergyTypes.Colours.Green:
                    unit.GetComponent<IStunnable>().Stun(direction, 1.5f, meleeComponent.GetMeleeColour());
                    break;
                default:
                    break;
            }
        }
    }
}

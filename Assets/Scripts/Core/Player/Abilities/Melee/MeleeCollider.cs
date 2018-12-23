using UnityEngine;
using TrixieCore.Units;

namespace TrixieCore
{
    public class MeleeCollider : MonoBehaviour
    {
        private MeleeAbility meleeAbility;
        private PolygonCollider2D meleeCollider;

        private void Start()
        {
            meleeAbility = Trixie.Instance.GetAbility<MeleeAbility>();
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
            return meleeAbility.GetMeleeColour();
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
            switch (meleeAbility.GetMeleeColour())
            {
                case EnergyTypes.Colours.Blue:
                    unit.GetComponent<IDamageable>().TakeDamage(1, direction, meleeAbility.GetMeleeColour());
                    break;
                case EnergyTypes.Colours.Red:
                    unit.GetComponent<IDamageable>().TakeDamage(1, direction, meleeAbility.GetMeleeColour());
                    unit.GetComponent<IStunnable>().Stun(direction, 1.5f, meleeAbility.GetMeleeColour());
                    break;
                case EnergyTypes.Colours.Green:
                    unit.GetComponent<IStunnable>().Stun(direction, 1.5f, meleeAbility.GetMeleeColour());
                    break;
                default:
                    break;
            }
        }
    }
}

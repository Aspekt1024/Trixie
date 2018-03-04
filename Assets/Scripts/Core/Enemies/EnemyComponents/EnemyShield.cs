using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrixieCore
{

    public class EnemyShield : MonoBehaviour
    {

        public EnergyTypes.Colours ShieldColour;

        private Collider2D coll;
        private SpriteRenderer sr;

        private LayerMask[] layers;

        private void Start()
        {
            coll = GetComponent<Collider2D>();
            sr = GetComponent<SpriteRenderer>();

            layers = new LayerMask[]
            {
                TrixieLayers.GetMask(Layers.PlayerProjectile)
            };
        }

        public void Activate()
        {
            sr.enabled = true;
            coll.enabled = true;
        }

        public void Deactivate()
        {
            sr.enabled = false;
            coll.enabled = false;
        }

        public bool IsActive()
        {
            return coll.enabled;
        }

        public void HitShield(EnergyTypes.Colours energyColour, int damage = 1)
        {
            if (energyColour == ShieldColour)
            {
                Deactivate();
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.layer == TrixieLayers.GetMask(Layers.PlayerProjectile))
            {
                if (collision.GetComponent<Projectile>().GetColour() == ShieldColour)
                {
                    Deactivate();
                }
            }
        }
    }
}

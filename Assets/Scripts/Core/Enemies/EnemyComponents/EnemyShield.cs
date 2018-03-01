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

        private void Start()
        {
            coll = GetComponent<Collider2D>();
            sr = GetComponent<SpriteRenderer>();
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

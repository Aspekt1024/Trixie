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

        private enum States
        {
            Inactive, Active, MoveToInactive
        }
        private States state;

        private void Start()
        {
            coll = GetComponent<Collider2D>();
            sr = GetComponent<SpriteRenderer>();

            layers = new LayerMask[]
            {
                TrixieLayers.GetMask(Layers.PlayerProjectile)
            };
        }

        private void Update()
        {
            switch (state)
            {
                case States.Inactive:
                    break;
                case States.Active:
                    break;
                case States.MoveToInactive:
                    state = States.Inactive;
                    break;
                default:
                    break;
            }
        }

        public void Activate()
        {
            sr.enabled = true;
            coll.enabled = true;
            state = States.Active;
        }

        public void Deactivate()
        {
            sr.enabled = false;
            coll.enabled = false;
            state = States.MoveToInactive;
        }

        public bool IsActive()
        {
            return state == States.Active || state == States.MoveToInactive;
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

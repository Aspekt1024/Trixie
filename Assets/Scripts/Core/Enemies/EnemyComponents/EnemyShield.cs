using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrixieCore.Units
{

    public class EnemyShield : UnitAbility
    {
        public EnergyTypes.Colours ShieldColour;
        public float ShieldCooldown = 1f;
        public bool StunsPlayer = false;
        public float StunDuration = 2f;
        public bool CanDamagePlayer = false;
        public int DamageToPlayer = 1;

        private Collider2D coll;
        private SpriteRenderer sr;
        
        private float cooldownRemaining;

        private bool hitPlayer;

        private enum States
        {
            Inactive, Active, MoveToInactive
        }
        private States state;

        private void Awake()
        {
            coll = GetComponent<Collider2D>();
            sr = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            if (cooldownRemaining > 0)
            {
                cooldownRemaining -= Time.deltaTime;
            }

            if (hitPlayer && state == States.Active)
            {
                Player.Instance.HitWithObject(this);
                hitPlayer = false;
            }

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
            if (cooldownRemaining > 0)
            {
                return;
            }

            sr.enabled = true;
            coll.enabled = true;
            state = States.Active;
        }

        public void DeactivateWithCooldown()
        {
            cooldownRemaining = ShieldCooldown;
            Deactivate();
        }

        public void Deactivate()
        {
            sr.enabled = false;
            coll.enabled = false;
            state = States.MoveToInactive;
        }

        public bool IsActive { get { return state == States.Active || state == States.MoveToInactive; } }
        public bool IsAvailable { get { return cooldownRemaining <= 0f; } }

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
                    DeactivateWithCooldown();
                }
            }
            else if (collision.gameObject.layer == TrixieLayers.GetMask(Layers.Player))
            {
                MeleeCollider melee = collision.GetComponent<MeleeCollider>();
                if (melee != null && melee.GetColour() == ShieldColour)
                {
                    DeactivateWithCooldown();
                }

                if (melee == null)
                {
                    hitPlayer = true;
                }
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrixieCore.Units;

namespace TrixieCore
{
    public class BlueShieldAbility : BaseShieldAbility
    {
        public bool SlowsProjectiles = true;
        public float ProjectileSpeedMultiplier = 0.2f;
        public float EffectRadius = 20f;

        public enum EnemyEffects
        {
            Stun, Damage, NoEffect
        }
        public EnemyEffects EffectOnEnemies;
        public float EnemyStunTime = 1f;
        public int DamageToEnemies = 3;

        public enum ProjectileEffects
        {
            Destroy, NoEffect
        }
        public ProjectileEffects EffectOnProjectiles;

        public bool PermaSlowProjectiles;
        public float ProjectileSpeedReturnDelay = 1f;

        public float ShieldDuration = 3f;

        public enum ChargeTypes
        {
            Chargeup, Cooldown
        }
        public ChargeTypes ChargeType;
        public float ChargeupTime = 1.5f;
        public float CooldownTime = 1f;
        public GameObject Telegraph;

        private float chargeTimer;
        private float durationTimer;

        private HashSet<Projectile> projectiles = new HashSet<Projectile>();

        protected override void Awake()
        {
            base.Awake();
            Colour = EnergyTypes.Colours.Blue;
            shield.ProjectileCollider.enabled = false;
            Telegraph.SetActive(false);
        }

        public override void ActivatePressed()
        {
            Telegraph.SetActive(true);
            durationTimer = 0f;

            if (ChargeType == ChargeTypes.Chargeup)
            {
                state = States.Charging;
            }
        }

        public override bool ActivateReleased()
        {
            if (state != States.Charged && state != States.Charging) return false;

            shield.ChargeIndicator.StopCharge();
            Telegraph.SetActive(false);
            bool activateSuccess = false;

            if (state == States.Charged && shield.IsShielding())
            {
                activateSuccess = true;
                Activate();
            }

            if (ChargeType == ChargeTypes.Chargeup)
            {
                chargeTimer = 0f;
                if (state == States.Charging)
                {
                    state = States.None;
                }
            }

            ReleaseProjectiles();

            return activateSuccess;
        }

        public override void ReturnShield()
        {
            if (state != States.Returning && shield.gameObject.activeSelf)
            {
                state = States.Returning;
                StartCoroutine(ReturnShieldRoutine());
            }
        }

        public override void DisableShield()
        {
            if (state == States.Activating)
            {
                chargeTimer = 0f;
            }
            state = States.None;
            Telegraph.SetActive(false);
            shield.ChargeIndicator.StopCharge();
            ReleaseProjectiles();
        }

        public override void UpdateCharge(float deltaTime)
        {
            switch (state)
            {
                case States.None:
                    if (ChargeType == ChargeTypes.Cooldown)
                    {
                        if (chargeTimer > CooldownTime)
                        {
                            state = States.Charged;
                        }
                        else
                        {
                            state = States.Charging;
                        }
                    }
                    else
                    {
                        chargeTimer = 0f;
                    }
                    break;
                case States.Charging:
                    chargeTimer += deltaTime;
                    durationTimer += deltaTime;
                    shield.ChargeIndicator.SetCharge(chargeTimer / (ChargeType == ChargeTypes.Chargeup ? ChargeupTime : CooldownTime));
                    SlowProjectiles();
                    TransitionIfCharged();
                    break;
                case States.Charged:
                    durationTimer += deltaTime;
                    if (durationTimer >= ShieldDuration)
                    {
                        ActivateReleased();
                    }
                    SlowProjectiles();
                    break;
                case States.Activating:
                    ReturnShield();
                    break;
                case States.Returning:
                    break;
                default:
                    break;
            }
        }

        private void Activate()
        {
            state = States.Activating;

            LayerMask layerMask = 0;

            if (EffectOnProjectiles == ProjectileEffects.Destroy)
            {
                layerMask |= 1 << TrixieLayers.GetMask(Layers.Projectile);
            }
            if (DamageToEnemies > 0)
            {
                if (EffectOnEnemies == EnemyEffects.Damage || EffectOnEnemies == EnemyEffects.Stun)  // TODO check max charge
                {
                    layerMask |= 1 << TrixieLayers.GetMask(Layers.Enemy);
                }
            }

            Collider2D[] results = Physics2D.OverlapCircleAll(transform.position, EffectRadius, layerMask);

            for (int i = 0; i < results.Length; i++)
            {
                if (results[i].gameObject.layer == TrixieLayers.GetMask(Layers.Projectile))
                {
                    results[i].gameObject.GetComponent<Projectile>().Destroy();
                }
                else if (results[i].gameObject.layer == TrixieLayers.GetMask(Layers.Enemy))
                {
                    BaseUnit enemy = results[i].GetComponent<BaseUnit>();
                    if (EffectOnEnemies == EnemyEffects.Damage)
                    {
                        if (enemy is IDamageable)
                        {
                            ((IDamageable)enemy).TakeDamage(DamageToEnemies, results[i].transform.position - transform.position, EnergyTypes.Colours.Blue);
                        }
                    }
                    else if (EffectOnEnemies == EnemyEffects.Stun)
                    {
                        if (enemy is IStunnable)
                        {
                            ((IStunnable)enemy).Stun(results[i].transform.position - transform.position, EnemyStunTime, EnergyTypes.Colours.Blue);
                        }
                    }
                }
            }
        }

        private void SlowProjectiles()
        {
            if (!SlowsProjectiles) return;

            HashSet<Projectile> oldList = projectiles;
            projectiles = new HashSet<Projectile>();

            Collider2D[] results = Physics2D.OverlapCircleAll(transform.position, EffectRadius, 1 << TrixieLayers.GetMask(Layers.Projectile));

            for (int i = 0; i < results.Length; i++)
            {
                Projectile projectile = results[i].GetComponent<Projectile>();
                projectiles.Add(projectile);
                if (!oldList.Contains(projectile))
                {
                    projectile.SetSpeedMultiplier(ProjectileSpeedMultiplier);
                    projectile.OnDestroyed += RemoveDestroyedProjectileFromList;
                }
                oldList.Remove(projectile);
            }

            foreach (Projectile proj in oldList)
            {
                proj.SetSpeedMultiplier(1f);
                proj.OnDestroyed -= RemoveDestroyedProjectileFromList;
            }
        }

        private void ReleaseProjectiles()
        {
            foreach (Projectile projectile in projectiles)
            {
                if (!PermaSlowProjectiles)
                {
                    projectile.SetSpeedMultiplier(1f, ProjectileSpeedReturnDelay);
                }

                projectile.OnDestroyed -= RemoveDestroyedProjectileFromList;
            }

        }

        private void RemoveDestroyedProjectileFromList(Projectile projectile, bool destroyedBySameShieldColour)
        {
            projectiles.Remove(projectile);
            projectile.OnDestroyed -= RemoveDestroyedProjectileFromList;
        }

        private IEnumerator ReturnShieldRoutine()
        {
            body.isKinematic = true;
            ReleaseProjectiles();

            yield return null;

            shield.ShieldCollider.isTrigger = false;
            state = States.None;

            if (ChargeType == ChargeTypes.Cooldown)
            {
                chargeTimer = 0f;
            }
            shield.OnReturn();
        }

        private void TransitionIfCharged()
        {
            if (ChargeType == ChargeTypes.Chargeup && chargeTimer > ChargeupTime)
            {
                shield.ChargeIndicator.SetCharged(ChargeType == ChargeTypes.Cooldown);
                state = States.Charged;
            }
            else if (ChargeType == ChargeTypes.Cooldown && chargeTimer > CooldownTime)
            {
                shield.ChargeIndicator.SetCharged(ChargeType == ChargeTypes.Cooldown);
                state = States.Charged;
            }
        }
    }
}



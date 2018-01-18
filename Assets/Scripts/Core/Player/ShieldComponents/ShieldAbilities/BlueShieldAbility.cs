using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrixieCore;

public class BlueShieldAbility : BaseShieldAbility
{
    public bool DestroysProjectiles = true;
    public bool DamageOnMaxCharge = true;
    public int DamageToEnemies = 3;
    public bool SlowsProjectiles = true;
    public float ProjectileSpeedMultiplier = 0.2f;
    public float EffectRadius = 20f;
    public enum ChargeTypes
    {
        Chargeup, Cooldown
    }
    public ChargeTypes ChargeType;
    public float ChargeupTime = 1.5f;
    public float CooldownTime = 1f;
    public GameObject Telegraph;

    private float timer;

    private HashSet<Projectile> projectiles = new HashSet<Projectile>();

    protected override void Start()
    {
        base.Start();
        Colour = EnergyTypes.Colours.Blue;
        shield.ProjectileCollider.enabled = false;
        Telegraph.SetActive(false);
    }

    public override void ActivatePressed()
    {
        Telegraph.SetActive(true);

        if (ChargeType == ChargeTypes.Chargeup)
        {
            state = States.Charging;
        }
    }

    public override bool ActivateReleased()
    {
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
            timer = 0f;
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
            timer = 0f;
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
                    if (timer > CooldownTime)
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
                    timer = 0f;
                }
                break;
            case States.Charging:
                timer += deltaTime;
                shield.ChargeIndicator.SetCharge(timer / (ChargeType == ChargeTypes.Chargeup ? ChargeupTime : CooldownTime));
                SlowProjectiles();
                TransitionIfCharged();
                break;
            case States.Charged:
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

        if (DestroysProjectiles)
        {
            layerMask |= 1 << TrixieLayers.GetMask(Layers.Projectile);
        }
        if (DamageToEnemies > 0)
        {
            if ((DamageOnMaxCharge && true) || !DamageOnMaxCharge)  // TODO check max charge
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
                results[i].GetComponent<BaseEnemy>().DamageEnemy(results[i].transform.position - transform.position, DamageToEnemies);
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
            projectile.SetSpeedMultiplier(1f);
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
            timer = 0f;
        }
        shield.OnReturn();
    }

    private void TransitionIfCharged()
    {
        if (ChargeType == ChargeTypes.Chargeup && timer > ChargeupTime)
        {
            shield.ChargeIndicator.SetCharged(ChargeType == ChargeTypes.Cooldown);
            state = States.Charged;
        }
        else if (ChargeType == ChargeTypes.Cooldown && timer > CooldownTime)
        {
            shield.ChargeIndicator.SetCharged(ChargeType == ChargeTypes.Cooldown);
            state = States.Charged;
        }
    }
}

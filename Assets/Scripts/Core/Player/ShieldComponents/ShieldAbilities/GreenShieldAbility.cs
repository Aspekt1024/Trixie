﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrixieCore;

public class GreenShieldAbility : BaseShieldAbility
{
    public int ProjectileMultiplier = 1;
    public float Spread = 10f;
    public bool HasCooldown = true;
    public float CooldownTime = 1.5f;

    private int numProjectilesStored;
    private float timer;
    private ShieldCollisionHandler collisionHandler;
    
    private void Start()
    {
        Colour = EnergyTypes.Colours.Green;
        shield.ProjectileCollider.enabled = false;
        collisionHandler = shield.ShieldCollider.GetComponent<ShieldCollisionHandler>();
    }
    
    public override void UpdateCharge(float deltaTime)
    {
        switch (state)
        {
            case States.None:
                MoveToChargeState();
                break;
            case States.Charging:
                CheckCooldown(deltaTime);
                break;
            case States.Charged:
                break;
            case States.Activated:
                break;
            case States.Returning:
                break;
            default:
                break;
        }
    }

    public override void ActivatePressed()
    {
        if (state == States.None || state == States.Charged)
        {
            state = States.Activated;
        }
    }

    public override bool ActivateReleased()
    {
        if (HasCooldown)
        {
            state = timer >= CooldownTime ? States.Charged : States.Charging;
        }
        else
        {
            state = States.None;
        }

        if (numProjectilesStored > 0)
        {
            ReleaseProjectiles();
        }
        return false;
    }

    public override void DisableShield()
    {
        state = States.None;
        numProjectilesStored = 0;
        shield.ChargeIndicator.StopCharge();
    }

    public override void ReturnShield()
    {
        if (state != States.Returning && shield.gameObject.activeSelf)
        {
            state = States.Returning;
            StartCoroutine(ReturnShieldRoutine());
        }
    }

#region charging
    private void MoveToChargeState()
    {
        if (HasCooldown)
        {
            state = timer > CooldownTime ? States.Charged : States.Charging;
        }
        else
        {
            state = States.Charged;
        }
    }
    
    private void CheckCooldown(float deltaTime)
    {
        if (HasCooldown && !shield.ShieldIsDisabled())
        {
            timer += deltaTime;
            shield.ChargeIndicator.SetCharge(timer / CooldownTime);
            TransitionIfCharged();
        }
        else
        {
            state = States.Charged;
        }
    }

    private void TransitionIfCharged()
    {
        if (timer > CooldownTime)
        {
            shield.ChargeIndicator.SetCharged(true);
            state = States.Charged;
        }
    }
#endregion

    public override void ProjectileImpact(Projectile projectile)
    {
        if (projectile.GetColour() == Colour)
        {
            ManipulateProjectile(projectile);
        }
        else
        {
            if (state == States.Activated && shield.ShieldIsCharged(Colour))
            {
                ReflectProjectile(projectile, false);
            }
            else
            {
                projectile.Destroy();
                shield.DisableShield(shield.DisableTime);
            }
        }
    }

    private void ManipulateProjectile(Projectile projectile)
    {
        if (state == States.Activated)
        {
            numProjectilesStored++;
            projectile.Disable();
        }
        else if (state == States.None || state == States.Charged)
        {
            ReflectProjectile(projectile);
        }
    }

    private void ReflectProjectile(Projectile projectile, bool useMultiplier = true)
    {
        Rigidbody2D projectileBody = projectile.GetComponent<Rigidbody2D>();
        projectileBody.velocity = transform.right * projectileBody.velocity.magnitude;
        projectile.gameObject.layer = TrixieLayers.GetMask(Layers.PlayerProjectile);
        
        if (useMultiplier && ProjectileMultiplier > 1 && shield.ShieldIsCharged(Colour))
        {
            GenerateNewProjectiles(projectile, projectileBody, ProjectileMultiplier, Spread);
        }
    }

    private void ReleaseProjectiles()
    {
        numProjectilesStored = 0;
        timer = 0f;
        state = States.None;
        Debug.Log("TODO IMPLEMENT ME!!!");
    }


    private IEnumerator ReturnShieldRoutine()
    {
        body.isKinematic = true;
        ReleaseProjectiles();

        yield return null;

        numProjectilesStored = 0;
        state = States.None;
        
        timer = 0f;
        shield.OnReturn();
    }

    private void GenerateNewProjectiles(Projectile projectile, Rigidbody2D projectileBody, int numProjectiles, float spread, bool ignoreFirst = true)
    {
        float directionModifier = 1f;
        for (int i = ignoreFirst ? 1 : 0; i < ProjectileMultiplier; i++)
        {
            GameObject newProjectile = ObjectPooler.Instance.GetPooledProjectile(projectile.name.Substring(0, projectile.name.Length - "(Clone)".Length));

            float angle = transform.eulerAngles.z + Spread * directionModifier * Mathf.CeilToInt(i / 2f);
            
            Projectile newProjScript = newProjectile.GetComponent<Projectile>();
            newProjScript.Activate(projectile.transform.position, angle, projectileBody.velocity.magnitude, projectile.GetSettings());

            newProjectile.layer = TrixieLayers.GetMask(Layers.PlayerProjectile);

            directionModifier *= -1f;
        }
    }
}

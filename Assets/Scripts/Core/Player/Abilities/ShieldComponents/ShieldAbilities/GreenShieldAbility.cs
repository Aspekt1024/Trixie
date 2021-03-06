﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrixieCore;

public class GreenShieldAbility : BaseShieldAbility
{
    public int ProjectileMultiplier = 3;
    public float Spread = 10f;
    public bool HasCooldown = true;
    public float CooldownTime = 1.5f;
    public GameObject ProjectilePrefab;
    public float ProjectileSpeed = 22f;
    public bool AbsorbsGreenWhenActive = true;
    public bool CanReflectOtherColours = true;
    public bool NeedsPowerUpToReflect = true;

    private int numProjectilesStored;
    private float timer;

    protected override void Start()
    {
        base.Start();
        Colour = EnergyTypes.Colours.Green;
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
            power.AddPower();
            ManipulateProjectile(projectile);
        }
        else
        {
            if (state == States.Activated && CanReflectOtherColours)
            {
                if (shield.ShieldIsCharged(Colour) && NeedsPowerUpToReflect)
                {
                    ReflectProjectile(projectile, false);
                }
                else if (!NeedsPowerUpToReflect)
                {
                    ReflectProjectile(projectile, false);
                }
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
        if (state == States.Activated && AbsorbsGreenWhenActive)
        {
            numProjectilesStored++;
            projectile.Disable();
        }
        else if (state == States.None || state == States.Charged || state == States.Activated)
        {
            ReflectProjectile(projectile);
        }
    }

    private void ReflectProjectile(Projectile projectile, bool useMultiplier = true)
    {
        Rigidbody2D projectileBody = projectile.GetComponent<Rigidbody2D>();
        Vector2 direction = transform.right * Mathf.Sign(transform.localScale.x);
        projectileBody.velocity = direction * projectileBody.velocity.magnitude;
        projectile.gameObject.layer = TrixieLayers.GetMask(Layers.PlayerProjectile);
        
        if (useMultiplier && ProjectileMultiplier > 1 && shield.ShieldIsCharged(Colour))
        {
            GenerateNewProjectiles(projectile, projectileBody, ProjectileMultiplier, Spread);
        }
    }

    private void ReleaseProjectiles()
    {
        if (numProjectilesStored > 0)
        {
            GameObject firstProjectile = ObjectPooler.Instance.GetPooledProjectile(ProjectilePrefab.name);
            Projectile.ProjectileSettings settings = new Projectile.ProjectileSettings()
            {
                ProjectileColour = EnergyTypes.Colours.Green,
                HasGravity = false,
            };
            Projectile projectile = firstProjectile.GetComponent<Projectile>();
            projectile.Activate(transform.position, transform.eulerAngles.z, ProjectileSpeed, settings, isPlayerProjectile: true);

            if (numProjectilesStored > 1)
            {
                GenerateNewProjectiles(projectile, firstProjectile.GetComponent<Rigidbody2D>(), numProjectilesStored - 1, Spread);
            }
        }

        numProjectilesStored = 0;
        timer = 0f;
        state = States.None;
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

    private void GenerateNewProjectiles(Projectile projectile, Rigidbody2D projectileBody, int numProjectiles, float spread)
    {
        float centerAngle = Mathf.Atan2(projectileBody.velocity.y, projectileBody.velocity.x) * Mathf.Rad2Deg;

        float directionModifier = 1f;
        for (int i = 1; i < numProjectiles; i++)
        {
            GameObject newProjectile = ObjectPooler.Instance.GetPooledProjectile(projectile.name.Substring(0, projectile.name.Length - "(Clone)".Length));

            float angle = centerAngle + Spread * directionModifier * Mathf.CeilToInt(i / 2f);
            
            Projectile newProjScript = newProjectile.GetComponent<Projectile>();
            newProjScript.Activate(projectile.transform.position, angle, projectileBody.velocity.magnitude, projectile.GetSettings(), isPlayerProjectile: true);

            directionModifier *= -1f;
        }
    }
}

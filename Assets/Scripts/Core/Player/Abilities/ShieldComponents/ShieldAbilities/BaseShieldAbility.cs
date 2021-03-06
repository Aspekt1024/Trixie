﻿using UnityEngine;
using TrixieCore;

public abstract class BaseShieldAbility : MonoBehaviour {

    public float MaxCharge = 3;
    public float InitialCharge = 0;
    public bool DisabledOnWrongColour = true;
    public bool LosesPowerOnWrongColour = true;
    public float PowerLossOnWrongColour = 1;
    public bool CancelOnMove = false;

    [HideInInspector] public EnergyTypes.Colours Colour;
   
    protected enum States
    {
        None, Charging, Charged, Activating, Activated, Returning
    }
    protected States state;
    protected bool isShielding;

    protected ShieldAbility shield;
    protected Rigidbody2D body;
    protected Animator anim;
    protected ShieldPower power;
    
    public abstract void ActivatePressed();
    public abstract bool ActivateReleased();
    public abstract void DisableShield();
    public abstract void ReturnShield();
    public abstract void UpdateCharge(float deltaTime);

    public virtual void ProjectileImpact(Projectile projectile)
    {
        if (projectile.GetColour() == Colour)
        {
            power.AddPower(1f);
        }
        else
        { 
            if (DisabledOnWrongColour)
            {
                shield.DisableShield(shield.DisableTime);
            }
            if (LosesPowerOnWrongColour)
            {
                power.ReducePower(PowerLossOnWrongColour);
            }
        }
        projectile.Destroy();
    }

    public virtual void BeginShielding()
    {
        if (state == States.Activated || state == States.Activating)
        {
            ReturnShield();
        }
        else
        {
            isShielding = true;
            shield.ShieldObject.SetActive(true);
            body.isKinematic = true;
            anim.Play("Activate" + Colour.ToString(), 0, 0f);
        }
    }

    public bool IsAtMaxCharge() { return power.ShieldFullyCharged(); }

    public virtual void DisableAbility() { }

    public virtual void StopShielding()
    {
        isShielding = false;
        body.isKinematic = true;
        body.velocity = Vector2.zero;
        shield.ShieldObject.SetActive(false);
    }

    private void Awake()
    {
        shield = GetComponent<ShieldAbility>();
        body = shield.ShieldObject.GetComponent<Rigidbody2D>();
        anim = shield.ShieldObject.GetComponent<Animator>();
    }

    protected virtual void Start()
    {
        power = new ShieldPower(this, MaxCharge, InitialCharge);
    }
}

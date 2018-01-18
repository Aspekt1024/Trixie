using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrixieCore;

public abstract class BaseShieldAbility : MonoBehaviour {

    [HideInInspector] public EnergyTypes.Colours Colour;
    
    protected enum States
    {
        None, Charging, Charged, Activating, Activated, Returning
    }
    protected States state;

    protected ShieldComponent shield;
    protected Rigidbody2D body;
    protected Animator anim;
    
    public abstract void ActivatePressed();
    public abstract bool ActivateReleased();
    public abstract void DisableShield();
    public abstract void ReturnShield();
    public abstract void UpdateCharge(float deltaTime);

    public virtual void ProjectileImpact(Projectile projectile)
    {
        projectile.Destroy();
    }

    private void Awake()
    {
        shield = Player.Instance.GetComponent<ShieldComponent>();
    }

    protected virtual void Start()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }
}

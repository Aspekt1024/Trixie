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
    protected bool isShielding;

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

    public virtual void BeginShielding()
    {
        if (state == States.Activated || state == States.Activating)
        {
            ReturnShield();
        }
        else
        {
            isShielding = true;
            gameObject.SetActive(true);
            body.isKinematic = true;
            anim.Play("Static", 0, 0f);
        }
    }

    public virtual void StopShielding()
    {
        isShielding = false;
        body.isKinematic = true;
        body.velocity = Vector2.zero;
        gameObject.SetActive(false);
    }

    private void Awake()
    {
        shield = Player.Instance.GetComponent<ShieldComponent>();
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }
}

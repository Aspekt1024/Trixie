using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseShieldAbility : MonoBehaviour {

    [HideInInspector] public EnergyTypes.Colours Colour;

    protected ShieldComponent shield;
    protected Rigidbody2D body;
    protected Animator anim;

    public abstract void ActivatePressed();
    public abstract bool ActivateReleased();
    public abstract void DisableShield();
    public abstract void ReturnShield();
    public abstract void UpdateCharge(float deltaTime);

    protected virtual void Start()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        shield = Player.Instance.GetComponent<ShieldComponent>();
    }
}

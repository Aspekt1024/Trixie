using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrixieCore;

[RequireComponent(typeof(BaseShieldAbility))]
public class ShieldComponent : MonoBehaviour {

    public float DisableTime = 3f;
    public GameObject ShieldObject;
    public Collider2D ShieldCollider;
    public Collider2D ProjectileCollider;
    public Transform CenterPoint;
    public ShieldChargeIndicator ChargeIndicator;

    private ShieldPower power;
    private ShieldStats stats;
    private ShieldPositioner positioner;
    private BaseShieldAbility[] abilities;

    private Rigidbody2D body;
    private Animator anim;
    private Animator playerAnim;

    private float shieldDisabledTimer;
    private bool activateButtonHeld;
    private bool shootButtonHeld;
    
    private EnergyTypes.Colours shieldColour;
    private int currentAbilityIndex;

    private enum States
    {
        None, Shielding, Firing, Disabled
    }
    private States state;

    private void Start()
    {
        stats = new ShieldStats();
        power = new ShieldPower();
        positioner = ShieldObject.GetComponent<ShieldPositioner>();
        abilities = ShieldObject.GetComponents<BaseShieldAbility>();
        
        anim = ShieldObject.GetComponent<Animator>();
        body = ShieldObject.GetComponent<Rigidbody2D>();
        ShieldObject.SetActive(false);

        playerAnim = Player.Instance.GetComponent<Animator>();
        
        SetShieldColour(EnergyTypes.Colours.Blue);
        positioner.Setup(CenterPoint);
    }

    private void Update()
    {
        if (abilities != null && abilities.Length > currentAbilityIndex)
        {
            abilities[currentAbilityIndex].UpdateCharge(Time.deltaTime);
        }

        switch (state)
        {
            case States.None:
                break;
            case States.Shielding:
                positioner.SetShieldPosition();
                break;
            case States.Firing:
                break;
            case States.Disabled:
                shieldDisabledTimer -= Time.deltaTime;
                if (shieldDisabledTimer <= 0f)
                {
                    GameUIManager.ShowShieldsEnabled();
                    state = States.None;
                }
                break;
        }
    }
    
    public void ProjectileImpact(Projectile projectile)
    {
        abilities[currentAbilityIndex].ProjectileImpact(projectile);
    }

    public void ReturnShield()
    {
        abilities[currentAbilityIndex].ReturnShield();
    }

    public void OnReturn()
    {
        body.velocity = Vector2.zero;
        if (activateButtonHeld)
        {
            Activate();
        }
        else
        {
            DisableShield();
        }
    }

    public bool HasShield()
    {
        return stats.ShieldUnlocked();
    }

    public void AddShieldPower(int powerToAdd = 1)
    {
        power.AddPower(shieldColour, powerToAdd);
    }

    public void ReduceShieldPower(int powerToRemove = 1)
    {
        power.ReducePower(shieldColour, powerToRemove);
    }

    public void ObtainedUnlock(ItemUnlock.UnlockType unlockType)
    {
        stats.ObtainedUnlock(unlockType);
        if (!stats.ColourUnlocked(shieldColour))
        {
            CycleShieldColourPressed();
        }
        else
        {
            SetShieldColour(shieldColour);
        }
    }

    public void CycleShieldColourPressed()
    {
        if (!stats.ShieldUnlocked() || state == States.Disabled || state == States.Firing || shootButtonHeld) return;

        switch (shieldColour)
        {
            case EnergyTypes.Colours.Blue:
                if (stats.ColourUnlocked(EnergyTypes.Colours.Red))
                {
                    SetShieldColour(EnergyTypes.Colours.Red);
                }
                else
                {
                    shieldColour = EnergyTypes.Colours.Red;
                    CycleShieldColourPressed();
                }
                break;
            case EnergyTypes.Colours.Red:
                if (stats.ColourUnlocked(EnergyTypes.Colours.Green))
                {
                    SetShieldColour(EnergyTypes.Colours.Green);
                }
                else
                {
                    shieldColour = EnergyTypes.Colours.Green;
                    CycleShieldColourPressed();
                }
                break;
            case EnergyTypes.Colours.Green:
                if (stats.ColourUnlocked(EnergyTypes.Colours.Blue))
                {
                    SetShieldColour(EnergyTypes.Colours.Blue);
                }
                else
                {
                    shieldColour = EnergyTypes.Colours.Blue;
                    CycleShieldColourPressed();
                }
                break;
        }
    }
    
    public bool ShieldActivatePressed()
    {
        activateButtonHeld = true;
        if (state == States.Disabled || state == States.Shielding || !stats.ShieldUnlocked()) return false;

        if (state == States.Firing)
        {
            abilities[currentAbilityIndex].ReturnShield();
        }
        else
        {
            Activate();
        }
        return true;
    }

    private void Activate()
    {
        state = States.Shielding;
        body.isKinematic = true;
        ShieldObject.SetActive(true);
        positioner.SetShieldPosition();
        anim.Play("Static", 0, 0f);

        if (shootButtonHeld)
        {
            abilities[currentAbilityIndex].ActivatePressed();
        }
        else
        {
        }
    }

    public bool ShieldDeactivatePressed()
    {
        activateButtonHeld = false;
        if (state == States.Firing) return false;

        DisableShield();
        return true;
    }
    
    public void ShootPressed()
    {
        shootButtonHeld = true;
        //if (!shieldStats.ShootUnlocked()) return;

        if (state == States.Shielding && power.ShieldFullyCharged(shieldColour))
        {
            abilities[currentAbilityIndex].ActivatePressed();
        }
    }

    public void ShootReleased()
    {
        shootButtonHeld = false;
        
        if (state == States.Shielding && power.ShieldFullyCharged(shieldColour))
        {
            bool shootSuccess = abilities[currentAbilityIndex].ActivateReleased();
            if (shootSuccess)
            {
                state = States.Firing;
            }
        }
    }

    public bool IsAwaitingActivation() { return activateButtonHeld; }
    public bool IsShielding() { return state == States.Shielding; }
    public bool IsFiring() { return state == States.Firing; }
    public EnergyTypes.Colours GetColour() { return shieldColour; }
    
    public void DisableShield(float secondsToDisable)
    {
        shieldDisabledTimer = DisableTime;
        GameUIManager.ShowShieldsDisabled();
        DisableShield();
    }

    public void DisableShield()
    {
        if (shieldDisabledTimer <= 0f)
        {
            state = States.None;
        }
        else
        {
            state = States.Disabled;
        }
        body.isKinematic = true;
        body.velocity = Vector2.zero;
        abilities[currentAbilityIndex].DisableShield();
        ShieldObject.SetActive(false);
    }

    private void SetShieldColour(EnergyTypes.Colours colour)
    {
        shieldColour = colour;
        if (stats.ShieldUnlocked())
        {
            GameUIManager.ShowShieldIndicator();
            GameUIManager.SetShieldColour(shieldColour);
        }
        else
        {
            GameUIManager.HideShieldIndicator();
        }

        for (int i = 0; i < abilities.Length; i++)
        {
            if (abilities[i].Colour == shieldColour)
            {
                currentAbilityIndex = i;
                break;
            }
        }

        switch (shieldColour)
        {
            case EnergyTypes.Colours.Blue:
                ShieldObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
                break;
            case EnergyTypes.Colours.Red:
                ShieldObject.GetComponent<SpriteRenderer>().color = new Color(1f, 0f, .7f, 1f);
                break;
            case EnergyTypes.Colours.Green:
                ShieldObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 0f, 1f);
                break;
        }
    }
}

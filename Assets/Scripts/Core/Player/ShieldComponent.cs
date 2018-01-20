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

        GetAbilities();
        
        anim = ShieldObject.GetComponent<Animator>();
        body = ShieldObject.GetComponent<Rigidbody2D>();
        ShieldObject.SetActive(false);

        playerAnim = Player.Instance.GetComponent<Animator>();
        
        SetShieldColour(EnergyTypes.Colours.Blue);
        positioner.Setup(CenterPoint);
    }

    private void GetAbilities()
    {
        abilities = new BaseShieldAbility[3];
        abilities[0] = ShieldObject.GetComponent<BlueShieldAbility>();
        abilities[1] = ShieldObject.GetComponent<RedShieldAbility>();
        abilities[2] = ShieldObject.GetComponent<GreenShieldAbility>();
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
        power.AddPower(abilities[currentAbilityIndex].Colour, powerToAdd);
    }

    public void ReduceShieldPower(int powerToRemove = 1)
    {
        power.ReducePower(abilities[currentAbilityIndex].Colour, powerToRemove);
    }

    public void ObtainedUnlock(ItemUnlock.UnlockType unlockType)
    {
        stats.ObtainedUnlock(unlockType);
        if (!stats.ColourUnlocked(abilities[currentAbilityIndex].Colour))
        {
            CycleShieldColour();
        }
        SetShieldColour(abilities[currentAbilityIndex].Colour);
    }

    public void CycleShieldColour()
    {
        if (!stats.ShieldUnlocked() || state == States.Disabled || state == States.Firing || shootButtonHeld) return;

        int index = currentAbilityIndex;

        index++;
        if (index == abilities.Length) index = 0;
        while (!stats.ColourUnlocked(abilities[index].Colour))
        {
            index++;
            if (index == abilities.Length) index = 0;
            if (currentAbilityIndex == index)
            {
                break;
            }
        }
        currentAbilityIndex = index;
        SetShieldColour(abilities[currentAbilityIndex].Colour);
    }
    
    public bool ShieldActivatePressed()
    {
        activateButtonHeld = true;
        if (state == States.Disabled || state == States.Shielding || !stats.ShieldUnlocked()) return false;
        Activate();

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
        positioner.SetShieldPosition();

        abilities[currentAbilityIndex].BeginShielding();

        if (shootButtonHeld)
        {
            abilities[currentAbilityIndex].ActivatePressed();
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

        if (state == States.Shielding)
        {
            abilities[currentAbilityIndex].ActivatePressed();
        }
    }

    public void ShootReleased()
    {
        shootButtonHeld = false;
        
        if (state == States.Shielding)
        {
            bool shootSuccess = abilities[currentAbilityIndex].ActivateReleased();
            if (shootSuccess)
            {
                state = States.Firing;
            }
        }
    }

    public bool ShieldIsDisabled() { return state == States.Disabled; }
    public bool ShieldIsCharged(EnergyTypes.Colours colour) { return power.ShieldFullyCharged(colour); }
    public bool IsAwaitingActivation() { return activateButtonHeld; }
    public bool IsShielding() { return state == States.Shielding; }
    public bool IsFiring() { return state == States.Firing; }
    public EnergyTypes.Colours GetColour() { return abilities[currentAbilityIndex].Colour; }
    
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
        abilities[currentAbilityIndex].DisableShield();
        abilities[currentAbilityIndex].StopShielding();
    }

    private void SetShieldColour(EnergyTypes.Colours colour)
    {
        if (stats.ShieldUnlocked())
        {
            GameUIManager.ShowShieldIndicator();
            GameUIManager.SetShieldColour(abilities[currentAbilityIndex].Colour);
        }
        else
        {
            GameUIManager.HideShieldIndicator();
        }

        switch (abilities[currentAbilityIndex].Colour)
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

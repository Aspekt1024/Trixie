using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldComponent : MonoBehaviour {

    public GameObject ShieldObject;
    public Transform ShieldCenterPoint;

    private ShieldPower power;
    private ShieldStats stats;
    private ShieldShoot shooter;
    private ShieldPositioner positioner;
    
    private Rigidbody2D body;
    private Animator anim;
    private Animator playerAnim;

    private float shieldDisabledTimer;
    private bool activateButtonHeld;
    private bool shootButtonHeld;
    
    private EnergyTypes.Colours shieldColour;

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
        shooter = ShieldObject.GetComponent<ShieldShoot>();
        
        anim = ShieldObject.GetComponent<Animator>();
        body = ShieldObject.GetComponent<Rigidbody2D>();
        ShieldObject.SetActive(false);

        playerAnim = Player.Instance.GetComponent<Animator>();
        
        SetShieldColour(EnergyTypes.Colours.Blue);
        positioner.Setup(ShieldCenterPoint);
    }

    private void Update()
    {
        shooter.UpdateCharge(Time.deltaTime);

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

    public void OnReturn()
    {
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
        if (!stats.ShieldUnlocked() || state == States.Disabled) return;

        switch (shieldColour)
        {
            case EnergyTypes.Colours.Blue:
                if (stats.ColourUnlocked(EnergyTypes.Colours.Pink))
                {
                    SetShieldColour(EnergyTypes.Colours.Pink);
                }
                else
                {
                    shieldColour = EnergyTypes.Colours.Pink;
                    CycleShieldColourPressed();
                }
                break;
            case EnergyTypes.Colours.Pink:
                if (stats.ColourUnlocked(EnergyTypes.Colours.Yellow))
                {
                    SetShieldColour(EnergyTypes.Colours.Yellow);
                }
                else
                {
                    shieldColour = EnergyTypes.Colours.Yellow;
                    CycleShieldColourPressed();
                }
                break;
            case EnergyTypes.Colours.Yellow:
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
            shooter.ReturnShield();
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
            shooter.Arm();
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
            shooter.Arm();
        }
    }

    public void ShootReleased()
    {
        shootButtonHeld = false;
        
        if (state == States.Shielding && power.ShieldFullyCharged(shieldColour))
        {
            bool shootSuccess = shooter.Shoot();
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
        shieldDisabledTimer = secondsToDisable;
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
        shooter.DisableShield();
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

        switch (shieldColour)
        {
            case EnergyTypes.Colours.Blue:
                ShieldObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
                break;
            case EnergyTypes.Colours.Pink:
                ShieldObject.GetComponent<SpriteRenderer>().color = new Color(1f, 0f, .7f, 1f);
                break;
            case EnergyTypes.Colours.Yellow:
                ShieldObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 0f, 1f);
                break;
        }
    }
    
}

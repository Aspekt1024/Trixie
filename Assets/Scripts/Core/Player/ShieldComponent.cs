using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldComponent : MonoBehaviour {

    public GameObject ShieldObject;
    public Transform ShieldCenterPoint;
    
    private ShieldPositioner positioner;
    private ShieldStats shieldStats;
    public ShieldPower shieldPower;

    private Rigidbody2D body;
    private Animator anim;
    private Animator playerAnim;

    private int shieldCharges;
    private float rechargeTimer;

    private const float shootSpeed = 34f;
    private const float shieldShootDistance = 10f;
    private float shieldDistance;

    private const int maxShieldCharges = 3;
    private const float shieldRechargeTime = 3f;
    
    private EnergyTypes.Colours shieldColour;

    private enum States
    {
        None, Shielding, Firing,
    }
    private States state;

    // TODO remove this when anim is set
    Vector3 maxSize;

    private void Start()
    {
        shieldStats = new ShieldStats();
        shieldPower = new ShieldPower();

        anim = ShieldObject.GetComponent<Animator>();
        body = ShieldObject.GetComponent<Rigidbody2D>();
        positioner = ShieldObject.GetComponent<ShieldPositioner>();
        ShieldObject.SetActive(false);

        playerAnim = Player.Instance.GetComponent<Animator>();

        shieldCharges = maxShieldCharges;
        SetShieldColour(EnergyTypes.Colours.Blue);
        positioner.Setup(ShieldCenterPoint);

        maxSize = ShieldObject.transform.localScale;
    }

    private void Update()
    {
        switch (state)
        {
            case States.None:
                break;
            case States.Shielding:
                positioner.SetShieldPosition();
                break;
            case States.Firing:
                shieldDistance += Time.deltaTime * shootSpeed;
                if (shieldDistance >= shieldShootDistance)
                {
                    state = States.None;
                }
                break;
        }
        
        if (shieldCharges < maxShieldCharges)
        {
            rechargeTimer += Time.deltaTime;
            if (rechargeTimer >= shieldRechargeTime)
            {
                rechargeTimer = 0f;
                AddCharge();
            }
        }
        else
        {
            rechargeTimer = 0f;
        }
    }

    public bool HasShield()
    {
        return shieldStats.ShieldUnlocked();
    }

    public void AddShieldPower(int powerToAdd = 1)
    {
        shieldPower.AddPower(shieldColour, powerToAdd);
    }

    public void ReduceShieldPower(int powerToRemove = 1)
    {
        shieldPower.ReducePower(shieldColour, powerToRemove);
    }

    public void ObtainedUnlock(ItemUnlock.UnlockType unlockType)
    {
        shieldStats.ObtainedUnlock(unlockType);
        if (!shieldStats.ColourUnlocked(shieldColour))
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
        if (!shieldStats.ShieldUnlocked()) return;

        switch (shieldColour)
        {
            case EnergyTypes.Colours.Blue:
                if (shieldStats.ColourUnlocked(EnergyTypes.Colours.Pink))
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
                if (shieldStats.ColourUnlocked(EnergyTypes.Colours.Yellow))
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
                if (shieldStats.ColourUnlocked(EnergyTypes.Colours.Blue))
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
    
    public bool ActivateShield()
    {
        if (state != States.None || shieldCharges == 0 || !shieldStats.ShieldUnlocked()) return false;

        state = States.Shielding;
        body.isKinematic = true;
        ShieldObject.SetActive(true);
        positioner.SetShieldPosition();
        anim.Play("Static", 0, 0f);
        return true;
    }

    public bool DeactivateShield()
    {
        if (state != States.Shielding) return true;

        DisableShield();
        return true;
    }
    
    public void Shoot()
    {
        if (state != States.Shielding) return;
        //if (!shieldStats.ShootUnlocked()) return;
        if (!shieldPower.ShieldFullyCharged(shieldColour)) return;
        
        state = States.Firing;
        shieldDistance = 0f;
        body.isKinematic = false;
        body.velocity = shootSpeed * body.transform.right;
        anim.Play("Shoot", 0, 0f);
    }

    public void RemoveCharge(int chargesToRemove = 1)
    {
        shieldCharges = Mathf.Max(0, shieldCharges - chargesToRemove);
        UpdateShieldSize();
        if (state == States.Shielding && shieldCharges == 0)
        {
            DisableShield();
        }
    }

    public bool IsShielding() { return state == States.Shielding; }
    public bool IsFiring() { return state == States.Firing; }
    public EnergyTypes.Colours GetColour() { return shieldColour; }

    private void AddCharge(int chargesToAdd = 1)
    {
        shieldCharges = Mathf.Min(maxShieldCharges, shieldCharges + chargesToAdd);
        UpdateShieldSize();
    }

    public void DisableShield()
    {
        state = States.None;
        body.isKinematic = true;
        body.velocity = Vector2.zero;
        ShieldObject.SetActive(false);
    }

    private void UpdateShieldSize()
    {
        if (shieldCharges == 0) return;
        ShieldObject.transform.localScale = maxSize * (3 + shieldCharges) / 6f;
    }

    private void SetShieldColour(EnergyTypes.Colours colour)
    {
        shieldColour = colour;
        if (shieldStats.ShieldUnlocked())
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldComponent : MonoBehaviour {

    public GameObject ShieldObject;
    public Transform ShieldCenterPoint;
    public ShieldPower shieldPower;
    public float ShootChargeTime = 1.5f;

    private ShieldPositioner positioner;
    private ShieldStats shieldStats;

    private Collider2D shieldCollider;
    private Rigidbody2D body;
    private Animator anim;
    private Animator playerAnim;
    
    private const float shootSpeed = 75f;
    private const float shieldShootDistance = 30f;
    private float shieldDistance;
    private float timeCharged;

    private float shieldDisabledTimer;
    private bool activateWhenAvailable;
    private bool chargeWhenAvailable;
    
    private EnergyTypes.Colours shieldColour;

    private enum States
    {
        None, Shielding, Firing, Disabled, Returning, ChargingShoot, ShootCharged
    }
    private States state;

    private void Start()
    {
        shieldStats = new ShieldStats();
        shieldPower = new ShieldPower();

        shieldCollider = ShieldObject.GetComponent<Collider2D>();
        anim = ShieldObject.GetComponent<Animator>();
        body = ShieldObject.GetComponent<Rigidbody2D>();
        positioner = ShieldObject.GetComponent<ShieldPositioner>();
        ShieldObject.SetActive(false);

        playerAnim = Player.Instance.GetComponent<Animator>();
        
        SetShieldColour(EnergyTypes.Colours.Blue);
        positioner.Setup(ShieldCenterPoint);
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
                    ReturnShield();
                }
                break;
            case States.Returning:
                break;
            case States.Disabled:
                shieldDisabledTimer -= Time.deltaTime;
                if (shieldDisabledTimer <= 0f)
                {
                    GameUIManager.ShowShieldsEnabled();
                    state = States.None;
                }
                break;
            case States.ChargingShoot:
                timeCharged += Time.deltaTime;
                if (timeCharged >= ShootChargeTime)
                {
                    state = States.ShootCharged;
                }
                positioner.SetShieldPosition();
                break;
            case States.ShootCharged:
                positioner.SetShieldPosition();
                break;
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
    
    public bool ShieldActivatePressed()
    {
        activateWhenAvailable = true;
        if (state == States.Disabled || state == States.Shielding || !shieldStats.ShieldUnlocked()) return false;
        if (state == States.Returning) return true;

        if (state == States.Firing)
        {
            ReturnShield();
        }
        else
        {
            Activate();
        }
        return true;
    }

    private void Activate()
    {
        body.isKinematic = true;
        ShieldObject.SetActive(true);
        positioner.SetShieldPosition();
        anim.Play("Static", 0, 0f);

        if (chargeWhenAvailable)
        {
            StartCharging();
        }
        else
        {
            state = States.Shielding;
        }
    }

    public bool ShieldDeactivatePressed()
    {
        activateWhenAvailable = false;
        if (state == States.Firing || state == States.Returning) return false;

        DisableShield();
        return true;
    }

    public void ReturnShield()
    {
        StartCoroutine(ReturnShieldRoutine());
    }
    
    public void ShootPressed()
    {
        chargeWhenAvailable = true;
        if (state != States.Shielding) return;
        StartCharging();
    }

    private void StartCharging()
    {
        //if (!shieldStats.ShootUnlocked()) return;
        if (!shieldPower.ShieldFullyCharged(shieldColour)) return;
        timeCharged = 0f;
        state = States.ChargingShoot;
    }

    public void ShootReleased()
    {
        timeCharged = 0f;
        chargeWhenAvailable = false;
        if (state != States.ShootCharged) return;

        shieldCollider.isTrigger = true;
        state = States.Firing;
        shieldDistance = 0f;
        body.isKinematic = false;
        body.velocity = shootSpeed * body.transform.right;
        anim.Play("Shoot", 0, 0f);
    }
    
    public bool IsAwaitingActivation() { return activateWhenAvailable; }
    public bool IsShielding() { return state == States.Shielding || state == States.ChargingShoot || state == States.ShootCharged; }
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
        state = States.Disabled;
        body.isKinematic = true;
        shieldCollider.isTrigger = false;
        body.velocity = Vector2.zero;
        ShieldObject.SetActive(false);
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
    
    private IEnumerator ReturnShieldRoutine()
    {
        state = States.Returning;
        body.isKinematic = true;

        while (Vector2.Distance(ShieldObject.transform.position, ShieldCenterPoint.position) > 1f)
        {
            Vector2 distVector = (ShieldCenterPoint.position - ShieldObject.transform.position).normalized;
            ShieldObject.transform.position += new Vector3(distVector.x, distVector.y, 0f) * shootSpeed * Time.deltaTime;

            // Ensure we don't overshoot the target position
            Vector2 newDistVector = ShieldCenterPoint.position - ShieldObject.transform.position;
            if (Mathf.Sign(newDistVector.x) != Mathf.Sign(distVector.x) || Mathf.Sign(newDistVector.y) != Mathf.Sign(distVector.y))
            {
                ShieldObject.transform.position = new Vector3(ShieldCenterPoint.position.x, ShieldCenterPoint.position.y, ShieldObject.transform.position.z);
            }
            yield return null;
        }

        if (activateWhenAvailable)
        {
            Activate();
        }
        else
        {
            DisableShield();
        }
    }
}

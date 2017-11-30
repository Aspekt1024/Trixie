using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldComponent : MonoBehaviour {

    public GameObject ShieldObject;
    public Transform ShieldCenterPoint;
    
    private ShieldPositioner positioner;

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

    public void CycleShieldColourPressed()
    {
        switch (shieldColour)
        {
            case EnergyTypes.Colours.Blue:
                SetShieldColour(EnergyTypes.Colours.Pink);
                break;
            case EnergyTypes.Colours.Pink:
                SetShieldColour(EnergyTypes.Colours.Yellow);
                break;
            case EnergyTypes.Colours.Yellow:
                SetShieldColour(EnergyTypes.Colours.Blue);
                break;
        }
    }
    
    public bool ActivateShield()
    {
        if (state != States.None || shieldCharges == 0) return false;

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
        if (state == States.Shielding)
        {
            state = States.Firing;
            shieldDistance = 0f;
            body.isKinematic = false;
            body.velocity = shootSpeed * body.transform.right;
            anim.Play("Shoot", 0, 0f);
        }
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
        GameUIManager.SetShieldColour(shieldColour);

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

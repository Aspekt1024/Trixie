using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldComponent : MonoBehaviour {

    public GameObject ShieldObject;
    public Transform CenterPoint;

    private Shield shield;
    private Rigidbody2D body;
    private Animator anim;

    private int shieldCharges;
    private float rechargeTimer;

    private const float shootSpeed = 34f;
    private const float shieldShootDistance = 10f;
    private float shieldDistance;

    private const int maxShieldCharges = 3;
    private const float shieldRechargeTime = 3f;
    private const float distFromCenter = 2f;

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
        shield = ShieldObject.GetComponent<Shield>();
        ShieldObject.SetActive(false);
        
        shieldCharges = maxShieldCharges;

        maxSize = ShieldObject.transform.localScale;
    }

    private void Update()
    {
        switch (state)
        {
            case States.None:
                break;
            case States.Shielding:
                SetShieldPosition();
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
    
    public void ShieldPressed()
    {
        if (state != States.None || shieldCharges == 0) return;

        state = States.Shielding;
        body.isKinematic = true;
        ShieldObject.SetActive(true);
        SetShieldPosition();
    }

    public void ShieldReleased()
    {
        if (state != States.Shielding) return;

        DisableShield();
    }
    
    public void Shoot()
    {
        if (state == States.Shielding)
        {
            state = States.Firing;
            shieldDistance = 0f;
            body.isKinematic = false;
            body.velocity = shootSpeed * body.transform.right;
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
        ShieldObject.transform.localScale = maxSize * shieldCharges / 3;
    }

    private void SetShieldPosition()
    {
        Vector2 mPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 distVector = (mPos - (Vector2)CenterPoint.position).normalized * distFromCenter;
        ShieldObject.transform.position = CenterPoint.position + (Vector3)distVector;
        float angle = Mathf.Rad2Deg * Mathf.Atan2(distVector.y, distVector.x);
        ShieldObject.transform.localEulerAngles = new Vector3(0f, 0f, angle);
    }


}

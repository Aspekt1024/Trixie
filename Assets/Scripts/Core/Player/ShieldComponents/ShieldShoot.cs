using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrixieCore;

public class ShieldShoot : MonoBehaviour
{
    public float ShootSpeed = 75f;
    public float ShootDistance = 30f;
    public enum ChargeTypes
    {
        Chargeup, Cooldown
    }
    public ChargeTypes ChargeType;
    public float ChargeupTime = 1f;
    public float CooldownTime = 1f;

    public Transform CenterPoint;
    public ShieldChargeIndicator ChargeIndicator;
    public Collider2D ProjectileCollider;

    private ShieldComponent shield;
    private ShieldTrajectory trajectory;
    private Rigidbody2D body;
    private Collider2D coll;
    private Animator anim;

    private float timer;

    private enum States
    {
        None, Charging, Charged, Shooting, Returning
    }
    private States state;

    private void Start()
    {
        shield = GetComponentInParent<ShieldComponent>();
        trajectory = GetComponentInChildren<ShieldTrajectory>();

        body = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();

        ProjectileCollider.enabled = false;
    }

    public void UpdateCharge(float deltaTime)
    {
        switch (state)
        {
            case States.None:
                if (ChargeType == ChargeTypes.Cooldown)
                {
                    if (timer > CooldownTime)
                    {
                        state = States.Charged;
                    }
                    else
                    {
                        state = States.Charging;
                    }
                }
                else
                {
                    timer = 0f;
                }
                break;
            case States.Charging:
                timer += deltaTime;
                ChargeIndicator.SetCharge(timer / (ChargeType == ChargeTypes.Chargeup ? ChargeupTime : CooldownTime));
                TransitionIfCharged();
                break;
            case States.Charged:
                break;
            case States.Shooting:
                float shieldDistance = (transform.position - CenterPoint.position).magnitude;
                if (shieldDistance >= ShootDistance)
                {
                    ReturnShield();
                }
                break;
            case States.Returning:
                break;
            default:
                break;
        }
    }

    public void Arm()
    {
        trajectory.Enable();

        if (ChargeType == ChargeTypes.Chargeup)
        {
            state = States.Charging;
        }
    }

    public bool Shoot()
    {
        ChargeIndicator.StopCharge();
        trajectory.Disable();
        bool shootSuccess = false;

        if (state == States.Charged && shield.IsShielding())
        {
            shootSuccess = true;
            ActivateShoot();
        }
        
        if (ChargeType == ChargeTypes.Chargeup)
        {
            timer = 0f;
            if (state == States.Charging)
            {
                state = States.None;
            }
        }

        return shootSuccess;
    }

    public void ReturnShield()
    {
        if (state != States.Returning)
        {
            state = States.Returning;
            StartCoroutine(ReturnShieldRoutine());
        }
    }

    public void DisableShield()
    {
        if (state == States.Shooting)
        {
            timer = 0f;
        }
        state = States.None;
        trajectory.Disable();
        ChargeIndicator.StopCharge();
    }

    private void ActivateShoot()
    {
        state = States.Shooting;

        ProjectileCollider.enabled = true;
        coll.isTrigger = true;
        body.isKinematic = false;
        body.velocity = ShootSpeed * body.transform.right;
        anim.Play("Shoot", 0, 0f);
    }
    
    private IEnumerator ReturnShieldRoutine()
    {
        ProjectileCollider.enabled = false;
        body.isKinematic = true;

        while (Vector2.Distance(transform.position, CenterPoint.position) > 1f)
        {
            Vector2 distVector = (CenterPoint.position - transform.position).normalized;
            transform.position += new Vector3(distVector.x, distVector.y, 0f) * ShootSpeed * Time.deltaTime;

            // Ensure we don't overshoot the target position
            Vector2 newDistVector = CenterPoint.position - transform.position;
            if (Mathf.Sign(newDistVector.x) != Mathf.Sign(distVector.x) || Mathf.Sign(newDistVector.y) != Mathf.Sign(distVector.y))
            {
                transform.position = new Vector3(CenterPoint.position.x, CenterPoint.position.y, transform.position.z);
            }
            yield return null;
        }

        coll.isTrigger = false;
        state = States.None;

        if (ChargeType == ChargeTypes.Cooldown)
        {
            timer = 0f;
        }
        shield.OnReturn();
    }

    private void TransitionIfCharged()
    {
        if (ChargeType == ChargeTypes.Chargeup && timer > ChargeupTime)
        {
            ChargeIndicator.SetCharged(ChargeType == ChargeTypes.Cooldown);
            state = States.Charged;
        }
        else if (ChargeType == ChargeTypes.Cooldown && timer > CooldownTime)
        {
            ChargeIndicator.SetCharged(ChargeType == ChargeTypes.Cooldown);
            state = States.Charged;
        }
    }
}

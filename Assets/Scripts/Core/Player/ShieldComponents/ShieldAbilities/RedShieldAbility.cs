using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedShieldAbility : BaseShieldAbility
{
    public bool ShootRequiresFullCharge = true;
    public float ShootSpeed = 75f;
    public float ShootDistance = 30f;
    public float Damage = 2f;
    public enum ChargeTypes
    {
        Chargeup, Cooldown
    }
    public ChargeTypes ChargeType;
    public float ChargeupTime = 1f;
    public float CooldownTime = 1f;

    private float timer;

    protected override void Awake()
    {
        base.Awake();
        Colour = EnergyTypes.Colours.Red;
        shield.ProjectileCollider.enabled = false;
    }

    public override void ActivatePressed()
    {
        if (ShootRequiresFullCharge && !power.ShieldFullyCharged()) return;

        if (ChargeType == ChargeTypes.Chargeup)
        {
            state = States.Charging;
        }
    }

    public override bool ActivateReleased()
    {
        shield.ChargeIndicator.StopCharge();
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
    
    public override void ReturnShield()
    {
        if (state != States.Returning && shield.gameObject.activeSelf)
        {
            state = States.Returning;
            StartCoroutine(ReturnShieldRoutine());
        }
    }

    public override void DisableShield()
    {
        if (state == States.Activating)
        {
            timer = 0f;
        }
        state = States.None;
        shield.ProjectileCollider.enabled = false;
        shield.ChargeIndicator.StopCharge();
    }

    public override void UpdateCharge(float deltaTime)
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
                shield.ChargeIndicator.SetCharge(timer / (ChargeType == ChargeTypes.Chargeup ? ChargeupTime : CooldownTime));
                TransitionIfCharged();
                break;
            case States.Charged:
                break;
            case States.Activating:
                float shieldDistance = (transform.position - shield.CenterPoint.position).magnitude;
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
    
    private void ActivateShoot()
    {
        state = States.Activating;

        shield.ProjectileCollider.enabled = true;
        shield.ShieldCollider.isTrigger = true;
        body.isKinematic = false;
        body.velocity = ShootSpeed * GetMoveDirection().normalized;
        anim.Play("Shoot", 0, 0f);
    }

    private IEnumerator ReturnShieldRoutine()
    {
        shield.ProjectileCollider.enabled = false;
        body.isKinematic = true;

        while (Vector2.Distance(transform.position, shield.CenterPoint.position) > 1f)
        {
            Vector2 distVector = (shield.CenterPoint.position - transform.position).normalized;
            transform.position += new Vector3(distVector.x, distVector.y, 0f) * ShootSpeed * Time.deltaTime;

            // Ensure we don't overshoot the target position
            Vector2 newDistVector = shield.CenterPoint.position - transform.position;
            if (Mathf.Sign(newDistVector.x) != Mathf.Sign(distVector.x) || Mathf.Sign(newDistVector.y) != Mathf.Sign(distVector.y))
            {
                transform.position = new Vector3(shield.CenterPoint.position.x, shield.CenterPoint.position.y, transform.position.z);
            }
            yield return null;
        }

        shield.ShieldCollider.isTrigger = false;
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
            shield.ChargeIndicator.SetCharged(ChargeType == ChargeTypes.Cooldown);
            state = States.Charged;
        }
        else if (ChargeType == ChargeTypes.Cooldown && timer > CooldownTime)
        {
            shield.ChargeIndicator.SetCharged(ChargeType == ChargeTypes.Cooldown);
            state = States.Charged;
        }
    }

    private Vector2 GetMoveDirection()
    {
        Vector2 dir = GameManager.GetMoveDirection();
        if (dir.y > Mathf.Abs(dir.x))
        {
            return Vector2.up;
        }
        else if (dir.y < -Mathf.Abs(dir.x))
        {
            return Vector2.down;
        }
        else if (Player.Instance.IsLookingRight())
        {
            return Vector2.right;
        }
        else
        {
            return Vector2.left;
        }
    }
}

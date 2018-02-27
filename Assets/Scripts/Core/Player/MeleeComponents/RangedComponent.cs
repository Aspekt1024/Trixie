using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedComponent : MonoBehaviour {

    public float PowerupTime = 0.6f;
    public float RangedCooldown = 0.1f;

    public GameObject ProjectilePrefab;
    
    private ShieldComponent shieldComponent;
    private ChargeIndicator indicator;

    private bool isChargingUp;
    private float chargingTime;

    private void Start()
    {
        shieldComponent = GetComponent<ShieldComponent>();
        indicator = GetComponentInChildren<ChargeIndicator>();
        indicator.SetChargeState(ChargeIndicator.States.None);
    }

    private void Update()
    {
        if (isChargingUp)
        {
            chargingTime += Time.deltaTime;
            if (chargingTime > PowerupTime)
            {
                indicator.SetChargeState(ChargeIndicator.States.StageOne);
            }
        }
    }

    public void RangedPressed()
    {
        isChargingUp = true;
        chargingTime = 0f;
        indicator.SetChargeState(ChargeIndicator.States.Charging);
        switch (shieldComponent.GetColour())
        {
            case EnergyTypes.Colours.Blue:
                AttackBlue();
                break;
            case EnergyTypes.Colours.Red:
                AttackRed();
                break;
            case EnergyTypes.Colours.Green:
                AttackGreen();
                break;
            default:
                break;
        }
    }

    public void RangedReleased()
    {
        if (chargingTime > PowerupTime)
        {
            PowerAttackRed();
        }
        isChargingUp = false;
        indicator.SetChargeState(ChargeIndicator.States.None);
    }

    private void AttackBlue()
    {

    }

    private void AttackRed()
    {
        float projectileSpeed = 26f;
        GameObject proj = Instantiate(ProjectilePrefab);
        proj.GetComponent<Rigidbody2D>().velocity = GetMoveDirection().normalized * projectileSpeed;
        proj.transform.position = (Vector2)transform.position + proj.GetComponent<Rigidbody2D>().velocity.normalized * 1f;
    }

    private void AttackGreen()
    {

    }

    private void PowerAttackRed()
    {
        float projectileSpeed = 26f;
        float arc = 360f;
        float spread = 45f;
        float angle = 0f;
        while (angle < arc)
        {
            GameObject proj = Instantiate(ProjectilePrefab);
            proj.transform.localEulerAngles = new Vector3(0f, 0f, angle);
            proj.GetComponent<Rigidbody2D>().velocity = proj.transform.right * projectileSpeed;
            proj.transform.position = (Vector2)transform.position + proj.GetComponent<Rigidbody2D>().velocity.normalized * 1f;
            angle += spread;
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

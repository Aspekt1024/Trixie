using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedComponent : MonoBehaviour {

    public float PowerupTime = 0.6f;
    public float RangedCooldown = 0.1f;

    public GameObject ProjectilePrefab;
    
    private ShieldComponent shieldComponent;

    private void Start()
    {
        shieldComponent = GetComponent<ShieldComponent>();
    }

    public void RangedPressed()
    {
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

    }

    private void AttackBlue()
    {

    }

    private void AttackRed()
    {
        float projectileSpeed = 16f;
        GameObject proj = Instantiate(ProjectilePrefab);
        proj.GetComponent<Rigidbody2D>().velocity = GetMoveDirection().normalized * projectileSpeed;
        proj.transform.position = (Vector2)transform.position + proj.GetComponent<Rigidbody2D>().velocity.normalized * 1f;
    }

    private void AttackGreen()
    {

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

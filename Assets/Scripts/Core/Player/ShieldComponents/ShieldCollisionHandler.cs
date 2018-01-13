using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldCollisionHandler : MonoBehaviour {

    [SerializeField]
    private PolygonCollider2D[] shieldColliders;
    private int currentColliderIndex;

    private ShieldComponent shieldComponent;
    
    private void Start ()
    {
        shieldComponent = Player.Instance.GetComponent<ShieldComponent>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        BlockProjectiles(collision.collider.gameObject);
        HitEnemies(collision.collider.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        BlockProjectiles(collision.gameObject);
        HitEnemies(collision.gameObject);
    }

    private void BlockProjectiles(GameObject other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Projectile"))
        {
            Projectile projectile = other.gameObject.GetComponent<Projectile>();
            if (projectile.GetColour() != shieldComponent.GetColour())
            {
                if (shieldComponent.IsShielding())
                {
                    shieldComponent.RemoveCharge();
                }
                else if (shieldComponent.IsFiring())
                {
                    shieldComponent.DisableShield();
                }
            }
            else
            {
                if (!shieldComponent.IsFiring())
                {
                    shieldComponent.AddShieldPower();
                }
            }
        }
    }

    private void HitEnemies(GameObject other)
    {
        if (other.tag == "Enemy")
        {
            BaseEnemy baseEnemy = other.GetComponent<BaseEnemy>();
            if (baseEnemy)
            {
                if (shieldComponent.IsFiring())
                {
                    baseEnemy.DamageEnemy(Vector2.zero);
                }
                shieldComponent.DisableShield();
            }
        }
    }

    
}

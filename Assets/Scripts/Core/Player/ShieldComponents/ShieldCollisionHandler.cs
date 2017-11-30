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
        DestroyEnemies(collision.collider.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        BlockProjectiles(collision.gameObject);
        DestroyEnemies(collision.gameObject);
    }

    private void BlockProjectiles(GameObject other)
    {
        if (other.tag == "Projectile")
        {
            Projectile projectile = other.gameObject.GetComponent<Projectile>();
            if (projectile.ProjectileColour != shieldComponent.GetColour())
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
            other.GetComponent<Projectile>().DestroyByCollision();
        }
    }

    private void DestroyEnemies(GameObject other)
    {
        if (other.tag == "Enemy")
        {
            BaseEnemy baseEnemy = other.GetComponent<BaseEnemy>();
            if (baseEnemy)
            {
                other.GetComponent<BaseEnemy>().DamageEnemy();
                shieldComponent.DisableShield();
            }
        }
    }

    
}

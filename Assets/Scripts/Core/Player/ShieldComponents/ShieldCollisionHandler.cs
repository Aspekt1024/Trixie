using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrixieCore;

public class ShieldCollisionHandler : MonoBehaviour {
    
    private ShieldComponent shieldComponent;
    
    private void Start ()
    {
        shieldComponent = Player.Instance.GetComponent<ShieldComponent>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        BlockProjectiles(collision.collider.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        BlockProjectiles(collision.gameObject);
    }

    private void BlockProjectiles(GameObject other)
    {
        if (other.gameObject.layer == TrixieLayers.GetMask(Layers.Projectile))
        {
            Projectile projectile = other.gameObject.GetComponent<Projectile>();
            if (projectile.GetColour() != shieldComponent.GetColour())
            {
                if (shieldComponent.IsShielding())
                {
                    shieldComponent.DisableShield(3f);
                }
            }
            else
            {
                shieldComponent.AddShieldPower();
            }
        }
    }
}

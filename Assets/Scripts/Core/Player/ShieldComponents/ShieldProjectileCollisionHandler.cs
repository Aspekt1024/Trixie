using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrixieCore;

public class ShieldProjectileCollisionHandler : MonoBehaviour {

    public ShieldShoot ShieldShoot;
    public ShieldComponent ShieldComponent;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == TrixieLayers.GetMask(Layers.Terrain))
        {
            ShieldShoot.ReturnShield();
        }
        else if (collision.gameObject.layer == TrixieLayers.GetMask(Layers.Enemy))
        {
            BaseEnemy baseEnemy = collision.GetComponent<BaseEnemy>();
            if (baseEnemy)
            {
                baseEnemy.DamageEnemy(Vector2.zero);
                ShieldShoot.ReturnShield();
            }
        }
        else if (collision.gameObject.layer == TrixieLayers.GetMask(Layers.Projectile))
        {
            ShieldComponent.DisableShield();
        }
    }
}

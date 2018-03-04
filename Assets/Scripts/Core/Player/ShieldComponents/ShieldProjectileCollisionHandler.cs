using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrixieCore;

public class ShieldProjectileCollisionHandler : MonoBehaviour {
    
    public ShieldComponent ShieldComponent;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == TrixieLayers.GetMask(Layers.Terrain))
        {
            ShieldComponent.ReturnShield();
        }
        else if (collision.gameObject.layer == TrixieLayers.GetMask(Layers.Enemy))
        {
            BaseEnemy baseEnemy = collision.GetComponent<BaseEnemy>();
            if (baseEnemy)
            {
                baseEnemy.DamageEnemy(Vector2.zero, EnergyTypes.Colours.Red, (int)((RedShieldAbility)ShieldComponent.GetAbility(EnergyTypes.Colours.Red)).Damage);
                ShieldComponent.ReturnShield();
            }
        }
        else if (collision.gameObject.layer == TrixieLayers.GetMask(Layers.Projectile))
        {
            ShieldComponent.DisableShield();
        }
    }
}

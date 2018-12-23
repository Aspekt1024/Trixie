using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrixieCore;
using TrixieCore.Units;

public class ShieldProjectileCollisionHandler : MonoBehaviour {
    
    public ShieldAbility Shield;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == TrixieLayers.GetMask(Layers.Terrain))
        {
            Shield.ReturnShield();
        }
        else if (collision.gameObject.layer == TrixieLayers.GetMask(Layers.Enemy))
        {
            BaseEnemy baseEnemy = collision.GetComponent<BaseEnemy>();
            if (baseEnemy)
            {
                //baseEnemy.DamageEnemy(Vector2.zero, EnergyTypes.Colours.Red, (int)((RedShieldAbility)ShieldComponent.GetAbility(EnergyTypes.Colours.Red)).Damage);
                Shield.ReturnShield();
            }
        }
        else if (collision.gameObject.layer == TrixieLayers.GetMask(Layers.Projectile))
        {
            Shield.DisableShield();
        }
    }
}

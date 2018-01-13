using ReGoap.Unity;
using UnityEngine;
using TrixieCore.Goap;
using TrixieCore;

public class EnemyProjectileSensor : ReGoapSensor<GoapLabels, object>
{
    private Projectile lastProjectile;
    private EnergyTypes.Colours weakColour;
    private AttackAction attackAction;
    
    private void Start()
    {
        memory = GetComponent<GoapTestMem>();
    }
    
    private void OnEnable()
    {
        attackAction = GetComponent<AttackAction>();
        attackAction.OnShoot += Shot;
    }

    private void OnDisable()
    {
        attackAction.OnShoot -= Shot;
    }

    private void Shot(Projectile projectile)
    {
        if (lastProjectile != null)
        {
            lastProjectile.OnDestroyed -= ProjectileDestroyed;
        }
        lastProjectile = projectile;
        lastProjectile.OnDestroyed += ProjectileDestroyed;
    }

    public EnergyTypes.Colours GetWeakColour()
    {
        return weakColour;
    }

    private void ProjectileDestroyed(Projectile projectile, bool destroyedBySameShieldColour)
    {
        if (projectile != lastProjectile)
        {
            Debug.LogWarning("Something went wrong, the projectile watched destroyed is not the same as the last projectile fired");
        }

        var worldState = memory.GetWorldState();
        worldState.Set(GoapLabels.HasCorrectProjectileColour, !destroyedBySameShieldColour);
        if (destroyedBySameShieldColour)
        {
            weakColour = projectile.GetColour();
            worldState.Set(GoapLabels.WeakProjectileColour, projectile.GetColour());
        }

        lastProjectile.OnDestroyed -= ProjectileDestroyed;
        lastProjectile = null;
    }
}

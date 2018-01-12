using ReGoap.Unity;
using UnityEngine;
using TrixieCore.Goap;

public class EnemyProjectileSensor : ReGoapSensor<GoapLabels, object>
{
    private Projectile lastProjectile;
    private EnergyTypes.Colours weakColour;

    private void Start()
    {
        memory = GetComponent<GoapTestMem>();
    }
    
    private void OnEnable()
    {
        TrixieEvent.OnShot += Shot;
    }

    private void OnDisable()
    {
        TrixieEvent.OnShot -= Shot;
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

public static class TrixieEvent
{
    public delegate void ShootEvent(Projectile projectile);
    public static ShootEvent OnShot;

    public static void Shot(Projectile projectile)
    {
        if (OnShot != null)
        {
            OnShot(projectile);
        }
    }
}

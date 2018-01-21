using ReGoap.Unity;
using UnityEngine;
using TrixieCore.Goap;
using TrixieCore;

public class EnemyProjectileSensor : ReGoapSensor<GoapLabels, object>
{
    private EnemyGoapAgent agent;
    private AttackAction attackAction;

    private Projectile lastProjectile;
    private EnergyTypes.Colours weakColour;

    private bool watchingEvents;

    private void Start()
    {
        agent = GetComponentInParent<EnemyGoapAgent>();
        attackAction = agent.GetAction<AttackAction>();
        memory = agent.GetComponent<GoapTestMem>();
        OnEnable();
    }
    
    private void OnEnable()
    {
        if (watchingEvents) return;

        if (attackAction == null)
        {
            watchingEvents = false;
        }
        else
        {
            watchingEvents = true;
            attackAction.OnShoot += Shot;
        }
    }

    private void OnDisable()
    {
        if (watchingEvents)
        {
            attackAction.OnShoot -= Shot;
        }
        watchingEvents = false;
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

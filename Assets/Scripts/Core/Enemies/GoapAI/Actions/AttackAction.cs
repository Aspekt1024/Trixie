using System;
using System.Collections;
using UnityEngine;
using ReGoap.Core;
using ReGoap.Unity;
using TrixieCore.Goap;
using TrixieCore;

public class AttackAction : ReGoapAction<GoapLabels, object> {

    private float indicationDelay;
    private float cooldownDuration;
    private float lastAttackTime;
    private bool preparingAttack;
    private ShootComponent shootComponent;
    private AnimateState animState;

    public event Action<Projectile> OnShoot = delegate { };
    public event Action<EnergyTypes.Colours> OnShootPreparation = delegate { };
    
    public void ShotFired(Projectile projectile)
    {
        if (OnShoot != null)
        {
            OnShoot(projectile);
        }
    }
    
    public void ShootPreparation(EnergyTypes.Colours colour)
    {
        if (OnShootPreparation != null)
        {
            OnShootPreparation(colour);
        }
    }

    protected override void Awake()
    {
        base.Awake();
        shootComponent = GetComponentInParent<ShootComponent>();
        animState = GetComponentInParent<EnemyGoapAgent>().GetComponent<AnimateState>();

        cooldownDuration = shootComponent.Cooldown;
        indicationDelay = shootComponent.IndicationDelay;
    }

    public bool CanAttack
    {
        get { return Time.time >= lastAttackTime + cooldownDuration; }
    }

    public override ReGoapState<GoapLabels, object> GetEffects(ReGoapState<GoapLabels, object> goalState, IReGoapAction<GoapLabels, object> next = null)
    {
        effects.Clear();
        effects.Set(GoapLabels.EliminateThreats, true);
        return effects;
    }

    public override ReGoapState<GoapLabels, object> GetPreconditions(ReGoapState<GoapLabels, object> goalState, IReGoapAction<GoapLabels, object> next = null)
    {
        preconditions.Clear();
        preconditions.Set(GoapLabels.CanHitPlayer, true);
        preconditions.Set(GoapLabels.HasCorrectProjectileColour, true);
        return preconditions;
    }

    public override bool CheckProceduralCondition(IReGoapAgent<GoapLabels, object> goapAgent, ReGoapState<GoapLabels, object> goalState, IReGoapAction<GoapLabels, object> next = null)
    {
        bool check = base.CheckProceduralCondition(goapAgent, goalState, next) && CanAttack && !preparingAttack && ((GoapTestMem)agent.GetMemory()).CheckCondition(GoapLabels.HasSeenPlayerRecently);
        return check;
    }

    public override void Run(IReGoapAction<GoapLabels, object> previous, IReGoapAction<GoapLabels, object> next, IReGoapActionSettings<GoapLabels, object> settings, ReGoapState<GoapLabels, object> goalState, Action<IReGoapAction<GoapLabels, object>> done, Action<IReGoapAction<GoapLabels, object>> fail)
    {
        preparingAttack = true;
        base.Run(previous, next, settings, goalState, done, fail);
        ShootPreparation(shootComponent.ProjectileSettings.ProjectileColour);
        animState.Animate(indicationDelay, OnDoneCallback, OnFailCallback);
    }

    private void OnDoneCallback()
    {
        lastAttackTime = Time.time;
        preparingAttack = false;
        Projectile[] projectiles = shootComponent.Shoot(Player.Instance.gameObject);
        ShotFired(projectiles[0]);

        if (Player.Instance.GetComponent<PlayerHealthComponent>().IsAlive())
        {
            failCallback(this);
        }
        else
        {
            doneCallback(this);
        }
    }

    private void OnFailCallback()
    {
        failCallback(this);
    }
}

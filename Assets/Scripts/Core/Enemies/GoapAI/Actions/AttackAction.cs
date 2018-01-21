using System;
using System.Collections;
using UnityEngine;
using ReGoap.Core;
using ReGoap.Unity;
using TrixieCore.Goap;
using TrixieCore;

public class AttackAction : ReGoapAction<GoapLabels, object> {
    
    public float CooldownDuration = 1f;

    private float cooldownTimer;
    private ShootComponent shootComponent;
    private AnimateState animState;
    
    public delegate void ShootEvent(Projectile projectile);
    public ShootEvent OnShoot;
    public void ShotFired(Projectile projectile)
    {
        if (OnShoot != null)
        {
            OnShoot(projectile);
        }
    }

    public delegate void ShootPreparationEvent(EnergyTypes.Colours colour);
    public ShootPreparationEvent OnShootPreparation;
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
        if (cooldownTimer < CooldownDuration)
        {
            cooldownTimer += Time.deltaTime;
        }
        bool check = base.CheckProceduralCondition(goapAgent, goalState, next) && cooldownTimer >= CooldownDuration;
        return check;
    }

    public override void Run(IReGoapAction<GoapLabels, object> previous, IReGoapAction<GoapLabels, object> next, IReGoapActionSettings<GoapLabels, object> settings, ReGoapState<GoapLabels, object> goalState, Action<IReGoapAction<GoapLabels, object>> done, Action<IReGoapAction<GoapLabels, object>> fail)
    {
        base.Run(previous, next, settings, goalState, done, fail);
        ShootPreparation(shootComponent.ProjectileSettings.ProjectileColour);
        animState.Animate(0.5f, OnDoneCallback, OnFailCallback);
    }

    private void OnDoneCallback()
    {
        cooldownTimer = 0f;
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

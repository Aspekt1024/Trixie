using System;
using System.Collections;
using UnityEngine;
using ReGoap.Core;
using ReGoap.Unity;

using GoapLabels = GoapConditions.Labels;

public class AttackAction : ReGoapAction<GoapLabels, object> {
    
    public float CooldownDuration = 1f;

    private float cooldownTimer;
    private ShootComponent shootComponent;
    private AnimateState animState;
    
    protected override void Awake()
    {
        base.Awake();
        shootComponent = GetComponentInParent<ShootComponent>();
        animState = GetComponent<AnimateState>();
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
        preconditions.Set(GoapLabels.TargetFound, true);
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
        animState.Animate(0.5f, OnDoneCallback, OnFailCallback);
    }

    private void OnDoneCallback()
    {
        cooldownTimer = 0f;
        shootComponent.Shoot(Player.Instance.gameObject);
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

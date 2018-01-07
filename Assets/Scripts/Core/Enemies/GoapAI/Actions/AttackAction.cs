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

    private enum States
    {
        None, Shooting, HasShot
    }
    private States state;

    protected override void Awake()
    {
        base.Awake();
        shootComponent = GetComponentInParent<ShootComponent>();
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

        if (state == States.Shooting)
        {
            failCallback(this);
            return;
        };

        state = States.Shooting;

        StartCoroutine(ShootAnimation());
    }

    private IEnumerator ShootAnimation()
    {
        float shootTimer = 0f;
        while (shootTimer < 0.5f)
        {
            shootTimer += Time.deltaTime;
            yield return null;
        }
        shootComponent.Shoot(Player.Instance.gameObject);

        cooldownTimer = 0f;
        state = States.HasShot;

        doneCallback(this);
    }
}

using System;
using System.Collections;
using UnityEngine;
using ReGoap.Core;
using ReGoap.Unity;
using TrixieCore;
using TrixieCore.Goap;

public class MoveFromTargetAction : ReGoapAction<GoapLabels, object> {

    private EnemyGoapAgent agentAI;
    private GoapTestMem memory;
    private GotoState gotoState;
    private KeepDistanceGoal keepDistanceGoal;

    protected override void Awake()
    {
        base.Awake();
        agentAI = GetComponentInParent<EnemyGoapAgent>();
        gotoState = agentAI.GetComponent<GotoState>();
        memory = (GoapTestMem)agentAI.GetMemory();
        keepDistanceGoal = agentAI.GetGoal<KeepDistanceGoal>();
    }

    public override ReGoapState<GoapLabels, object> GetEffects(ReGoapState<GoapLabels, object> goalState, IReGoapAction<GoapLabels, object> next = null)
    {
        effects.Clear();
        effects.Set(GoapLabels.Goal_HasDistanceFromPlayer, true);
        return effects;
    }
    
    public override ReGoapState<GoapLabels, object> GetPreconditions(ReGoapState<GoapLabels, object> goalState, IReGoapAction<GoapLabels, object> next = null)
    {
        preconditions.Clear();
        preconditions.Set(GoapLabels.CanHitPlayer, true);
        //TODO preconditions.Set(GoapLabels.AttackIsOnCooldown, true);
        return preconditions;
    }

    public override bool CheckProceduralCondition(IReGoapAgent<GoapLabels, object> goapAgent, ReGoapState<GoapLabels, object> goalState, IReGoapAction<GoapLabels, object> next = null)
    {
        if (memory.CheckCondition(GoapLabels.HasDistanceFromPlayer) && memory.CheckCondition(GoapLabels.CanSeePlayer))
        {
            return false;
        }
        if (memory.CheckCondition(GoapLabels.IsStunned))
        {
            return false;
        }
        return base.CheckProceduralCondition(goapAgent, goalState, next);
    }

    public override void Run(IReGoapAction<GoapLabels, object> previous, IReGoapAction<GoapLabels, object> next, IReGoapActionSettings<GoapLabels, object> settings, ReGoapState<GoapLabels, object> goalState, Action<IReGoapAction<GoapLabels, object>> done, Action<IReGoapAction<GoapLabels, object>> fail)
    {
        base.Run(previous, next, settings, goalState, done, fail);
        Vector2 targetPosition = GetSafePosition();
        gotoState.GoTo(targetPosition, OnDoneCallback, OnFailCallback);

        if (!memory.CheckCondition(GoapLabels.CanSeePlayer) || memory.CheckCondition(GoapLabels.HasDistanceFromPlayer))
        {
            gotoState.Exit();
        }
        else
        {
            gotoState.SetTargetPosition(targetPosition);
        }
    }

    private Vector2 GetSafePosition()
    {
        Vector2 distVector = Player.Instance.transform.position - transform.position;
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, 0.6f, -distVector, keepDistanceGoal.DistanceToKeep, 1 << TrixieLayers.GetMask(Layers.Terrain));

        float distance = keepDistanceGoal.DistanceToKeep;
        if (hit.collider != null)
        {
            distance = distVector.magnitude + hit.distance;
        }
        return (Vector2)Player.Instance.transform.position - distVector.normalized * distance;
    }
    
    private void OnDoneCallback()
    {
        if (memory.CheckCondition(GoapLabels.HasDistanceFromPlayer))
        {
            failCallback(this);
            //doneCallback(this);
        }
        else
        {
            failCallback(this);
        }
    }

    private void OnFailCallback()
    {
        failCallback(this);
    }
}

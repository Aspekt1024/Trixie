using System;
using System.Collections;
using UnityEngine;
using ReGoap.Core;
using ReGoap.Unity;
using TrixieCore.Goap;

public class GuyPatrolAction : ReGoapAction<GoapLabels, object> {

    private EnemyGoapAgent agentAI;
    private GuyMemory memory;
    private BasicPatrolComponent patrolComponent;

    protected override void Awake()
    {
        base.Awake();
        agentAI = GetComponentInParent<EnemyGoapAgent>();
        memory = (GuyMemory)agentAI.GetMemory();
    }

    private void OnEnable()
    {
        patrolComponent = agentAI.Parent.GetComponent<BasicPatrolComponent>();
    }

    public override ReGoapState<GoapLabels, object> GetEffects(ReGoapState<GoapLabels, object> goalState, IReGoapAction<GoapLabels, object> next = null)
    {
        effects.Clear();
        effects.Set(GoapLabels.Guy_Goal_Patrol, true);
        return effects;
    }

    public override ReGoapState<GoapLabels, object> GetPreconditions(ReGoapState<GoapLabels, object> goalState, IReGoapAction<GoapLabels, object> next = null)
    {
        preconditions.Clear();
        return preconditions;
    }

    public override bool CheckProceduralCondition(IReGoapAgent<GoapLabels, object> goapAgent, ReGoapState<GoapLabels, object> goalState, IReGoapAction<GoapLabels, object> next = null)
    {
        if (agentAI.Parent.IsDead())
        {
            patrolComponent.Deactivate();
            return false;
        }
        return base.CheckProceduralCondition(goapAgent, goalState, next);
    }

    public override void Run(IReGoapAction<GoapLabels, object> previous, IReGoapAction<GoapLabels, object> next, IReGoapActionSettings<GoapLabels, object> settings, ReGoapState<GoapLabels, object> goalState, Action<IReGoapAction<GoapLabels, object>> done, Action<IReGoapAction<GoapLabels, object>> fail)
    {
        base.Run(previous, next, settings, goalState, done, fail);
        if (!memory.CheckCondition(GoapLabels.Guy_Grounded))
        {
            patrolComponent.Deactivate();
        }
        else
        {
            if (memory.CheckCondition(GoapLabels.Guy_WallForward) || !memory.CheckCondition(GoapLabels.Guy_GroundedForward))
            {
                patrolComponent.TurnAround();
            }
            patrolComponent.Activate();
        }

        failCallback(this);
    }
    
    private void OnDoneCallback()
    {
        //doneCallback(this);
        failCallback(this);
    }

    private void OnFailCallback()
    {
        failCallback(this);
    }
}

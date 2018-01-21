using System;
using System.Collections;
using UnityEngine;
using ReGoap.Core;
using ReGoap.Unity;
using TrixieCore.Goap;

public class LookForTargetAction : ReGoapAction<GoapLabels, object> {

    private EnemyGoapAgent agentAI;
    private GoapTestMem memory;
    private GotoState gotoState;
    
    protected override void Awake()
    {
        base.Awake();
        agentAI = GetComponentInParent<EnemyGoapAgent>();
        gotoState = agentAI.GetComponent<GotoState>();
        memory = (GoapTestMem)agentAI.GetMemory();
    }

    public override ReGoapState<GoapLabels, object> GetEffects(ReGoapState<GoapLabels, object> goalState, IReGoapAction<GoapLabels, object> next = null)
    {
        effects.Clear();
        effects.Set(GoapLabels.FindTarget, true);
        return effects;
    }

    public override bool CheckProceduralCondition(IReGoapAgent<GoapLabels, object> goapAgent, ReGoapState<GoapLabels, object> goalState, IReGoapAction<GoapLabels, object> next = null)
    {
        if (!memory.CheckCondition(GoapLabels.HasSeenPlayerRecently) && !memory.CheckCondition(GoapLabels.CanSensePlayer))
        {
            return false;
        }
        return base.CheckProceduralCondition(goapAgent, goalState, next);
    }

    public override void Run(IReGoapAction<GoapLabels, object> previous, IReGoapAction<GoapLabels, object> next, IReGoapActionSettings<GoapLabels, object> settings, ReGoapState<GoapLabels, object> goalState, Action<IReGoapAction<GoapLabels, object>> done, Action<IReGoapAction<GoapLabels, object>> fail)
    {
        base.Run(previous, next, settings, goalState, done, fail);
        gotoState.GoTo(memory.GetLastKnownPlayerPosition(), OnDoneCallback, OnFailCallback);

        if (memory.CheckCondition(GoapLabels.CanSeePlayer) || memory.CheckCondition(GoapLabels.CanHitPlayer))
        {
            gotoState.Exit();
        }
        else
        {
            gotoState.SetTargetPosition(memory.GetLastKnownPlayerPosition());
        }
    }
    
    private void OnDoneCallback()
    {
        if (memory.CheckCondition(GoapLabels.CanSeePlayer))
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

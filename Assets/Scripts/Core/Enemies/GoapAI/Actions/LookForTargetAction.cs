using System;
using System.Collections;
using UnityEngine;
using ReGoap.Core;
using ReGoap.Unity;
using TrixieCore.Goap;

[RequireComponent(typeof(GotoState))]
public class LookForTargetAction : ReGoapAction<GoapLabels, object> {

    private GoapTestMem memory;
    private GotoState gotoState;
    
    protected override void Awake()
    {
        base.Awake();
        effects.Set(GoapLabels.TargetFound, true);
        gotoState = GetComponent<GotoState>();
        memory = GetComponent<GoapTestMem>();
    }

    public override ReGoapState<GoapLabels, object> GetEffects(ReGoapState<GoapLabels, object> goalState, IReGoapAction<GoapLabels, object> next = null)
    {
        effects.Clear();
        effects.Set(GoapLabels.TargetFound, true);
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

        if (memory.CheckCondition(GoapLabels.CanSeePlayer))
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
            doneCallback(this);
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

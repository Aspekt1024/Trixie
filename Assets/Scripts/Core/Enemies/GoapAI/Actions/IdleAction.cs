using System;
using System.Collections;
using UnityEngine;
using ReGoap.Core;
using ReGoap.Unity;

using GoapLabels = GoapConditions.Labels;

public class IdleAction : ReGoapAction<GoapLabels, object>
{
    protected override void Awake()
    {
        base.Awake();
    }

    public override void Run(IReGoapAction<GoapLabels, object> previous, IReGoapAction<GoapLabels, object> next, IReGoapActionSettings<GoapLabels, object> settings, ReGoapState<GoapLabels, object> goalState, Action<IReGoapAction<GoapLabels, object>> done, Action<IReGoapAction<GoapLabels, object>> fail)
    {
        base.Run(previous, next, settings, goalState, done, fail);
        failCallback(this);
    }

    public override ReGoapState<GoapLabels, object> GetEffects(ReGoapState<GoapLabels, object> goalState, IReGoapAction<GoapLabels, object> next = null)
    {
        effects.Clear();
        effects.Set(GoapLabels.Idle, true);
        return effects;
    }

    public override ReGoapState<GoapLabels, object> GetPreconditions(ReGoapState<GoapLabels, object> goalState, IReGoapAction<GoapLabels, object> next = null)
    {
        preconditions.Clear();
        //preconditions.Set(GoapLabels.IsAtStartingPoint, true);
        return preconditions;
    }
}

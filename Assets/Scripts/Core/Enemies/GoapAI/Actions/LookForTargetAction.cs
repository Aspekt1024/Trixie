using System;
using System.Collections;
using UnityEngine;
using ReGoap.Core;
using ReGoap.Unity;

using GoapLabels = GoapConditions.Labels;

[RequireComponent(typeof(GotoState))]
public class LookForTargetAction : ReGoapAction<GoapLabels, object> {

    private VisionComponent vision;
    private GotoState gotoState;
    
    protected override void Awake()
    {
        base.Awake();
        effects.Set(GoapLabels.TargetFound, true);
        vision = GetComponentInParent<VisionComponent>();
        gotoState = GetComponent<GotoState>();
    }

    public override ReGoapState<GoapLabels, object> GetEffects(ReGoapState<GoapLabels, object> goalState, IReGoapAction<GoapLabels, object> next = null)
    {
        effects.Clear();
        effects.Set(GoapLabels.TargetFound, true);
        return effects;
    }

    public override bool CheckProceduralCondition(IReGoapAgent<GoapLabels, object> goapAgent, ReGoapState<GoapLabels, object> goalState, IReGoapAction<GoapLabels, object> next = null)
    {
        if (!vision.HasSeenPlayerRecenty())
        {
            return false;
        }
        return base.CheckProceduralCondition(goapAgent, goalState, next);
    }

    public override void Run(IReGoapAction<GoapLabels, object> previous, IReGoapAction<GoapLabels, object> next, IReGoapActionSettings<GoapLabels, object> settings, ReGoapState<GoapLabels, object> goalState, Action<IReGoapAction<GoapLabels, object>> done, Action<IReGoapAction<GoapLabels, object>> fail)
    {
        base.Run(previous, next, settings, goalState, done, fail);
        StartCoroutine(LookForTarget());
    }

    private IEnumerator LookForTarget()
    {
        gotoState.GoTo(vision.GetLastKnownPlayerPosition(), OnDoneCallback, OnFailCallback);

        while (vision.HasSeenPlayerRecenty())
        {
            if (vision.CanSeePlayer())
            {
                gotoState.Exit();
                yield break;
            }
            else
            {
                gotoState.SetTargetPosition(vision.GetLastKnownPlayerPosition());
            }
            yield return null;
        }

        gotoState.Exit();
    }

    private void OnDoneCallback()
    {
        if (vision.CanSeePlayer())
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

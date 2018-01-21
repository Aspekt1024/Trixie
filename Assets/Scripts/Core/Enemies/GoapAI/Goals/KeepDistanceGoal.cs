using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReGoap.Unity;
using ReGoap.Core;
using TrixieCore.Goap;

public class KeepDistanceGoal : ReGoapGoal<GoapLabels, object> {

    public float DistanceToKeep = 5f;

    protected override void Awake()
    {
        base.Awake();
        goal.Set(GoapLabels.Goal_HasDistanceFromPlayer, true);
    }
}

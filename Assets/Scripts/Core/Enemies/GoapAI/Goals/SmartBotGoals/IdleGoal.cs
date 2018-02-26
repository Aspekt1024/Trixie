using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReGoap.Unity;
using ReGoap.Core;
using TrixieCore.Goap;

public class IdleGoal : ReGoapGoal<GoapLabels, object> {

    protected override void Awake()
    {
        base.Awake();
        goal.Set(GoapLabels.Idle, true);
    }
}

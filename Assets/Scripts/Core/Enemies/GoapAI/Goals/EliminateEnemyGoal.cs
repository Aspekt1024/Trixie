﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReGoap.Unity;
using ReGoap.Core;
using GoapLabels = GoapConditions.Labels;

public class EliminateEnemyGoal : ReGoapGoal<GoapLabels, object> {

    protected override void Awake()
    {
        base.Awake();
        goal.Set(GoapLabels.EliminateThreats, true);
    }
}

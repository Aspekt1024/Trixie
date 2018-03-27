﻿using Aspekt.AI;
using TestUnitLabels;
using UnityEngine;

public class AttackGoal : AIGoal
{
    protected override void SetConditions()
    {
        AddCondition(SauceLabels.TargetAttacked, true);
    }
}
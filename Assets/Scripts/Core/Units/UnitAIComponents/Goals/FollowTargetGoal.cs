using Aspekt.AI;
using TestUnitLabels;
using UnityEngine;

public class FollowTargetGoal : AIGoal
{
    protected override void SetConditions()
    {
        AddCondition(AILabels.TargetReached.ToString(), true);
    }
}
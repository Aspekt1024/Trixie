using Aspekt.AI;
using TestUnitLabels;
using UnityEngine;

public class CollectWoodGoal : AIGoal
{
    protected override void SetConditions()
    {
        AddCondition(AILabels.SellWood.ToString(), true);
    }
}
using ReGoap.Unity;
using UnityEngine;
using TrixieCore.Goap;
using TrixieCore;
using System.Collections.Generic;

public class EnemyDistanceSensor : ReGoapSensor<GoapLabels, object>
{
    private KeepDistanceGoal keepDistanceGoal;
    
    private LayerMask hitLayers;

    private void Start()
    {
        var agentAi = GetComponentInParent<EnemyGoapAgent>();
        memory = agentAi.GetMemory();
        keepDistanceGoal = agentAi.GetGoal<KeepDistanceGoal>();
    }

    public override void UpdateSensor()
    {
        bool hasDistance = Vector2.Distance(Player.Instance.transform.position, transform.position) > keepDistanceGoal.DistanceToKeep;
        memory.GetWorldState().Set(GoapLabels.HasDistanceFromPlayer, hasDistance);
    }
}

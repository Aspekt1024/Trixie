using ReGoap.Unity;
using UnityEngine;

using GoapLabels = GoapConditions.Labels;

public class EnemySensor : ReGoapSensor<GoapLabels, object>
{

    private VisionComponent vision;

    private void Start()
    {
        vision = GetComponentInParent<VisionComponent>();
    }

    public override void UpdateSensor()
    {
        memory = GetComponent<GoapTestMem>();
        var worldState = memory.GetWorldState();

        worldState.Set(GoapLabels.TargetFound, vision.CanSeePlayer());
        worldState.Set(GoapLabels.CanSeePlayer, vision.CanSeePlayer());
        worldState.Set(GoapLabels.HasSeenPlayerRecently, vision.HasSeenPlayerRecenty());
        worldState.Set(GoapLabels.NotSeenPlayerRecently, !vision.HasSeenPlayerRecenty());
    }
}

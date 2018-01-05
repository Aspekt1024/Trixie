using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookForTargetAction : GoapAction {

    private VisionComponent vision;

    public LookForTargetAction()
    {
        AddPrecondition(GoapLabels.HasSeenPlayerRecently, true);
        AddEffect(GoapLabels.TargetFound, true);
    }

    private void Start()
    {
        vision = GetComponent<VisionComponent>();
    }

    public override bool CheckProceduralPrecondition(GameObject agent)
    {
        target = GetComponent<SauceRobot>().GetTargetObject();
        return true;
    }

    public override bool IsDone()
    {
        return vision.CanSeePlayer();
    }

    public override bool Perform(GameObject agent)
    {
        return vision.HasSeenPlayerRecenty();
    }

    public override bool RequiresInRange()
    {
        return true;
    }

    public override void ResetAction()
    {

    }
}

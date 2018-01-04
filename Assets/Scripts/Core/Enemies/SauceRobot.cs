using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GoapLabels = GoapAction.GoapLabels;

public class SauceRobot : BaseEnemy, IGoap {

    private VisionComponent vision;
    private bool isShrunk;
    
    public void SetShrunkState()
    {
        isShrunk = true;
    }

    private void Start()
    {
        vision = GetComponent<VisionComponent>();
        vision.Activate();
    }
    
    public void ActionsFinished()
    {
    }

    public HashSet<KeyValuePair<GoapLabels, object>> CreateGoalState()
    {
        return new HashSet<KeyValuePair<GoapLabels, object>>()
        {
            new KeyValuePair<GoapLabels, object>(GoapLabels.KillPlayer, true),
            //new KeyValuePair<GoapLabels, object>(GoapLabels.Survive, true),
            //new KeyValuePair<GoapLabels, object>(GoapLabels.Idle, true)
        };
    }

    public HashSet<KeyValuePair<GoapLabels, object>> GetWorldState()
    {
        return new HashSet<KeyValuePair<GoapLabels, object>>()
        {
            new KeyValuePair<GoapLabels, object>(GoapLabels.CanSeePlayer, vision.CanSeePlayer()),
            //new KeyValuePair<GoapLabels, object>(GoapLabels.IsDying, GetComponent<HealthComponent>().GetHealth() == 1 && GetComponent<HealthComponent>().MaxHealth > 1),
        };
    }

    public bool MoveAgent(GoapAction nextAction)
    {
        return false;
    }

    public void PlanAborted(GoapAction aborter)
    {
        Debug.LogWarning("bailed on plan!");
    }

    public void PlanFailed(HashSet<KeyValuePair<GoapLabels, object>> failedGoal)
    {
    }

    public void PlanFound(HashSet<KeyValuePair<GoapLabels, object>> goal, Queue<GoapAction> actions)
    {
    }
}

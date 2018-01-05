using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GoapLabels = GoapAction.GoapLabels;

public class SauceRobot : BaseEnemy, IGoap {

    private Transform movementTf;
    private bool isShrunk;
    private EnemyAITest pathFinder;
    private Vector2 startPosition;

    private VisionComponent vision;
    
    public void SetShrunkState()
    {
        isShrunk = true;
    }

    private void Start()
    {
        movementTf = new GameObject("MovementTf").transform;
        
        vision = GetComponent<VisionComponent>();
        vision.Activate();

        pathFinder = GetComponent<EnemyAITest>();

        startPosition = transform.position;
    }

    public GameObject GetTargetObject()
    {
        return movementTf.gameObject;
    }

    public void SetTargetPosition(Vector2 position)
    {
        movementTf.position = position;
    }
    
    public void ActionsFinished()
    {
    }

    public Dictionary<GoapLabels, object> CreateGoalState()
    {
        Dictionary<GoapLabels, object> goals = new Dictionary<GoapLabels, object>();

        if (vision.HasSeenPlayerRecenty() || vision.CanSeePlayer())
        {
            goals.Add(GoapLabels.EliminateThreats, true);
        }
        else
        {
            goals.Add(GoapLabels.Survive, true);
        }
        
        return goals;
    }

    public Dictionary<GoapLabels, object> GetWorldState()
    {
        return new Dictionary<GoapLabels, object>()
        {
            { GoapLabels.CanSeePlayer, vision.CanSeePlayer() },
            { GoapLabels.IsDying, GetComponent<HealthComponent>().GetHealth() == 1 && GetComponent<HealthComponent>().MaxHealth > 1 },
            { GoapLabels.HasSeenPlayerRecently, vision.HasSeenPlayerRecenty() },
            { GoapLabels.TargetFound, vision.CanSeePlayer() }
        };
    }

    public bool MoveAgent(GoapAction nextAction)
    {
        Transform target = null;
        if (vision.CanSeePlayer())
        {
            target = Player.Instance.transform;
        }
        else if (vision.HasSeenPlayerRecenty())
        {
            movementTf.transform.position = vision.GetLastKnownPlayerPosition();
            target = movementTf;
        }
        
        if (target != null)
        {
            pathFinder.Activate(target.transform);
        }
        nextAction.SetInRange(pathFinder.FinishedPathing());
        return pathFinder.FinishedPathing();
    }

    public void PlanAborted(GoapAction aborter)
    {
        movementTf.position = startPosition;
        pathFinder.Activate(movementTf);
    }

    public void PlanFailed(Dictionary<GoapLabels, object> failedGoal)
    {
    }

    public void PlanFound(Dictionary<GoapLabels, object> goal, Queue<GoapAction> actions)
    {
        pathFinder.CancelPath();
    }
}

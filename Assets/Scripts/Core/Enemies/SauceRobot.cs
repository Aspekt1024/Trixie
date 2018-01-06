using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GoapLabels = GoapAction.GoapLabels;

public class SauceRobot : BaseEnemy, IGoap {

    private Transform movementTf;
    private bool isShrunk;
    private Vector2 startPosition;
    
    private EnemyAITest pathFinder;
    private VisionComponent vision;
    private SpriteRenderer spriteRenderer;
    
    public void SetShrunkState()
    {
        isShrunk = true;
    }

    private void Start()
    {
        movementTf = new GameObject("MovementTf").transform;
        

        pathFinder = GetComponent<EnemyAITest>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        vision = GetComponent<VisionComponent>();
        vision.Activate();

        startPosition = transform.position;
    }

    public void Update()
    {
        UpdateLookDirection();
    }

    public GameObject GetTargetObject()
    {
        return movementTf.gameObject;
    }

    public void SetTargetPosition(Vector2 position)
    {
        movementTf.position = position;
    }

#region GOAP AI
    public void ActionsFinished()
    {
    }

    public Dictionary<GoapLabels, object> CreateGoalState()
    {
        Dictionary<GoapLabels, object> goals = new Dictionary<GoapLabels, object>();

        if (vision.HasSeenPlayerRecenty() || vision.CanSeePlayer())
        {
            Camera.main.GetComponent<CameraFollow>().AddObjectToFollow(transform);
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
        Camera.main.GetComponent<CameraFollow>().StopFollowingObject(transform);
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
#endregion GOAP AI

    public override void DamageEnemy(Vector2 direction, int damage = 1)
    {
        HealthComponent healthComponent = GetComponent<HealthComponent>();
        healthComponent.TakeDamage(damage);
        if (healthComponent.IsDead())
        {
            DestroyEnemy();
        }
        else
        {
            // TODO can't overlap!
            StartCoroutine(ShowDamaged());
        }
    }
    
    private IEnumerator ShowDamaged()
    {
        spriteRenderer.color = new Color(1f, 0f, 0f, 0.5f);
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.color = Color.white;
    }

    private void UpdateLookDirection()
    {
        if (pathFinder.Target == null) return;
        if (transform.position.x > pathFinder.Target.position.x)
        {
            vision.FaceInitialDirection();
        }
        else
        {
            vision.FaceOppositeDirection();
        }
    }

    protected override void DestroyEnemy()
    {
        gameObject.SetActive(false);
    }
}

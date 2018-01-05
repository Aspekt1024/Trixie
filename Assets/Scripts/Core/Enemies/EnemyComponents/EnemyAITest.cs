using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

[RequireComponent(typeof(VisionComponent))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Seeker))]
public class EnemyAITest : MonoBehaviour {

    public Transform Target;
    public float UpdateFrequency = 2f;
    public float Speed = 300f;
    public ForceMode2D ForceMode;

    public float nextWaypointDistance = 3f;
    public float MinDistToTarget = 5f;
    public float MaxDistToTarget = 18f;

    [HideInInspector] public Path Path;
    [HideInInspector] public bool PathIsEnded = false;

    private Seeker seeker;
    private Rigidbody2D body;
    private VisionComponent vision;
    private int currentWaypoint = 0;

    private bool currentlySeeking;

    private void Start()
    {
        seeker = GetComponent<Seeker>();
        body = GetComponent<Rigidbody2D>();
        vision = GetComponent<VisionComponent>();

        if (Target == null)
        {
            return;
        }

        Activate(Target);
    }

    public void Activate(Transform target)
    {
        Target = target;
        if (currentlySeeking) return;

        currentlySeeking = true;
        InvokeRepeating("UpdatePath", 0f, 1f / UpdateFrequency);
    }

    public bool FinishedPathing()
    {
        return PathIsEnded;
    }

    public void CancelPath()
    {
        Path = null;
    }

    private void FixedUpdate()
    {
        if (Target == null || Path == null) return;

        if (InSweetSpot())
        {
            // TODO: Randomise path
            PathIsEnded = true;
            return;
        }

        if (currentWaypoint >= Path.vectorPath.Count)
        {
            if (PathIsEnded) return;
            
            PathIsEnded = true;
            return;
        }
        PathIsEnded = false;

        Vector3 dir = (Path.vectorPath[currentWaypoint] - transform.position).normalized;
        dir *= Speed * Time.fixedDeltaTime;

        body.AddForce(dir, ForceMode);

        if (Vector3.Distance(transform.position, Path.vectorPath[currentWaypoint]) < nextWaypointDistance)
        {
            currentWaypoint++;
            return;
        }
    }

    private bool InSweetSpot()
    {
        if (!vision.CanSeePlayer()) return false;

        float dist = Vector2.Distance(Target.position, transform.position);
        return dist > MinDistToTarget && dist < MaxDistToTarget;
    }

    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            Path = p;
            currentWaypoint = 0;
        }
    }

    private void UpdatePath()
    {
        if (Target == null)
        {
            // TODO: insert a player search here
            return;
        }

        seeker.StartPath(transform.position, Target.position, OnPathComplete);
    }

}

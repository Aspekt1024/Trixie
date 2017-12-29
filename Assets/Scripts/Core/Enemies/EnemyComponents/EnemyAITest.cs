using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

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
    private int currentWaypoint = 0;

    private void Start()
    {
        seeker = GetComponent<Seeker>();
        body = GetComponent<Rigidbody2D>();

        if (Target == null)
        {
            Debug.LogError("no target found on " + name);
            return;
        }

        InvokeRepeating("UpdatePath", 0f, 1f / UpdateFrequency);
    }

    private void FixedUpdate()
    {
        if (Target == null || Path == null) return;

        if (InSweetSpot())
        {
            // TODO: Randomise path
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
        if (!TargetInLineOfSight()) return false;

        float dist = Vector2.Distance(Target.position, transform.position);
        return dist > MinDistToTarget && dist < MaxDistToTarget;
    }

    private bool TargetInLineOfSight()
    {
        Vector2 distVector = Target.position - transform.position;
        
        RaycastHit2D hit = Physics2D.Raycast(transform.position, distVector, 500, 1 << Target.gameObject.layer | 1 << LayerMask.NameToLayer("Terrain"));

        if (hit.collider != null && hit.collider.gameObject.layer != LayerMask.NameToLayer("Terrain"))
        {
            return true;
        }
        return false;
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

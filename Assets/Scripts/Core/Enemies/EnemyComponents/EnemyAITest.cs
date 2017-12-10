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
        if (Target == null) return;

        // TODO: Always look at player

        if (Path == null) return;

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

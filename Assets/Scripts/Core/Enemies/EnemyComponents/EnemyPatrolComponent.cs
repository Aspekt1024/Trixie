using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrolComponent : MonoBehaviour {

    // TODO create inspector component to add these
    public Transform[] PatrolPoints;
    public bool GroundOnly;
    public float WaitTimeAtEachPoint = 1f;

    private Vector3[] points;

    private int targetPointIndex;
    private float moveSpeed = 1f;
    private float timePointReached;

    // TODO interface with EnemyMoveComponent??

    private enum States
    {
        None, Active
    }
    private States state;

    public void Activate()
    {
        targetPointIndex = 0;
        state = States.Active;
    }

    public void Deactivate()
    {
        state = States.None;
    }

    private void Start()
    {
        points = new Vector3[PatrolPoints.Length];
        for (int i = 0; i < points.Length; i++)
        {
            points[i] = PatrolPoints[i].position;
        }
    }

    private void Update()
    {
        switch (state)
        {
            case States.None:
                break;
            case States.Active:
                MoveTowardsPoint();
                break;
            default:
                break;
        }
    }

    public void SetMoveSpeed(float speed)
    {
        moveSpeed = speed;
    }

    private void MoveTowardsPoint()
    {
        if (timePointReached + WaitTimeAtEachPoint > Time.time) return;
        
        float xMovement = Mathf.Clamp(points[targetPointIndex].x - transform.position.x, -Time.deltaTime * moveSpeed, Time.deltaTime * moveSpeed);
        float yMovement = 0f;
        if (!GroundOnly)
        {
            yMovement = Mathf.Clamp(points[targetPointIndex].y - transform.position.y, -Time.deltaTime * moveSpeed, Time.deltaTime * moveSpeed);  // todo need to calc magnitude
        }

        transform.position += new Vector3(xMovement, yMovement, 0f);

        if (Vector2.Distance(transform.position, points[targetPointIndex]) < 0.01f)
        {
            timePointReached = Time.time;
            targetPointIndex++;
            if (targetPointIndex == points.Length)
            {
                targetPointIndex = 0;
            }
        }
    }
}

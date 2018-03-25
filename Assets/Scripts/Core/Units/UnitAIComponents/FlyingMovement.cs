using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Aspekt.AI;

public class FlyingMovement : MonoBehaviour, IAIMovementBehaviour
{
    public float Speed = 4f;
    private Transform target;
    private Rigidbody2D body;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }

    private enum States
    {
        Stopped, Active, TargetReached
    }
    private States state;


    public void Run(Transform target)
    {
        this.target = target;
        state = States.Active;
    }

    public void Stop()
    {
        state = States.Stopped;
    }

    public bool TargetReached()
    {
        return state == States.TargetReached;
    }

    public void SetTargetReached()
    {
        
    }

    public void Tick(float deltaTime)
    {
        switch (state)
        {
            case States.Stopped:
                body.velocity = Vector3.Lerp(body.velocity, Vector3.zero, deltaTime);
                break;
            case States.Active:
                Move(deltaTime);
                break;
            case States.TargetReached:
                break;
            default:
                break;
        }
    }

    public void UpdateTarget(Transform target)
    {
        this.target = target;
    }

    private void Move(float deltaTime)
    {
        if (Vector2.Distance(target.position, body.transform.position) < 4f)
        {
            state = States.TargetReached;
        }
        else
        {
            body.velocity = (target.position - body.transform.position).normalized * Speed;
        }
    }
}

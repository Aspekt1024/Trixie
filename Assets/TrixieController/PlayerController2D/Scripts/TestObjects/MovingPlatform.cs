using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour, IShootable {

    public float MoveSpeed = 5f;
    public Directions StartDirection = Directions.Incrementing;
    public States InitialState = States.Moving;

    public Vector2[] MovementPoints;
    public float StationaryTime;
    public int StartingPoint;

    private int pointIndex;
    private Vector2 currentPoint;
    private Vector2 previousPoint;
    private float timeToPoint;
    private float moveTimer;

    private Vector2 velocity;
    
    public enum States
    {
        Idle, Moving, Frozen, PausedAtPoint
    }
    private States state;

    public enum Directions
    {
        Incrementing, Decrementing
    }
    private Directions direction;

    public bool ClosedLoop = true;

    public void HandleShot(Bullet bullet)
    {
        if (bullet.BulletType == Bullet.BulletTypes.Frost)
        {
            Freeze();
        }
    }

    private void Start()
    {
        state = InitialState;
        direction = StartDirection;

        pointIndex = StartingPoint;
        transform.position = MovementPoints[pointIndex];
        CalculateMovementToNextPoint();
    }

    public Vector2 GetVelocity()
    {
        return velocity;
    }

    private void Update()
    {
        if (MovementPoints.Length < 2) return;

        switch (state)
        {
            case States.Idle:
                break;
            case States.Moving:
                moveTimer += Time.deltaTime;
                transform.position = Vector2.Lerp(previousPoint, currentPoint, moveTimer / timeToPoint);

                if (moveTimer > timeToPoint)
                {
                    state = States.PausedAtPoint;
                    timeToPoint = StationaryTime;
                    moveTimer = 0f;
                    velocity = Vector2.zero;
                }
                break;
            case States.Frozen:
                break;
            case States.PausedAtPoint:
                moveTimer += Time.deltaTime;
                if (moveTimer > timeToPoint)
                {
                    state = States.Moving;
                    CalculateMovementToNextPoint();
                }
                break;
            default:
                break;
        }
    }

    private void Freeze()
    {
        state = States.Frozen;
    }

    private void CalculateMovementToNextPoint()
    {
        if (MovementPoints.Length < 2) return;

        previousPoint = MovementPoints[pointIndex];
        if (direction == Directions.Incrementing)
        {
            pointIndex++;
            if (pointIndex == MovementPoints.Length)
            {
                if (ClosedLoop)
                {
                    pointIndex = 0;
                }
                else
                {
                    direction = Directions.Decrementing;
                    pointIndex -= 2;
                }
            }
        }
        else
        {
            pointIndex--;
            if (pointIndex == -1)
            {
                if (ClosedLoop)
                {
                    pointIndex = MovementPoints.Length - 1;
                }
                else
                {
                    direction = Directions.Incrementing;
                    pointIndex += 2;
                }
            }
        }
        currentPoint = MovementPoints[pointIndex];

        timeToPoint = Vector2.Distance(currentPoint, previousPoint) / MoveSpeed;
        moveTimer = 0f;
        velocity = (currentPoint - previousPoint).normalized * MoveSpeed;
    }
    
}

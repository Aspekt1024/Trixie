using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicPatrolComponent : MonoBehaviour {

    public float WaitTimeAtEdge = 0.3f;
    public float MoveSpeed = 7f;
    public float Acceleration = 3f;
    public bool SpriteFacesLeft = false;

    private enum States
    {
        None, Moving, Waiting
    }
    private States state;

    private enum MoveDirections
    {
        Left, Right
    }
    private MoveDirections currentDirection;

    private float targetVelocity;
    private Rigidbody2D body;
    private SpriteRenderer sr;
    private Vector2 originalScale;

    private GameObject startingPlatform;

    private void Start()
    {
        currentDirection = MoveDirections.Left;
        state = States.None;
        body = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        originalScale = transform.localScale;
        GetStartingPlatform();
    }

    public void Activate()
    {
        state = States.Moving;
        targetVelocity = MoveSpeed;
        SetLookDirection();
    }

    public void Deactivate()
    {
        state = States.None;
    }

    private void Update()
    {
        switch (state)
        {
            case States.None:
                break;
            case States.Moving:
                Move();
                break;
            case States.Waiting:
                break;
            default:
                break;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Terrain"))
        {
            if (collision.gameObject != startingPlatform)
            {
                SwitchDirection();
            }
        }
    }

    private void Move()
    {
        float velocityDireciton = currentDirection == MoveDirections.Left ? -1f : 1f;
        body.velocity = new Vector2(Mathf.Lerp(body.velocity.x, targetVelocity * velocityDireciton, Time.deltaTime * Acceleration), body.velocity.y);
        CheckForEdge();
    }

    private void CheckForEdge()
    {

    }

    private void SwitchDirection()
    {
        if (currentDirection == MoveDirections.Left)
        {
            currentDirection = MoveDirections.Right;
        }
        else
        {
            currentDirection = MoveDirections.Left;
        }
        SetLookDirection();
    }

    private void SetLookDirection()
    {
        float lookDirectionScaleModifier = 1f;
        if (currentDirection == MoveDirections.Right && SpriteFacesLeft)
        {
            lookDirectionScaleModifier = -1f;
        }
        else if (currentDirection == MoveDirections.Left && !SpriteFacesLeft)
        {
            lookDirectionScaleModifier = -1f;
        }
        transform.localScale = new Vector3(lookDirectionScaleModifier * originalScale.x, originalScale.y, 1f);
    }

    private void GetStartingPlatform()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 10f, 1 << LayerMask.NameToLayer("Terrain"));
        if (hit.collider != null)
        {
            startingPlatform = hit.collider.gameObject;
        }
    }
}

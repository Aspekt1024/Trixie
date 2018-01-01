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
        None, Moving, SwitchingDirection
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
    private float waitTime;

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
        targetVelocity = 0f;
        state = States.None;
    }

    private void Update()
    {
        switch (state)
        {
            case States.None:
                UpdateVelocity();
                break;
            case States.Moving:
                if (AtWallOrEdge())
                {
                    waitTime = 0f;
                    state = States.SwitchingDirection;
                }
                else
                {
                    UpdateVelocity();
                }
                break;
            case States.SwitchingDirection:
                waitTime += Time.deltaTime;
                if (waitTime >= WaitTimeAtEdge)
                {
                    SwitchDirection();
                    state = States.Moving;
                }
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

    private void UpdateVelocity()
    {
        float velocityDireciton = currentDirection == MoveDirections.Left ? -1f : 1f;
        body.velocity = new Vector2(Mathf.Lerp(body.velocity.x, targetVelocity * velocityDireciton, Time.deltaTime * Acceleration), body.velocity.y);
    }

    private bool AtWallOrEdge()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right * (currentDirection == MoveDirections.Right ? 1 : -1), GetComponent<CapsuleCollider2D>().bounds.extents.x, 1 << LayerMask.NameToLayer("Terrain"));
        return hit.collider != null;
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

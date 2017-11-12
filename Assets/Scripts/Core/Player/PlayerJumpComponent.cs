using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpComponent : MonoBehaviour {

    public GameObject Jetpacks;
    public Transform GroundCheckObj;

    private const float groundCheckRadius = 0.1f;
    private LayerMask groundLayer;

    private Rigidbody2D body;

    private const float jumpVelocity = 15f;
    private const float boostVelocity = 35f;
    private const float fallVelocity = 15f;
    private const float maxFallVelocity = 25f;

    private float jumpTimer;
    private const float maxJumpTime = 0.3f;

    private float boostTimer;
    private const float maxBoostTime = 1.2f;
    
    private float targetVerticalVelocity;
    
    private enum States
    {
        None, Jumping, PostJump, Boosting, PostBoost, Falling, Grounded
    }
    private States state;

    private void Awake()
    {
        state = States.Falling;
        body = GetComponent<Rigidbody2D>();
        Jetpacks.SetActive(false);
        groundLayer = 1 << LayerMask.NameToLayer("Terrain");
    }

    private void FixedUpdate()
    {
        switch(state)
        {
            case States.Grounded:
                CheckNotGrounded();
                break;
            case States.Falling:
                CheckGrounded();
                break;
            case States.Boosting:
                boostTimer += Time.deltaTime;
                if (boostTimer > maxBoostTime)
                {
                    SetPostBoostState();
                }
                break;
            case States.Jumping:
                jumpTimer += Time.deltaTime;
                if (jumpTimer > maxJumpTime)
                {
                    SetPostJumpState();
                }
                break;
            case States.PostBoost:
                break;
            case States.PostJump:
                break;
        }

        UpdateVelocity();
        if (state != States.Grounded)
        {
            CheckFalling();
        }
    }
    
    public void Jump()
    {
        if (state == States.Jumping || state == States.Falling || state == States.PostJump || state == States.PostBoost)
        {
            UseBoost();
        }
        else if (state == States.Grounded)
        {
            state = States.Jumping;
            jumpTimer = 0f;
            body.velocity = new Vector2(body.velocity.x, jumpVelocity);
            targetVerticalVelocity = jumpVelocity;
        }
    }
    
    public void JumpReleased()
    {
        if (state == States.Jumping)
        {
            SetPostJumpState();
        }
        if (state == States.Boosting)
        { 
            SetPostBoostState();
        }
    }

    private void SetPostJumpState()
    {
        body.velocity = new Vector2(body.velocity.x, body.velocity.y / 2f);
        targetVerticalVelocity = -fallVelocity;
        state = States.PostJump;
    }

    private void SetPostBoostState()
    {
        state = States.PostBoost;
        targetVerticalVelocity = -fallVelocity / 4f;
        Jetpacks.SetActive(false);
    }

    private void UpdateVelocity()
    {
        body.velocity = new Vector2(body.velocity.x, Mathf.Lerp(body.velocity.y, targetVerticalVelocity, 2f * Time.deltaTime));
    }
    
    private void CheckGrounded()
    {
        Collider2D collider = Physics2D.OverlapCircle(GroundCheckObj.position, groundCheckRadius, groundLayer);
        if (collider != null)
        {
            boostTimer = 0f;    // TODO move to update - add cooldown
            state = States.Grounded;
        }
    }

    private void CheckNotGrounded()
    {
        Collider2D collider = Physics2D.OverlapCircle(GroundCheckObj.position, groundCheckRadius, groundLayer);
        if (collider == null)
        {
            if (body.velocity.y > 0f)
            {
                state = States.Jumping;
            }
            else
            {
                state = States.Falling;
            }
        }
    }

    private void CheckFalling()
    {
        if (body.velocity.y <= 0f && state != States.Boosting)
        {
            targetVerticalVelocity = -maxFallVelocity;
            state = States.Falling;
        }
    }

    private void UseBoost()
    {
        state = States.Boosting;
        targetVerticalVelocity = boostVelocity;
        Jetpacks.SetActive(true);
    }
}

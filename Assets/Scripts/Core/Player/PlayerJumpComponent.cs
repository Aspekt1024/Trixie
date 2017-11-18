using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpComponent : MonoBehaviour {

    public float JumpVelocity = 15f;
    public float BoostVelocity = 10f;
    public float FallVelocity = 15f;
    public float MaxFallVelocity = 25f;

    public float MaxJumpTime = 0.4f;
    public float MaxBoostTime = 1.2f;

    public GameObject Jetpacks;
    public Transform GroundCheckObj;

    private const float groundCheckRadius = 0.1f;
    private LayerMask groundLayer;

    private Rigidbody2D body;
    private Animator anim;

    private float jumpTimer;

    private float boostTimer;
    
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
        anim = GetComponent<Animator>();
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
                if (boostTimer > MaxBoostTime)
                {
                    SetPostBoostState();
                }
                break;
            case States.Jumping:
                jumpTimer += Time.deltaTime;
                if (jumpTimer > MaxJumpTime)
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
            anim.Play("Jump");
            anim.SetBool("grounded", false);
            jumpTimer = 0f;
            body.velocity = new Vector2(body.velocity.x, JumpVelocity);
            targetVerticalVelocity = JumpVelocity * 0.5f;
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
        targetVerticalVelocity = -FallVelocity;
        state = States.PostJump;
    }

    private void SetPostBoostState()
    {
        state = States.PostBoost;
        if (body.velocity.y > 0f)
        {
            body.velocity = new Vector2(body.velocity.x, body.velocity.y / 3f);
        }
        targetVerticalVelocity = -FallVelocity / 4f;
        Jetpacks.SetActive(false);
        anim.SetBool("jetpacking", false);
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
            anim.SetBool("grounded", true);
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
            anim.SetBool("grounded", false);
        }
    }

    private void CheckFalling()
    {
        if (body.velocity.y <= 0f && state != States.Boosting)
        {
            targetVerticalVelocity = -MaxFallVelocity;
            state = States.Falling;
            anim.SetBool("falling", true);
        }
        else
        {
            anim.SetBool("falling", false);
        }
    }

    private void UseBoost()
    {
        state = States.Boosting;
        targetVerticalVelocity = BoostVelocity;
        anim.SetBool("jetpacking", true);
        //Jetpacks.SetActive(true);
    }
}

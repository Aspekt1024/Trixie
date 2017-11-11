using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpComponent : MonoBehaviour {

    public GameObject Jetpacks;
    public Transform GroundCheckObj;

    private const float groundCheckRadius = 0.1f;
    private LayerMask groundLayer;

    private Rigidbody2D body;

    private const float jumpForce = 1500f;
    private const float additionalJumpForce = 60f;
    private const float boostForce = 160;


    private float boostTimer;
    private float boostTimerTop;
    private const float maxBoostTime = 1.4f;

    private bool boostUsed;
    
    private enum States
    {
        None, JumpStart, Jumping, BoostStart, Boosting, PostBoost, Falling, Grounded
    }
    private States state;

    private void Awake()
    {
        state = States.Grounded;
        body = GetComponent<Rigidbody2D>();
        Jetpacks.SetActive(false);
        groundLayer = 1 << LayerMask.NameToLayer("Terrain");
    }

    private void FixedUpdate()
    {
        switch(state)
        {
            case States.JumpStart:
                state = States.Jumping;
                break;
            case States.BoostStart:
                state = States.Boosting;
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
                CheckFalling();
                break;
            case States.Jumping:
                CheckFalling();
                break;
            case States.PostBoost:
                AddPostBoostForce();
                CheckFalling();
                break;
        }
    }
    
    public void Jump()
    {
        if (state == States.Jumping || state == States.Falling)
        {
            if (!boostUsed)
            {
                UseBoost();
            }
        }
        else if (state == States.Grounded)
        {
            state = States.JumpStart;
            body.AddForce(jumpForce * Vector2.up);
        }
    }

    public void JumpHeld()
    {
        if (state == States.Boosting)
        {
            body.AddForce(Vector2.up * boostForce);
        }
        else if (state == States.Jumping)
        {
            body.AddForce(additionalJumpForce * Vector2.up);
        }
    }

    public void JumpReleased()
    {
        if (state == States.Boosting)
        { 
            SetPostBoostState();
        }
    }

    private void SetPostBoostState()
    {
        boostTimerTop = boostTimer;
        state = States.PostBoost;
        Jetpacks.SetActive(false);
    }
    
    private void CheckGrounded()
    {
        Collider2D collider = Physics2D.OverlapCircle(GroundCheckObj.position, groundCheckRadius, groundLayer);
        if (collider != null)
        {
            state = States.Grounded;
            boostUsed = false;
        }
    }

    private void CheckFalling()
    {
        if (body.velocity.y <= 0f)
        {
            state = States.Falling;
        }
    }

    private void AddPostBoostForce()
    {
        if (boostTimer > 0f)
        {
            boostTimer -= Time.deltaTime * 2f;
            body.AddForce(Vector2.up * boostForce * 2 * (boostTimer / boostTimerTop));
        }
    }

    private void UseBoost()
    {
        boostUsed = true;
        boostTimer = 0f;
        state = States.BoostStart;
        body.velocity = new Vector2(body.velocity.x, 0.1f);
        body.AddForce(jumpForce / 4f * Vector2.up);
        Jetpacks.SetActive(true);
    }
}

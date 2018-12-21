using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aspekt.PlayerController
{
    public class PlayerGravity : MonoBehaviour
    {
        public float FallVelocity = 15f;
        public float MaxFallVelocity = 25f;
        public float GravityFieldVelocityLerpTime = 0.5f;

        private float targetVerticalVelocity;

        private Player player;
        private Rigidbody2D body;
        private Animator anim;

        private PlayerBoundsCheck boundsCheck;

        private float groundDetectedTimer;
        private float groundSetDelay = 0.1f;
        private float timeInGravityField;

        private enum States
        {
            Grounded, Jumping, Falling
        }
        private States state;

        private void Start()
        {
            player = GetComponent<Player>();
            anim = GetComponent<Animator>();
            body = GetComponent<Rigidbody2D>();
            boundsCheck = new PlayerBoundsCheck(player.transform, body, GetComponent<Collider2D>());
        }

        public void SetTargetVelocity(float velocity)
        {
            targetVerticalVelocity = velocity;
        }

        public void ApplyNormalGravity()
        {
            body.velocity = new Vector2(body.velocity.x, body.velocity.y / 3f - FallVelocity / 3f);
            targetVerticalVelocity = -FallVelocity;
        }

        public void DetachedFromWall()
        {
            targetVerticalVelocity = -MaxFallVelocity;
        }

        public void AddVelocityInField(Vector2 velocity)
        {
            timeInGravityField = 0f;
            float fieldStrength = player.GetPlayerState().GetFloat(StateLabels.FieldStrength);
            body.velocity = new Vector2(velocity.x, velocity.y / (1 + Mathf.Abs(fieldStrength)) + fieldStrength);
        }

        private void FixedUpdate()
        {
            if (player.CheckState(StateLabels.IsInGravityField))
            {
                timeInGravityField += Time.fixedDeltaTime;
                body.velocity = new Vector2(body.velocity.x, Mathf.Lerp(body.velocity.y, player.GetPlayerState().GetFloat(StateLabels.FieldStrength), timeInGravityField / GravityFieldVelocityLerpTime));
                return;
            }
            else
            {
                timeInGravityField = 0f;
            }

            if (player.CheckState(StateLabels.IsAttachedToWall))
            {
                body.velocity = Vector2.zero;
                targetVerticalVelocity = 0;
                return;
            }
            else if (player.CheckState(StateLabels.IsAgainstWall) && !player.CheckState(StateLabels.IsBoosting))
            {
                if (body.velocity.y > targetVerticalVelocity / 3f)
                {
                    body.velocity = new Vector2(body.velocity.x, Mathf.Lerp(body.velocity.y, targetVerticalVelocity, 8f * Time.deltaTime));
                }
                else
                {
                    body.velocity = new Vector2(body.velocity.x, Mathf.Lerp(body.velocity.y, targetVerticalVelocity, 2f * Time.deltaTime));
                }
            }
            else
            {
                body.velocity = new Vector2(body.velocity.x, Mathf.Lerp(body.velocity.y, targetVerticalVelocity, 2f * Time.deltaTime));
            }
            CheckState();
            boundsCheck.CheckBounds(Time.deltaTime);
        }
        
        private void CheckState()
        {
            if (player.CheckState(StateLabels.IsGrounded))
            {
                if (state == States.Falling)
                {
                    SetGrounded();
                }
                else if (state != States.Grounded)
                {
                    groundDetectedTimer += Time.deltaTime;
                    if (groundDetectedTimer >= groundSetDelay)
                    {
                        SetGrounded();
                    }
                }
            }
            else
            {
                if (player.CheckState(StateLabels.IsJumping))
                {
                    SetJumping();
                }
                else if (state == States.Jumping || state == States.Grounded)
                {
                    targetVerticalVelocity = -FallVelocity;
                    if (body.velocity.y < 0)
                    {
                        SetFalling();
                    }
                }
            }
        }

        private void SetGrounded()
        {
            if (state == States.Grounded) return;
            state = States.Grounded;
            groundDetectedTimer = 0f;
            targetVerticalVelocity = -MaxFallVelocity;
            player.AnimationHandler.SetBool(AnimationBools.Falling, false);
            player.AnimationHandler.SetBool(AnimationBools.Grounded, true);
        }

        private void SetFalling()
        {
            if (state == States.Falling) return;
            state = States.Falling;
            groundDetectedTimer = 0f;
            targetVerticalVelocity = -MaxFallVelocity;
            player.AnimationHandler.SetBool(AnimationBools.Falling, true);
            player.AnimationHandler.SetBool(AnimationBools.Grounded, false);
        }

        private void SetJumping()
        {
            if (state == States.Jumping) return;
            state = States.Jumping;
            groundDetectedTimer = 0f;
            player.AnimationHandler.SetBool(AnimationBools.Falling, false);
            player.AnimationHandler.SetBool(AnimationBools.Grounded, false);
        }
    }
}


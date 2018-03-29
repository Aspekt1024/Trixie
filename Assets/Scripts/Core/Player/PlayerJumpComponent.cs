using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrixieCore
{
    public class PlayerJumpComponent : MonoBehaviour
    {
        public float JumpVelocity = 15f;
        public float BouncyJumpVelocity = 20f;
        public float FallVelocity = 15f;
        public float MaxFallVelocity = 25f;

        public float MaxJumpTime = 0.4f;
        public float JumpEaseOutTime = 0.4f;

        public Transform GroundCheckObj;

        private const float groundCheckRadius = 0.7f;
        private LayerMask groundLayer;

        private Rigidbody2D body;
        private Animator anim;
        private BoostComponent boostComponent;

        private float jumpTimer;
        private float timeNotGrounded;

        private float targetVerticalVelocity;

        private enum States
        {
            None, Jumping, PostJump, Boosting, PostBoost, Falling, Grounded
        }
        private States state;

        private bool inGravityField;
        private List<GravityField> gravityFields;

        private void Awake()
        {
            state = States.Falling;
            body = GetComponent<Rigidbody2D>();
            anim = GetComponent<Animator>();
            boostComponent = GetComponent<BoostComponent>();
            groundLayer = 1 << LayerMask.NameToLayer("Terrain");
            gravityFields = new List<GravityField>();
        }

        private void FixedUpdate()
        {
            if (inGravityField)
            {
                UpdateVelocity();
                return;
            }
            switch (state)
            {
                case States.Grounded:
                    boostComponent.CanRecharge = true;
                    CheckNotGrounded();
                    break;
                case States.Falling:
                    timeNotGrounded += Time.deltaTime;
                    CheckGrounded();
                    break;
                case States.Boosting:
                    boostComponent.CanRecharge = false;
                    if (boostComponent.UseBoost(Time.deltaTime))
                    {
                        // all okay! boost being used :)
                    }
                    else
                    {
                        SetPostBoostState();
                    }
                    break;
                case States.Jumping:
                    jumpTimer += Time.deltaTime;
                    if (jumpTimer > MaxJumpTime)
                    {
                        if (jumpTimer > JumpEaseOutTime + MaxJumpTime)
                        {
                            SetPostBoostState();
                        }
                        else
                        {
                            EaseOutOfJump();
                        }
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

        public void EnterGravityField(GravityField field)
        {
            inGravityField = true;
            gravityFields.Add(field);
            if (state == States.Boosting)
            {
                SetPostBoostState();
            }
            else
            {
                SetPostJumpState();
            }
            targetVerticalVelocity = field.Strength;
        }

        public void ExitGravityField(GravityField field)
        {
            gravityFields.Remove(field);
            if (gravityFields.Count > 0)
            {
                targetVerticalVelocity = gravityFields[0].Strength;
            }
            else
            {
                inGravityField = false;
                SetPostJumpState();
            }
        }

        public void Jump()
        {
            if (inGravityField) return;
            if (state == States.Falling && timeNotGrounded < 0.1f)
            {
                StartJump();
            }
            else if (state == States.Jumping || state == States.Falling || state == States.PostJump || state == States.PostBoost)
            {
                UseBoost();
            }
            else if (state == States.Grounded)
            {
                StartJump();
            }
        }

        public void JumpReleased()
        {
            if (inGravityField) return;
            if (state == States.Jumping)
            {
                SetPostJumpState();
            }
            if (state == States.Boosting)
            {
                SetPostBoostState();
            }
        }

        private void StartJump()
        {
            state = States.Jumping;
            anim.Play("Jump");
            anim.SetBool("grounded", false);
            jumpTimer = 0f;
            body.velocity = new Vector2(body.velocity.x, JumpVelocity);
            targetVerticalVelocity = (Player.Instance.IsOnBouncyGround ? BouncyJumpVelocity : JumpVelocity) * 0.5f;
        }

        private void EaseOutOfJump()
        {
            targetVerticalVelocity = -FallVelocity;
        }

        private void SetPostJumpState()
        {
            body.velocity = new Vector2(body.velocity.x, body.velocity.y / 3f - FallVelocity / 3f);
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
            boostComponent.DeactivateBoosters();
            anim.SetBool("jetpacking", false);
        }

        private void UpdateVelocity()
        {
            body.velocity = new Vector2(body.velocity.x, Mathf.Lerp(body.velocity.y, targetVerticalVelocity, 2f * Time.deltaTime));
        }

        public void SetGrounded()
        {
            state = States.Grounded;
            anim.SetBool("grounded", true);
        }

        private void CheckGrounded()
        {
            if (IsGrounded())
            {
                state = States.Grounded;
                anim.SetBool("grounded", true);
            }
        }

        private void CheckNotGrounded()
        {
            if (!IsGrounded())
            {
                if (body.velocity.y > 0f)
                {
                    state = States.Jumping;
                }
                else
                {
                    timeNotGrounded = 0f;
                    state = States.Falling;
                }
                anim.SetBool("grounded", false);
            }
        }

        private bool IsGrounded()
        {
            return Player.Instance.IsGrounded;
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
            targetVerticalVelocity = boostComponent.BoostVelocity;
            anim.SetBool("jetpacking", true);
            //Jetpacks.SetActive(true);
        }
    }
}
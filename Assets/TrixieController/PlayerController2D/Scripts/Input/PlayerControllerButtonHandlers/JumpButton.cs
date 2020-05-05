using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aspekt.PlayerController
{
    public class JumpButton : PlayerControllerButtonHandler
    {
        private float lateButtonGrace = 0.04f;
        private float earlyButtonGrace = 0.1f;
        private float wallDetachGrace = 0.1f;
        private float doubleJumpDelay = 0.05f;

        private bool jumpPressed;
        private float timeJumpPressed;
        private float timeNotGrounded;
        private bool checkDoubleJump;
        private float timeDetachedFromWall;

        private PlayerJumpComponent jumpComponent;
        private WallJumpAbility wallJump;
        private AirBoostAbility airBoost;

        private bool isGrounded;
        private bool stomped;
        private bool isAttachedToWall;

        public JumpButton(Player player, IO.PlayerController controller) : base(player, controller)
        {
            jumpComponent = player.GetAbility<PlayerJumpComponent>();
            wallJump = player.GetAbility<WallJumpAbility>();
            airBoost = player.GetAbility<AirBoostAbility>();
        }

        public override void Tick(float deltaTime)
        {
            if (player.CheckState(StateLabels.IsTouchingCeiling) && !player.CheckState(StateLabels.IsBoosting))
            {
                Released();
                return;
            }
            
            if (player.CheckState(StateLabels.IsStomping) && player.CheckState(StateLabels.IsInGravityField))
            {
                jumpComponent.Stop();
            }

            if (!isAttachedToWall && player.CheckState(StateLabels.IsAttachedToWall))
            {
                SetAttachedToWall();
                return;
            }
            else if (isAttachedToWall && !player.CheckState(StateLabels.IsAttachedToWall))
            {
                timeDetachedFromWall = Time.time;
                isAttachedToWall = false;
            }

            if (player.CheckState(StateLabels.IsGrounded))
            {
                SetGrounded();
            }
            else
            {
                SetNotGrounded();
            }

            if (checkDoubleJump)
            {
                CheckDoubleJump();
            }
        }

        public void StompPressed()
        {
            if (player.CheckState(StateLabels.IsStomping) || isGrounded || isAttachedToWall || player.CheckState(StateLabels.IsInGravityField)) return;
            jumpComponent.Stomp();
        }

        public override void Pressed()
        {
            if (player.CheckState(StateLabels.IsStomping)) return;

            jumpPressed = true;
            timeJumpPressed = Time.time;
            checkDoubleJump = false;
            
            if (isGrounded)
            {
                jumpComponent.Jump();
            }
            else if (isAttachedToWall)
            {
                wallJump.JumpFromWall();
                jumpComponent.Jump();
            }
            else
            {
                if (player.CheckState(StateLabels.IsInGravityField))
                {
                    jumpComponent.Jump();
                }
                else if (controller.GetMoveDirection().y < -0.5f && player.HasTrait(PlayerTraits.Traits.CanStomp))
                {
                    stomped = true;
                    jumpComponent.Stomp();
                }
                else if (Time.time < timeNotGrounded + lateButtonGrace)
                {
                    jumpComponent.Jump();
                }
                else if (Time.time < timeDetachedFromWall + wallDetachGrace)
                {
                    jumpComponent.Jump();
                    wallJump.JumpFromWall();
                }
                else
                {
                    checkDoubleJump = true;
                }
            }
        }

        public override void Released()
        {
            if (player.CheckState(StateLabels.IsBoosting))
            {
                airBoost.StopBoost();
            }
            else
            {
                jumpComponent.JumpReleased();
            }
            
            jumpPressed = false;
            timeJumpPressed = Time.time + earlyButtonGrace + 1f;
        }

        private void SetGrounded()
        {
            checkDoubleJump = false;
            if (isGrounded) return;
            
            isGrounded = true;
            jumpComponent.Grounded();

            if (stomped)
            {
                stomped = false;
            }
            else if (jumpPressed && Time.time < timeJumpPressed + earlyButtonGrace)
            {
                jumpComponent.Jump();
            }
        }
        
        private void SetAttachedToWall()
        {
            jumpComponent.Stop();
            isAttachedToWall = true;
            isGrounded = true;

            if (jumpPressed && Time.time < timeJumpPressed + earlyButtonGrace)
            {
                jumpComponent.Jump();
                wallJump.JumpFromWall();
            }
        }

        private void SetNotGrounded()
        {
            if (!isGrounded) return;
            isGrounded = false;
            timeNotGrounded = Time.time;
        }
        
        private void CheckDoubleJump()
        {
            if (Time.time > timeJumpPressed + doubleJumpDelay)
            {
                checkDoubleJump = false;

                if (jumpComponent.CanDoubleJump)
                {
                    jumpComponent.DoubleJump();
                }
                else
                {
                    airBoost.StartBoost();
                }
            }
        }
    }
}

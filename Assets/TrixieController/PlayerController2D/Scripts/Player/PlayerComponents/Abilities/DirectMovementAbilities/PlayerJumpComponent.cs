using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Aspekt.TestObjects;

namespace Aspekt.PlayerController
{
    public class PlayerJumpComponent : PlayerAbility
    {
        public float JumpVelocity = 15f;
        public float AirTime = 0.4f;
        
        private float jumpTimer;
        private bool doubleJumped;

        private Player player;
        private Rigidbody2D body;
        private PlayerGravity gravity;
        
        private enum States
        {
            None, JumpMode, OnGround, StompMode
        }
        private States state;

        private void Awake()
        {
            state = States.None;
            player = GetComponentInParent<Player>();
            body = player.GetComponent<Rigidbody2D>();
            gravity = player.GetComponent<PlayerGravity>();
        }
        
        private void FixedUpdate()
        {
            if (player.CheckState(StateLabels.IsKnockedBack) && state != States.None)
            {
                JumpReleased();
                state = States.None;
                return;
            }

            if (state == States.JumpMode)
            {
                CheckJumpFinished();
            }
        }
        
        private void CheckJumpFinished()
        {
            jumpTimer += Time.deltaTime;
            if (jumpTimer > AirTime)
            {
                state = States.None;
                player.SetState(StateLabels.IsJumping, false);
            }
        }
        
        public void Jump()
        {
            if (player.CheckState(StateLabels.IsKnockedBack)) return;
            if (!player.HasTrait(PlayerTraits.Traits.CanJump)) return;

            if (state == States.StompMode) return;
            
            player.SetState(StateLabels.IsJumping, true);
            player.SetState(StateLabels.IsStomping, false);
            state = States.JumpMode;
            player.AnimationHandler.Play(AnimationNames.Jump);
            jumpTimer = 0f;
            
            float velocity = JumpVelocity * player.GetPlayerState().GetFloat(StateLabels.TerrainBounciness);

            if (player.CheckState(StateLabels.IsInGravityField))
            {
                gravity.AddVelocityInField(new Vector2(body.velocity.x, velocity));
            }
            else
            {
                body.velocity = new Vector2(body.velocity.x, velocity);
                gravity.SetTargetVelocity(velocity * 0.5f);
            }
        }

        public void Stop()
        {
            state = States.None;
            player.SetState(StateLabels.IsJumping, false);
            player.SetState(StateLabels.IsStomping, false);
        }

        public void Stomp()
        {
            state = States.StompMode;
            player.SetState(StateLabels.IsJumping, false);
            player.SetState(StateLabels.IsStomping, true);
            doubleJumped = true;
            body.velocity = new Vector2(body.velocity.x, -40f);
            gravity.SetTargetVelocity(-40f);
        }

        public void DoubleJump()
        {
            if (doubleJumped || !player.HasTrait(PlayerTraits.Traits.CanDoubleJump)) return;

            player.SetState(StateLabels.TerrainBounciness, 1f);
            doubleJumped = true;
            Jump();
            player.GetEffect<DoubleJumpEffect>().Play();
        }

        public void JumpReleased()
        {
            if (state == States.StompMode) return;

            if (state == States.JumpMode)
            {
                player.SetState(StateLabels.IsJumping, false);
                gravity.ApplyNormalGravity();
            }
            state = States.None;
        }

        public void Grounded()
        {
            if (state == States.StompMode)
            {
                player.SetState(StateLabels.IsJumping, false);
                player.SetState(StateLabels.IsStomping, false);
                player.GetEffect<DoubleJumpEffect>().Play();
            }
            else if (state != States.None)
            {
                return;
            }
            
            state = States.OnGround;
            doubleJumped = false;
        }

        public bool CanDoubleJump { get { return doubleJumped == false && player.HasTrait(PlayerTraits.Traits.CanDoubleJump); } }

        private bool IsGrounded()
        {
            return player.CheckState(StateLabels.IsGrounded);
        }
    }
}
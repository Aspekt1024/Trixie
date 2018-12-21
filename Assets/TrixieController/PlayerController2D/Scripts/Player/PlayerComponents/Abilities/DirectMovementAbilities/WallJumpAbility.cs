using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aspekt.PlayerController
{
    public class WallJumpAbility : PlayerAbility
    {
        public float WallAttachDuration = 0.5f;
        public float AttachFromGroundDelay = 0.3f;

        private enum States
        {
            None, OnWall
        }
        private States state;

        private float wallAttachTime;
        private bool canAttachToWall;
        private float leftGroundTime;

        private Player player;
        private Rigidbody2D body;
        private PlayerGravity gravity;

        private MoveComponent move;
        private IO.PlayerController playerController;

        private WallAttachEffect attachEffect;

        private float wallDirection;

        private void Start()
        {
            player = Player.Instance;
            body = player.GetComponent<Rigidbody2D>();
            gravity = player.GetComponent<PlayerGravity>();
            state = States.None;
            attachEffect = player.GetEffect<WallAttachEffect>();
            canAttachToWall = true;

            move = player.GetAbility<MoveComponent>();
            playerController = IO.PlayerController.Get();
        }
        
        private void FixedUpdate()
        {
            if (!player.HasTrait(PlayerTraits.Traits.CanWallJump) || player.CheckState(StateLabels.IsInGravityField)) return;

            if (!canAttachToWall && player.CheckState(StateLabels.IsJumping))
            {
                canAttachToWall = true;
            }
            if (player.CheckState(StateLabels.IsGrounded))
            {
                leftGroundTime = Time.time;
            }

            if (state == States.OnWall)
            {
                if (Time.time > wallAttachTime + WallAttachDuration || !player.CheckState(StateLabels.IsAgainstWall))
                {
                    player.GetPlayerState().Set(StateLabels.IsAttachedToWall, false);
                    state = States.None;
                    canAttachToWall = false;

                    if (!player.CheckState(StateLabels.IsJumping))
                    {
                        gravity.ApplyNormalGravity();
                    }
                }
            }

            if (state == States.None && !player.CheckState(StateLabels.IsGrounded))
            {
                if (canAttachToWall && player.CheckState(StateLabels.IsAgainstWall) && !player.CheckState(StateLabels.IsStomping)
                    && Mathf.Abs(playerController.GetMoveDirection().x) > 0.1f && Time.time > leftGroundTime + AttachFromGroundDelay)
                {
                    state = States.OnWall;
                    wallDirection = player.IsFacingRight() ? 1 : -1;
                    player.GetPlayerState().Set(StateLabels.IsAttachedToWall, true);
                    wallAttachTime = Time.time;

                    RaycastHit2D hit = Physics2D.Raycast(player.transform.position, Vector2.right * wallDirection, 3f, 1 << LayerMask.NameToLayer("Terrain"));
                    if (hit.collider != null)
                    {
                        attachEffect.transform.position = hit.point;
                    }
                    attachEffect.Play();
                }
            }
        }
        
        public void JumpFromWall()
        {
            player.SetState(StateLabels.IsAttachedToWall, false);
            move.PropelJump(-wallDirection);
        }
    }
}


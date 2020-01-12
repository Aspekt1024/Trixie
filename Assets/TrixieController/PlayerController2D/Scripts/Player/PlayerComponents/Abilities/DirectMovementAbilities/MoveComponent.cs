﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aspekt.PlayerController
{
    public class MoveComponent : PlayerAbility
    {
        public float MoveSpeed = 10f;
        public float Acceleration = 8f;

        private Player player;
        private Rigidbody2D body;

        private float targetSpeed;
        private float timeSinceSpeedChange;
        private const float timeToChange = 0.3f;
        
        private float forceMoveTimer;
        private bool propelFromWall;

        public bool IsMoving => Mathf.Abs(targetSpeed) > 0f;
        
        private void Start()
        {
            player = GetComponentInParent<Player>();
            body = player.GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            if (player.CheckState(StateLabels.IsOnMovingPlatform) && !player.CheckState(StateLabels.IsJumping))
            {
                float extent = body.GetComponent<Collider2D>().bounds.extents.y;
                RaycastHit2D hit = Physics2D.Raycast(player.transform.position, Vector2.down, extent + 0.2f, 1 << LayerMask.NameToLayer("Terrain"));
                if (hit.collider != null)
                {
                    Vector3 pos = body.position;
                    pos.y = hit.point.y + extent;
                    pos.x += player.GetPlayerState().GetVector2(StateLabels.PlatformVelocity).x * Time.deltaTime;
                    body.transform.position = pos;
                }
            }
        }

        private void FixedUpdate()
        {
            if (player.IsIncapacitated)
            {
                MoveReleased();
            }

            player.AnimationHandler.SetFloat(AnimationFloats.MoveSpeed, Mathf.Abs(body.velocity.x));

            if (forceMoveTimer > 0)
            {
                forceMoveTimer -= Time.fixedDeltaTime;
                return;
            }
            else if (player.CheckState(StateLabels.IsKnockedBack))
            {
                return;
            }
            else if (propelFromWall)
            {
                if (player.CheckState(StateLabels.IsGrounded))
                {
                    propelFromWall = false;
                }
                else
                {
                    body.velocity = new Vector2(Mathf.Lerp(body.velocity.x, 0, Time.fixedDeltaTime * Acceleration), body.velocity.y);
                    return;
                }
            }
            
            timeSinceSpeedChange += Time.fixedDeltaTime * Acceleration;
            
            if (player.CheckState(StateLabels.IsAttachedToWall) && !player.CheckState(StateLabels.IsGrounded))
            {
                body.velocity = new Vector2(Mathf.Lerp(body.velocity.x, 0, Time.fixedDeltaTime * Acceleration), body.velocity.y);
            }
            else
            {
                // Normal move logic
                var xVelocity = Mathf.Lerp(body.velocity.x, targetSpeed, timeSinceSpeedChange / timeToChange);
                body.velocity = new Vector2(xVelocity, body.velocity.y);
            }

            float slopeGradient = player.GetPlayerState().GetFloat(StateLabels.SlopeGradient);
            if (player.CheckState(StateLabels.IsGrounded) && targetSpeed == 0f)
            {
                if (Mathf.Abs(slopeGradient) < 0.5f && Mathf.Abs(slopeGradient) > 0.05f)
                {
                    body.velocity = new Vector2(body.velocity.x - slopeGradient, body.velocity.y + Mathf.Abs(slopeGradient));
                }

                if (timeSinceSpeedChange > timeToChange)
                {
                    body.velocity = Vector2.zero;
                }
            }

            if (player.CheckState(StateLabels.IsOnSlope) && targetSpeed > 0f)
            {
                if (Mathf.Abs(slopeGradient) > 0.5f)
                {
                    body.velocity = new Vector2(Mathf.Lerp(body.velocity.x, 0, Time.fixedDeltaTime * Acceleration), Mathf.Lerp(body.velocity.y, -15, Time.fixedDeltaTime * Acceleration));
                }
            }
        }

        public void PropelJump(float direction)
        {
            propelFromWall = true;
            forceMoveTimer = 0.1f;
            body.velocity = new Vector2(direction * MoveSpeed, body.velocity.y);
        }

        public void MoveRight()
        {
            SetMoving();
            targetSpeed = MoveSpeed;
            player.FaceDirection(1);
        }

        public void MoveLeft()
        {
            SetMoving();
            targetSpeed = -MoveSpeed;
            player.FaceDirection(-1);
        }

        public void MoveReleased()
        {
            player.AnimationHandler.SetBool(AnimationBools.IsRunning, false);
            timeSinceSpeedChange = 0f;
            targetSpeed = 0f;
        }

        private void SetMoving()
        {
            if (forceMoveTimer <= 0 && propelFromWall)
            {
                propelFromWall = false;
            }
            player.AnimationHandler.SetBool(AnimationBools.IsRunning, true);
            timeSinceSpeedChange = 0f;
        }
    }
}
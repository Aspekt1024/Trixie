using System.Collections;
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
        private const float slopeClimbMax = 0.7f;
        
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
            var xVelocity = Mathf.Lerp(body.velocity.x, targetSpeed, timeSinceSpeedChange / timeToChange);

            if (player.CheckState(StateLabels.IsAttachedToWall) && !player.CheckState(StateLabels.IsGrounded))
            {
                body.velocity = new Vector2(Mathf.Lerp(body.velocity.x, 0, Time.fixedDeltaTime * Acceleration), body.velocity.y);
            }
            else
            {
                // Normal move logic
                body.velocity = new Vector2(xVelocity, body.velocity.y);
            }

            // Slope move logic
            float slopeGradient = player.GetPlayerState().GetFloat(StateLabels.SlopeGradient);
            if (Mathf.Abs(slopeGradient) < slopeClimbMax && player.CheckState(StateLabels.IsGrounded) && !player.CheckState(StateLabels.IsJumping))
            {
                // We are allowed to climb.

                // Determine whether or not we are climbing.
                bool climbing = xVelocity < 0 && slopeGradient > 0 || xVelocity > 0 && slopeGradient < 0;

                // Find the slope angle and how far we should move along it.
                var layerMask = 1 << 8;
                RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up, Mathf.Infinity, layerMask);
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                float slopeMoveDist = Mathf.Abs(xVelocity);
                float yDist = slopeMoveDist * Mathf.Sin(Mathf.Deg2Rad * slopeAngle);
                float xDist = slopeMoveDist * Mathf.Cos(Mathf.Deg2Rad * slopeAngle) * Mathf.Sign(xVelocity);

                if (!climbing)
                {
                    // We are descending.
                    yDist = -yDist;
                }

                body.velocity = new Vector2(xDist, yDist);
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
            // Maybe want to play an idle animation here.

            timeSinceSpeedChange = 0f;
            targetSpeed = 0f;
        }

        private void SetMoving()
        {
            if (forceMoveTimer <= 0 && propelFromWall)
            {
                propelFromWall = false;
            }
            player.AnimationHandler.SetFacing(Mathf.Sign(targetSpeed));
            player.AnimationHandler.SetAnimation("Run");

            timeSinceSpeedChange = 0f;
        }
    }
}
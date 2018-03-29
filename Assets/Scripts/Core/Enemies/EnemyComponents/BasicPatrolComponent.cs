using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrixieCore.Units
{

    public class BasicPatrolComponent : MonoBehaviour
    {

        public float WaitTimeAtEdge = 0.3f;
        public float MoveSpeed = 7f;
        public float Acceleration = 3f;
        public bool SpriteFacesLeft = false;

        private Guy2 enemyScript;
        private CapsuleCollider2D capsuleCollider;
        private bool[] groundedResults;
        private bool[] wallResults;

        private enum States
        {
            None, Moving, SwitchingDirection, StartMoving
        }
        private States state;

        private enum MoveDirections
        {
            Left, Right
        }
        private MoveDirections currentDirection;

        private float targetVelocity;
        private Rigidbody2D body;
        private SpriteRenderer sr;
        private Vector2 originalScale;
        private float waitTime;

        private void Awake()
        {
            currentDirection = MoveDirections.Right;
            state = States.None;
            body = GetComponent<Rigidbody2D>();
            sr = GetComponent<SpriteRenderer>();
            originalScale = transform.localScale;

            enemyScript = GetComponent<Guy2>();
            capsuleCollider = GetComponent<CapsuleCollider2D>();
        }

        public void Activate()
        {
            if (state != States.None) return;

            state = States.Moving;
            targetVelocity = MoveSpeed;
            SetLookDirection();
        }

        public void Deactivate()
        {
            targetVelocity = 0f;
            state = States.None;
        }

        public void DeactivateImmediate()
        {
            Deactivate();
            body.velocity = Vector2.zero;
        }

        private void Update()
        {


            switch (state)
            {
                case States.None:
                    break;
                case States.Moving:
                    UpdateVelocity();
                    UpdateGroundedResults();
                    UpdateWallResults();

                    if (IsAgainstWall() || !NotAtEdge())
                    {
                        TurnAround();
                    }
                    break;
                case States.SwitchingDirection:
                    waitTime += Time.deltaTime;
                    if (waitTime >= WaitTimeAtEdge)
                    {
                        SwitchDirection();
                        waitTime = 0;
                        state = States.StartMoving;
                        targetVelocity = MoveSpeed;
                    }
                    break;
                case States.StartMoving:
                    waitTime += Time.deltaTime;
                    if (waitTime >= WaitTimeAtEdge)
                    {
                        state = States.Moving;
                    }
                    break;
                default:
                    break;
            }
        }

        public void TurnAround()
        {
            if (state == States.SwitchingDirection) return;
            waitTime = 0f;
            targetVelocity = 0f;
            state = States.SwitchingDirection;
        }

        private void UpdateVelocity()
        {
            float velocityDireciton = currentDirection == MoveDirections.Left ? -1f : 1f;
            body.velocity = new Vector2(Mathf.Lerp(body.velocity.x, targetVelocity * velocityDireciton, Time.deltaTime * Acceleration), body.velocity.y);
        }

        private void SwitchDirection()
        {
            if (currentDirection == MoveDirections.Left)
            {
                currentDirection = MoveDirections.Right;
            }
            else
            {
                currentDirection = MoveDirections.Left;
            }
            SetLookDirection();
        }

        private void SetLookDirection()
        {
            float lookDirectionScaleModifier = 1f;
            if (currentDirection == MoveDirections.Right && SpriteFacesLeft)
            {
                lookDirectionScaleModifier = -1f;
            }
            else if (currentDirection == MoveDirections.Left && !SpriteFacesLeft)
            {
                lookDirectionScaleModifier = -1f;
            }
            transform.localScale = new Vector3(lookDirectionScaleModifier * originalScale.x, originalScale.y, 1f);
        }


        private void UpdateGroundedResults()
        {
            float xScale = Mathf.Abs(enemyScript.transform.localScale.x);
            float yScale = enemyScript.transform.localScale.y;


            float yPos = transform.position.y + capsuleCollider.offset.y * yScale;
            float yDist = capsuleCollider.size.y / 2f * yScale + .2f;

            Vector2[] points = new Vector2[]
            {
                new Vector2(capsuleCollider.transform.position.x + (capsuleCollider.offset.x - capsuleCollider.size.x / 2f) * xScale - 3f, yPos - yDist - 1f),
                new Vector2(capsuleCollider.transform.position.x + capsuleCollider.offset.x * xScale, yPos - yDist),
                new Vector2(capsuleCollider.transform.position.x + (capsuleCollider.offset.x + capsuleCollider.size.x / 2f) * xScale + 3f, yPos - yDist - 1f)
            };

            groundedResults = new bool[3];

            for (int i = 0; i < points.Length; i++)
            {
                RaycastHit2D hit = Physics2D.Raycast(new Vector2(points[i].x, transform.position.y), Vector2.down, transform.position.y - points[i].y, 1 << TrixieLayers.GetMask(Layers.Terrain));
                groundedResults[i] = hit.collider != null;
            }
        }

        private bool IsGrounded()
        {
            return groundedResults[1];
        }

        private bool NotAtEdge()
        {
            if (enemyScript.GetHorizontalVelocity() < 0)
            {
                return groundedResults[0];
            }
            else
            {
                return groundedResults[2];
            }
        }

        private void UpdateWallResults()
        {
            wallResults = new bool[3];

            float xScale = Mathf.Abs(enemyScript.transform.localScale.x);
            float yScale = enemyScript.transform.localScale.y;

            float dirMod = Mathf.Sign(enemyScript.GetHorizontalVelocity());

            Vector2 capBottom = (Vector2)capsuleCollider.transform.position + new Vector2(capsuleCollider.offset.x * xScale, (capsuleCollider.offset.y - capsuleCollider.size.y / 2f) * yScale);
            float capHeight = capsuleCollider.size.y * yScale;

            float xDist = capsuleCollider.size.x / 2 * xScale + 0.2f;

            float[] yPos = new float[]
            {
                capBottom.y + capHeight * 0.14f,
                capBottom.y + capHeight * 0.5f,
                capBottom.y + capHeight * 0.8f,
            };

            for (int i = 0; i < yPos.Length; i++)
            {
                RaycastHit2D hit = Physics2D.Raycast(new Vector2(capBottom.x, yPos[i]), dirMod * Vector2.right, xDist, 1 << TrixieLayers.GetMask(Layers.Terrain));
                wallResults[i] = hit.collider != null;
            }
        }

        private bool IsAgainstWall()
        {
            return wallResults[0] || wallResults[1] || wallResults[2];
        }
    }
}


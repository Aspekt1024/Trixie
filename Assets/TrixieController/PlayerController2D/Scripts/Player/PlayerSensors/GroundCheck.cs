using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aspekt.PlayerController
{
    public class GroundCheck : PlayerSensor
    {
        private Collider2D coll;
        private PlayerState playerState;

        private RaycastHit2D hit;
        private const int NUM_POINTS = 5;
        private readonly bool[] groundedPoints = new bool[NUM_POINTS];
        private readonly float[] checkPoints = new float[NUM_POINTS];

        private void Start()
        {
            Player player = GetComponentInParent<Player>();
            coll = player.GetComponent<Collider2D>();
            playerState = player.GetPlayerState();
            for (int i = 0; i < NUM_POINTS; i++)
            {
                checkPoints[i] = -1f + 2f * (i / (NUM_POINTS - 1f));
            }

            playerState.Set(StateLabels.TerrainBounciness, 1f);
        }

        private void FixedUpdate()
        {
            bool isGrounded = false;
            playerState.Set(StateLabels.IsOnMovingPlatform, false);
            for (int i = 0; i < NUM_POINTS; i++)
            {
                groundedPoints[i] = GroundHit(coll.transform.position + Vector3.right * coll.bounds.extents.x * checkPoints[i]);
                if (groundedPoints[i])
                {
                    isGrounded = true;
                }
            }

            playerState.Set(StateLabels.IsGrounded, isGrounded);
            playerState.Set(StateLabels.IsAtEdge, groundedPoints[0] == false || groundedPoints[NUM_POINTS - 1] == false);
        }

        public bool IsAtEdge
        {
            get { return groundedPoints[0] == false || groundedPoints[NUM_POINTS - 1] == false; }
        }

        private bool GroundHit(Vector2 origin)
        {
            playerState.Set(StateLabels.IsOnSlope, false);
            hit = Physics2D.Raycast(origin, Vector2.down, coll.bounds.extents.y + 0.2f, 1 << LayerMask.NameToLayer("Terrain"));
            if (hit.collider != null)
            {
                playerState.Set(StateLabels.SlopeGradient, hit.normal.x);
                if (Mathf.Abs(hit.normal.y) < 0.5f)
                {
                    playerState.Set(StateLabels.IsOnSlope, true);
                    return false;
                }

                TestObjects.BouncyObject bouncyObject = hit.collider.GetComponent<TestObjects.BouncyObject>();
                if (bouncyObject != null)
                {
                    StartCoroutine(SetBounciness(bouncyObject.Bounciness));
                }
                else
                {
                    MovingPlatform movingPlatform = hit.collider.GetComponent<MovingPlatform>();
                    if (movingPlatform != null)
                    {
                        playerState.Set(StateLabels.IsOnMovingPlatform, true);
                        playerState.Set(StateLabels.PlatformVelocity, movingPlatform.GetVelocity());
                    }
                }
            }

            return playerState.Check(StateLabels.IsOnMovingPlatform) || hit.collider != null && hit.point.y < origin.y - coll.bounds.extents.y * 0.9f;
        }

        private IEnumerator SetBounciness(float bounciness)
        {
            float bouncinessTimer = 0f;
            const float bouncinessDuration = 0.4f;

            playerState.Set(StateLabels.TerrainBounciness, bounciness);

            while (bouncinessTimer < bouncinessDuration)
            {
                bouncinessTimer += Time.fixedDeltaTime;
                yield return null;
            }

            playerState.Set(StateLabels.TerrainBounciness, 1f);
        }
    }

}
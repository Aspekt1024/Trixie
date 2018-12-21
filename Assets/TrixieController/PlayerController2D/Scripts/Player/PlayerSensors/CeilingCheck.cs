using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aspekt.PlayerController
{
    public class CeilingCheck : PlayerSensor
    {
        private Collider2D coll;
        private PlayerState playerState;

        private RaycastHit2D hit;
        private const int NUM_POINTS = 5;
        private bool[] groundedPoints = new bool[NUM_POINTS];
        private float[] checkPoints = new float[NUM_POINTS];

        private void Start()
        {
            Player player = GetComponentInParent<Player>();
            coll = player.GetComponent<Collider2D>();
            playerState = player.GetPlayerState();
            for (int i = 0; i < NUM_POINTS; i++)
            {
                checkPoints[i] = -0.8f + 1.6f * (i / (NUM_POINTS - 1f));
            }
        }

        private void FixedUpdate()
        {
            for (int i = 0; i < NUM_POINTS; i++)
            {
                groundedPoints[i] = TouchingCeiling(coll.transform.position + Vector3.right * coll.bounds.extents.x * checkPoints[i]);
                if (groundedPoints[i])
                {
                    playerState.Set(StateLabels.IsTouchingCeiling, true);
                    return;
                }
            }
            playerState.Set(StateLabels.IsTouchingCeiling, false);
        }

        private bool TouchingCeiling(Vector2 origin)
        {
            hit = Physics2D.Raycast(origin, Vector2.up, coll.bounds.extents.y + 0.1f, 1 << LayerMask.NameToLayer("Terrain"));
            if (hit.collider != null)
            {
                if (Mathf.Abs(hit.normal.y) < 0.5f)
                {
                    return false;
                }
            }

            return hit.collider != null && hit.point.y > origin.y + coll.bounds.extents.y * 0.9f;
        }
    }
}



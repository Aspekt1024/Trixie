using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aspekt.PlayerController
{
    public class WallCheck : PlayerSensor
    {
        private PlayerState playerState;
        private Collider2D coll;
        private Player player;
        
        private const int NUM_POINTS = 3;
        private bool[] wallChecks = new bool[NUM_POINTS];
        private float[] verticalPoints = new float[NUM_POINTS];

        private void Start()
        {
            player = GetComponentInParent<Player>();
            playerState = player.GetPlayerState();
            coll = player.GetComponent<Collider2D>();
            
            for (int i = 0; i < NUM_POINTS; i++)
            {
                verticalPoints[i] = -1f + 2f * (i / (NUM_POINTS - 1f));
            }
        }

        private void Update()
        {
            bool isTouchingWall = true;
            for (int i = 0; i < NUM_POINTS; i++)
            {
                wallChecks[i] = WallHit(coll.transform.position + Vector3.up * coll.bounds.extents.y * verticalPoints[i]);
                if (wallChecks[i] == false)
                {
                    isTouchingWall = false;
                    break;
                }
            }
            playerState.Set(StateLabels.IsAgainstWall, isTouchingWall);
        }

        private bool WallHit(Vector2 origin)
        {
            float direction = player.IsFacingRight() ? 1 : -1;
            RaycastHit2D hit = Physics2D.Raycast(
                            coll.transform.position,
                            Vector2.right * direction,
                            coll.bounds.extents.x + 0.02f, 
                            1 << LayerMask.NameToLayer("Terrain"));

            return hit.collider != null;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Checks if Trixie is touching the ground
/// </summary>
public class GroundCheck : MonoBehaviour {

    public BoxCollider2D LeftGroundTrigger;
    public BoxCollider2D MidGroundTrigger;
    public BoxCollider2D RightGroundTrigger;
    
    private void FixedUpdate()
    {
        int terrainLayer = 1 << LayerMask.NameToLayer("Terrain");
        RaycastHit2D hitResultsLeft = Physics2D.BoxCast(LeftGroundTrigger.transform.position, LeftGroundTrigger.bounds.size, 0f, Vector3.forward, Mathf.Infinity, terrainLayer);
        RaycastHit2D hitResultsRight = Physics2D.BoxCast(RightGroundTrigger.transform.position, RightGroundTrigger.bounds.size, 0f, Vector3.forward, Mathf.Infinity, terrainLayer);
        RaycastHit2D hitResultsMid = Physics2D.BoxCast(MidGroundTrigger.transform.position, MidGroundTrigger.bounds.size, 0f, Vector3.forward, Mathf.Infinity, terrainLayer);

        Player.Instance.IsGrounded = false;
        Player.Instance.IsAgainstWall = false;

        CheckGround(hitResultsLeft);
        CheckGround(hitResultsRight);
        CheckGround(hitResultsMid);
    }

    private void CheckGround(RaycastHit2D hit)
    {
        if (hit.collider == null) return;

        if (Mathf.Abs(hit.normal.x) < 0.8f)
        {
            Player.Instance.IsGrounded = true;
        }
        else
        {
            Player.Instance.IsAgainstWall = true;
        }
    }
}

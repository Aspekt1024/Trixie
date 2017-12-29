using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveComponent : MonoBehaviour {

    public float MoveSpeed = 10f;
    public float Acceleration = 8f;

    private Rigidbody2D body;
    private Animator playerAnim;
    private SpriteRenderer playerRenderer;

    private float targetSpeed;
    private float timeSinceSpeedChange;
    private const float timeToChange = 0.3f;
    
    private void Start ()
    {
        body = GetComponent<Rigidbody2D>();
        playerAnim = GetComponent<Animator>();
        playerRenderer = Player.Instance.GetPlayerRenderer();
	}

    private void FixedUpdate()
    {
        playerAnim.SetFloat("MoveSpeed", Mathf.Abs(body.velocity.x));

        timeSinceSpeedChange += Time.fixedDeltaTime * Acceleration;
        if (Player.Instance.IsAgainstWall)
        {
            body.velocity = new Vector2(Mathf.Lerp(body.velocity.x, targetSpeed, Time.fixedDeltaTime * Acceleration), body.velocity.y);
        }
        else
        {
            body.velocity = new Vector2(Mathf.Lerp(body.velocity.x, targetSpeed, timeSinceSpeedChange / timeToChange), body.velocity.y);
        }

        if (Player.Instance.IsGrounded && targetSpeed == 0f)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up, 4f, 1 << LayerMask.NameToLayer("Terrain"));
            if (hit.collider != null && !Player.Instance.IsAgainstWall)
            {
                body.velocity = new Vector2(body.velocity.x - hit.normal.x * 1.6f, body.velocity.y);
            }
        }

    }

    public void MoveRight()
    {
        timeSinceSpeedChange = 0f;
        targetSpeed = MoveSpeed;
        playerRenderer.transform.localScale = new Vector3()
        {
            x = Mathf.Abs(playerRenderer.transform.localScale.x),
            y = playerRenderer.transform.localScale.y,
            z = playerRenderer.transform.localScale.z
        };
    }

    public void MoveLeft()
    {
        timeSinceSpeedChange = 0f;
        targetSpeed = -MoveSpeed;
        playerRenderer.transform.localScale = new Vector3()
        {
            x = -Mathf.Abs(playerRenderer.transform.localScale.x),
            y = playerRenderer.transform.localScale.y,
            z = playerRenderer.transform.localScale.z
        };
    }

    public void MoveReleased()
    {
        timeSinceSpeedChange = 0f;
        targetSpeed = 0f;
    }

    public bool IsLookingRight()
    {
        return playerRenderer.transform.localScale.x >= 0f;
    }

}

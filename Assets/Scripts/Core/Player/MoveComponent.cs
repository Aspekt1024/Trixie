using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveComponent : MonoBehaviour {

    public float MoveSpeed = 10f;

    private Rigidbody2D body;
    private Animator playerAnim;
    private SpriteRenderer playerRenderer;

    private float targetSpeed;

    private void Start ()
    {
        body = GetComponent<Rigidbody2D>();
        playerAnim = GetComponent<Animator>();
        playerRenderer = Player.Instance.GetPlayerRenderer();
	}

    private void FixedUpdate()
    {
        playerAnim.SetFloat("MoveSpeed", Mathf.Abs(body.velocity.x));
        body.velocity = new Vector2(Mathf.Lerp(body.velocity.x, targetSpeed, 8 * Time.deltaTime), body.velocity.y);
    }

    public void MoveRight()
    {
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
        targetSpeed = 0f;
    }

    public bool IsLookingRight()
    {
        return playerRenderer.transform.localScale.x >= 0f;
    }

}

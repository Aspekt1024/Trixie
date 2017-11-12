using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveComponent : MonoBehaviour {

    private Rigidbody2D body;

    private float targetSpeed;
    private const float speed = 15f;

    private void Start ()
    {
        body = GetComponent<Rigidbody2D>();
	}

    private void FixedUpdate()
    {
        body.velocity = new Vector2(Mathf.Lerp(body.velocity.x, targetSpeed, 8 * Time.deltaTime), body.velocity.y);
    }

    public void MoveRight()
    {
        targetSpeed = speed;
    }

    public void MoveLeft()
    {
        targetSpeed = -speed;
    }

    public void MoveReleased()
    {
        targetSpeed = 0f;
    }

}

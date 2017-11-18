using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveComponent : MonoBehaviour {

    public float MoveSpeed = 10f;

    private Rigidbody2D body;
    private Animator anim;

    private float targetSpeed;

    private void Start ()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
	}

    private void FixedUpdate()
    {
        anim.SetFloat("MoveSpeed", Mathf.Abs(body.velocity.x));
        body.velocity = new Vector2(Mathf.Lerp(body.velocity.x, targetSpeed, 8 * Time.deltaTime), body.velocity.y);
    }

    public void MoveRight()
    {
        targetSpeed = MoveSpeed;
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

    public void MoveLeft()
    {
        targetSpeed = -MoveSpeed;
        transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

    public void MoveReleased()
    {
        targetSpeed = 0f;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Trixie : MonoBehaviour {

    public Animator animator;
    public GameObject Jetpacks;
    public Transform GroundCheckObj;
    public Transform BulletPoint;

    private const float groundCheckRadius = 0.1f;
    private bool isGrounded;

    private LayerMask groundLayer;

    private enum States
    {
        Idle, Jumping, Jetpacking
    }
    private States _state;

    public void SetIdleState()
    {
        _state = States.Idle;
    }

    private void Start()
    {
        groundLayer = 1 << LayerMask.NameToLayer("Terrain");
    }

    private void FixedUpdate()
    {
        isGrounded = CheckGrounded();
        animator.SetBool("grounded", isGrounded);

        if (_state == States.Jetpacking)
        {
            animator.SetBool("jetpacking", true);
            GetComponent<Rigidbody2D>().velocity = Vector2.up * 4f;
        }
        else
        {
            animator.SetBool("jetpacking", false);
            Jetpacks.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            Jump();
        }
        if (Input.GetKeyUp(KeyCode.W))
        {
            EndJetpack();
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Shoot();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            Kick();
        }
    }

    private void Kick()
    {
        animator.Play("Kick", 0, 0f);
    }

    private void Jump()
    {
        if (_state == States.Idle && isGrounded)
        {
            _state = States.Jumping;
            animator.Play("Jump", 0, 0f);
            GetComponent<Rigidbody2D>().velocity = new Vector2(0f, 4f);
        }
        else if (_state == States.Jumping)
        {
            Jetpacks.SetActive(true);
            animator.Play("JetpackStart", 0, 0f);
            _state = States.Jetpacking;
        }
    }

    private void Shoot()
    {
        animator.Play("Shoot", 0, 0f);
        StartCoroutine(ShootAnim());
    }

    private IEnumerator ShootAnim()
    {
        yield return new WaitForSeconds(0.15f);
        GameObject bullet = Instantiate(Resources.Load<GameObject>("Prefabs/Bullet"));
        bullet.transform.position = BulletPoint.position;
        bullet.GetComponent<Rigidbody2D>().velocity = BulletPoint.right * 15f;
    }

    private void EndJetpack()
    {
        if (_state == States.Jetpacking)
        {
            _state = States.Jumping;
        }
    }
    
    private bool CheckGrounded()
    {
        Collider2D collider = Physics2D.OverlapCircle(GroundCheckObj.position, groundCheckRadius, groundLayer);
        if (collider != null)
        {
            return true;
        }
        return false;
    }
}

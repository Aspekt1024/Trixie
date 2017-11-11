using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    
    public GameObject Jetpacks;
    public Transform GroundCheckObj;
    public Transform BulletPoint;

    private const float speed = 15f;



    private Rigidbody2D body;

    private PlayerJumpComponent jumpComponent;
    private ShieldComponent shieldComponent;

    private enum States
    {
        Idle, Jumping
    }
    private States _state;

    // Use this for initialization
    void Start () {
        body = GetComponent<Rigidbody2D>();
        jumpComponent = GetComponent<PlayerJumpComponent>();
        shieldComponent = GetComponent<ShieldComponent>();
    }

    private void FixedUpdate()
    {
    }

    public void MoveLeft()
    {
        body.velocity = new Vector2(-speed, body.velocity.y);
    }
    public void MoveRight()
    {
        body.velocity = new Vector2(speed, body.velocity.y);
    }
    public void Jump()
    {
        jumpComponent.Jump();
    }

    public void JumpHeld() { jumpComponent.JumpHeld(); }
    public void JumpReleased() { jumpComponent.JumpReleased(); }
    public void ShieldPressed() { shieldComponent.ShieldPressed(); }
    public void ShieldReleased() { shieldComponent.ShieldReleased(); }

    public void Shoot()
    {
        shieldComponent.Shoot();
    }

    private IEnumerator ShootAnim()
    {
        yield return new WaitForSeconds(0.15f);
        GameObject bullet = Instantiate(Resources.Load<GameObject>("Prefabs/Bullet"));
        bullet.transform.position = BulletPoint.position;
        bullet.GetComponent<Rigidbody2D>().velocity = BulletPoint.right * 15f;
    }

}

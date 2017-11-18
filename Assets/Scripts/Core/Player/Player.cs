using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    
    public GameObject Jetpacks;
    public Transform GroundCheckObj;
    public Transform BulletPoint;

    public static Player Instance;

    private MoveComponent moveComponent;
    private PlayerJumpComponent jumpComponent;
    private ShieldComponent shieldComponent;

    private enum States
    {
        Idle, Jumping
    }
    private States _state;
    
    private void Awake ()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("Multiple Player classes found in scene. There can only be one");
            Destroy(gameObject);
            return;
        }

        moveComponent = GetComponent<MoveComponent>();
        jumpComponent = GetComponent<PlayerJumpComponent>();
        shieldComponent = GetComponent<ShieldComponent>();
    }

    public void MoveLeft()
    {
        moveComponent.MoveLeft();
    }
    public void MoveRight()
    {
        moveComponent.MoveRight();
    }
    public void MoveReleased()
    {
        moveComponent.MoveReleased();
    }
    public void Jump()
    {
        jumpComponent.Jump();
    }
    
    public void JumpReleased() { jumpComponent.JumpReleased(); }
    public void ShieldPressed() { shieldComponent.ShieldPressed(); }
    public void ShieldReleased() { shieldComponent.ShieldReleased(); }
    public void CycleShieldColourPressed() { shieldComponent.CycleShieldColourPressed(); }

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

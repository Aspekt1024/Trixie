using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    
    public GameObject Jetpacks;
    public Transform GroundCheckObj;
    public Transform BulletPoint;

    public static Player Instance;

    private Animator anim;

    private MoveComponent moveComponent;
    private PlayerJumpComponent jumpComponent;
    private ShieldComponent shieldComponent;
    private HealthComponent healthComponent;
    
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

        anim = GetComponent<Animator>();

        moveComponent = GetComponent<MoveComponent>();
        jumpComponent = GetComponent<PlayerJumpComponent>();
        shieldComponent = GetComponent<ShieldComponent>();
        healthComponent = GetComponent<HealthComponent>();
    }

    private void Update()
    {
        Vector2 viewportPos = Camera.main.WorldToViewportPoint(transform.position);
        if (viewportPos.y < -0.2f)
        {
            GameManager.RespawnPlayerStart();
        }
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

    public void ShieldPressed()
    {
        if (shieldComponent.ActivateShield())
        {
            anim.SetBool("shieldEnabled", true);
        }
    }
    public void ShieldReleased()
    {
        if (shieldComponent.DeactivateShield())
        {
            anim.SetBool("shieldEnabled", false);
        }
    }

    public void JumpReleased() { jumpComponent.JumpReleased(); }
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "BouncyBulb")
        {
            Rigidbody2D body = GetComponent<Rigidbody2D>();
            body.velocity = new Vector2(body.velocity.x, 30f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy" || collision.tag == "Projectile")
        {
            TakeDamage();
        }
    }

    private void TakeDamage()
    {
        healthComponent.TakeDamage();
        GameUIManager.UpdateHealth(healthComponent.GetHealth());
        if (healthComponent.IsDead())
        {
            Debug.Log("you are dead");
        }
    }
}

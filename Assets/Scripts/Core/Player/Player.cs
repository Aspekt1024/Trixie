using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    
    public GameObject Jetpacks;
    public Transform GroundCheckObj;
    public Transform BulletPoint;
    public SpriteRenderer TrixieRenderer;

    public static Player Instance;

    private Animator anim;

    private MoveComponent moveComponent;
    private PlayerJumpComponent jumpComponent;
    private ShieldComponent shieldComponent;
    private PlayerHealthComponent healthComponent;
    private MeleeComponent meleeComponent;

    private bool isGrounded;
    private bool isAgainstWall;

    public bool IsGrounded
    {
        get { return isGrounded; }
        set { isGrounded = value; }
    }

    public bool IsAgainstWall
    {
        get { return isAgainstWall; }
        set { isAgainstWall = value; }
    }
    
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
        healthComponent = GetComponent<PlayerHealthComponent>();
        meleeComponent = GetComponent<MeleeComponent>();
    }

    private void Update()
    {
        Vector2 viewportPos = Camera.main.WorldToViewportPoint(transform.position);
        if (viewportPos.y < -0.2f)
        {
            GameManager.RespawnPlayerStart();
        }
    }

    public void Melee()
    {
        if (shieldComponent.HasShield())
        {
            shieldComponent.DisableShield();
            meleeComponent.Activate();
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

    public SpriteRenderer GetPlayerRenderer()
    {
        return TrixieRenderer;
    }

    public bool IsLookingRight()
    {
        return moveComponent.IsLookingRight();
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
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy") || collision.gameObject.layer == LayerMask.NameToLayer("Projectile"))
        {
            TakeDamage();
        }
        else if (collision.collider.tag == "BouncyBulb")
        {
            Rigidbody2D body = GetComponent<Rigidbody2D>();
            body.velocity = new Vector2(body.velocity.x, 30f);
        }
        else if (collision.collider.tag == "Enemy")
        {
            TakeDamage();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "GravityField")
        {
            jumpComponent.EnterGravityField(collision.GetComponent<GravityField>());
        }
        else if (collision.tag == "CameraFocusField")
        {
            Camera.main.GetComponent<CameraFollow>().SetCameraFocus(collision.transform);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "GravityField")
        {
            jumpComponent.ExitGravityField(collision.GetComponent<GravityField>());
        }
        else if (collision.tag == "CameraFocusField")
        {
            Camera.main.GetComponent<CameraFollow>().SetCameraFollow();
        }
    }

    private void TakeDamage()
    {
        healthComponent.TakeDamage();
        if (healthComponent.IsDead())
        {
            healthComponent.AddHealth(healthComponent.MaxHealth);
            GameManager.RespawnPlayerStart();
        }
    }
}

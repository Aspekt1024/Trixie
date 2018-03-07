using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrixieCore;

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
    private RangedComponent rangedComponent;

    private bool isGrounded;
    private bool isAgainstWall;
    private bool isOnBouncyGround;

    private float stunDuration;

    private enum States
    {
        Normal, Stunned
    }
    private States state;

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

    public bool IsOnBouncyGround
    {
        get { return isOnBouncyGround; }
        set { isOnBouncyGround = value; }
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
        rangedComponent = GetComponent<RangedComponent>();
    }

    private void Update()
    {
        Vector2 viewportPos = Camera.main.WorldToViewportPoint(transform.position);
        if (viewportPos.y < -0.2f)
        {
            GameManager.RespawnPlayerStart();
        }

        if (state == States.Stunned)
        {
            if (stunDuration > 0)
            {
                stunDuration -= Time.deltaTime;
            }
            if (stunDuration <= 0)
            {
                state = States.Normal;
            }
        }
    }

    public void MeleePressed()
    {
        if (state != States.Normal) return;
        meleeComponent.MeleePressed();
        shieldComponent.Moved();
    }

    public void MeleeReleased()
    {
        if (state != States.Normal) return;
        meleeComponent.MeleeReleased();
    }

    public void RangedAttackPressed()
    {
        if (state != States.Normal) return;
        rangedComponent.RangedPressed();
        shieldComponent.Moved();
    }

    public void RangedAttackReleased ()
    {
        if (state != States.Normal) return;
        rangedComponent.RangedReleased();
    }

    public void MoveLeft()
    {
        if (state != States.Normal) return;
        moveComponent.MoveLeft();
        shieldComponent.Moved();
    }
    public void MoveRight()
    {
        if (state != States.Normal) return;
        moveComponent.MoveRight();
        shieldComponent.Moved();
    }
    public void MoveReleased()
    {
        if (state != States.Normal) return;
        moveComponent.MoveReleased();
    }
    public void Jump()
    {
        if (state != States.Normal) return;
        jumpComponent.Jump();
        shieldComponent.Moved();
    }

    public void ShieldPressed()
    {
        if (state != States.Normal) return;
        if (shieldComponent.ShieldActivatePressed())
        {
            anim.SetBool("shieldEnabled", true);
        }
    }
    public void ShieldReleased()
    {
        if (state != States.Normal) return;
        if (shieldComponent.ShieldDeactivatePressed())
        {
            anim.SetBool("shieldEnabled", false);
        }
    }

    public void JumpReleased() { if (state != States.Normal) return; jumpComponent.JumpReleased(); }
    public void CycleShieldColourPressed() { shieldComponent.CycleShieldColour(); }

    public void Damage(int damage = 1)
    {
        TakeDamage(damage);
    }

    public void ShootPressed()
    {
        if (state != States.Normal) return;
        // TODO let the shield component handle this (originally it was going to enable a basic shoot. we're not doing that anymore)
        if (shieldComponent.IsAwaitingActivation())
        {
            shieldComponent.ShootPressed();
        }
    }

    public void ShootReleased()
    {
        if (state != States.Normal) return;
        shieldComponent.ShootReleased();
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
        if (collision.collider.tag == "BouncyBulb")
        {
            Rigidbody2D body = GetComponent<Rigidbody2D>();
            body.velocity = new Vector2(body.velocity.x, 30f);
            collision.collider.GetComponent<Bulb>().Bounce();
            jumpComponent.SetGrounded();
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

    private void Stun(float duration)
    {
        state = States.Stunned;
        shieldComponent.Moved();
        stunDuration = duration;
        moveComponent.MoveReleased();
        jumpComponent.JumpReleased();
        // TODO stop movement, play animation etc.
    }

    public void HitWithObject(object obj)
    {
        if (obj.GetType().Equals(typeof(EnemyShield)))
        {
            HitWithShield((EnemyShield)obj);
        }
    }

    private void HitWithShield(EnemyShield shield)
    {
        if (shield.StunsPlayer)
        {
            if (state != States.Stunned)
            {
                Stun(shield.StunDuration);
            }
        }
        if (shield.CanDamagePlayer)
        {
            TakeDamage(shield.DamageToPlayer);
        }
    }

    private void TakeDamage(int damage = 1)
    {
        healthComponent.TakeDamage(damage);
        if (healthComponent.IsDead())
        {
            healthComponent.AddHealth(healthComponent.MaxHealth);
            GameManager.RespawnPlayerStart();
        }
    }
}

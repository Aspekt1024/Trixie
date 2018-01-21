using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootTestEnemy : BaseEnemy {

    public float ShootCooldown = 1f;
    public Transform Turrets;
    public GameObject ExplosionEffect;
    public GameObject Sprites;

    private float cooldown;

    private ShootComponent[] shooters;

    private float cdTimer = 0f;
    private Coroutine damageRoutine;
    private Rigidbody2D body;
    private Collider2D coll;
    private SpriteRenderer[] spriteRenderer;

    private enum States
    {
        None, TakingDamage, Dead
    }
    private States state;

    private void Start()
    {
        cooldown = Random.Range(ShootCooldown, ShootCooldown * 2f);
        shooters = GetComponents<ShootComponent>();
        body = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        spriteRenderer = Sprites.GetComponentsInChildren<SpriteRenderer>();
        ExplosionEffect.SetActive(false);
    }

    protected override void Update()
    {
        if (state == States.Dead) return;

        if (Vector2.Distance(Player.Instance.transform.position, transform.position) < 30f)
        {
            HasAggro();
        }
        else if (hasAggro)
        {
            LostAggro();
        }

        if (Turrets != null)
        {
            Vector2 distVector = Player.Instance.transform.position - Turrets.transform.position;
            float targetRotation = Mathf.Atan2(distVector.y, distVector.x) * Mathf.Rad2Deg;
            Turrets.eulerAngles = new Vector3(0f, 0f, targetRotation);
        }

        if (cdTimer < cooldown)
        {
            if (state == States.None)
            {
                cdTimer += Time.deltaTime;
            }
        }
        else
        {
            cooldown = Random.Range(ShootCooldown, ShootCooldown * 2f);
            cdTimer = 0f;
            foreach (ShootComponent shooter in shooters)
            {
                shooter.Shoot(Player.Instance.gameObject);
            }
        }
    }
    
    public override void DamageEnemy(Vector2 direction, int damage = 1)
    {
        healthComponent.TakeDamage(damage);
        if (healthComponent.IsDead())
        {
            DestroyEnemy();
        }
        else
        {
            if (damageRoutine != null) StopCoroutine(damageRoutine);
            damageRoutine = StartCoroutine(ShowDamaged(direction));
        }
    }

    protected override void DestroyEnemy()
    {
        state = States.Dead;
        if (damageRoutine != null) StopCoroutine(damageRoutine);
        body.velocity = Vector2.zero;
        body.isKinematic = true;
        coll.enabled = false;
        Sprites.SetActive(false);
        LostAggro();

        Turrets.gameObject.SetActive(false);
        ExplosionEffect.SetActive(true);
    }

    private IEnumerator ShowDamaged(Vector2 direction)
    {
        state = States.TakingDamage;

        body.velocity = direction.normalized * 14f;

        foreach (var r in spriteRenderer)
        {
            if (r.name != "FlameGreen")
            {
                r.color = new Color(1f, 0f, 0f, 0.6f);
            }
        }
        yield return new WaitForSeconds(0.3f);
        foreach (var r in spriteRenderer)
        {
            if (r.name != "FlameGreen")
            {
                r.color = Color.white;
            }
        }

        body.velocity = Vector2.zero;
        yield return new WaitForSeconds(0.7f);
        state = States.None;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootTestEnemy : BaseEnemy {

    public float ShootCooldown = 1f;
    public Transform Turrets;
    public GameObject ExplosionEffect;
    public GameObject Sprites;
    public GameObject StunEffect;

    private float cooldown;

    private ShootComponent[] shooters;

    private float cdTimer = 0f;
    private Coroutine damageRoutine;
    private Rigidbody2D body;
    private Collider2D coll;
    private SpriteRenderer[] spriteRenderer;

    private enum States
    {
        None, TakingDamage, Dead, Stunned
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
        StunEffect.SetActive(false);
    }

    protected override void Update()
    {
        if (state == States.Dead || state == States.Stunned) return;

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
        AudioMaster.PlayAudio(AudioMaster.AudioClips.Explosion1);
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

    private Coroutine stunRoutine;
    public override void Stun(Vector2 direction, float stunTime)
    {
        if (stunRoutine != null) StopCoroutine(stunRoutine);
        stunRoutine = StartCoroutine(StunRoutine(stunTime));
    }

    private IEnumerator StunRoutine(float stunTime)
    {
        state = States.Stunned;
        Color origiginalColor = spriteRenderer[0].color;
        StunEffect.SetActive(true);
        SetRendererColour(new Color(0.3f, 0.3f, 1f, 1f));
        float timer = 0;
        while(timer < stunTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        StunEffect.SetActive(false);
        SetRendererColour(origiginalColor);
        state = States.None;
    }

    private void SetRendererColour(Color color)
    {
        foreach (SpriteRenderer r in spriteRenderer)
        {
            r.color = color;
        }
    }
}

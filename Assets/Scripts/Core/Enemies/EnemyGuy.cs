using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGuy : BaseEnemy {
    
    private BasicPatrolComponent patrolComponent;
    private CapsuleCollider2D coll;
    private Rigidbody2D body;
    private SpriteRenderer spriteRenderer;

    private enum States
    {
        None, Patrolling, Dead, TakingDamage
    }
    private States state;

    private Coroutine damageRoutine;

    private void Start()
    {
        coll = GetComponent<CapsuleCollider2D>();
        body = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        patrolComponent = GetComponent<BasicPatrolComponent>();
        patrolComponent.Activate();
        state = States.Patrolling;
    }

    protected override void Update()
    {
        switch (state)
        {
            case States.None:
                break;
            case States.Patrolling:
                break;
            case States.Dead:
                break;
            default:
                break;
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
        body.velocity = Vector2.zero;
        if (damageRoutine != null) StopCoroutine(damageRoutine);
        patrolComponent.Deactivate();
        anim.Play("Explosion", 0, 0f);
        coll.enabled = false;
        body.isKinematic = true;
        transform.localScale = Vector3.one * 1.8f;
    }

    private IEnumerator ShowDamaged(Vector2 direction)
    {
        state = States.TakingDamage;
        patrolComponent.Deactivate();

        body.velocity = direction.normalized * 14f;

        spriteRenderer.color = new Color(1f, 0f, 0f, 0.5f);
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.color = Color.white;

        yield return new WaitForSeconds(0.8f);
        patrolComponent.Activate();
        state = States.Patrolling;
    }

}

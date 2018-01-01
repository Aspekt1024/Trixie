using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGuy : BaseEnemy {
    
    private BasicPatrolComponent patrolComponent;
    private HealthComponent healthComponent;
    private CapsuleCollider2D coll;
    private Rigidbody2D body;

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
        healthComponent = GetComponent<HealthComponent>();
        patrolComponent = GetComponent<BasicPatrolComponent>();
        patrolComponent.Activate();
        state = States.Patrolling;
    }

    private void Update()
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

    public override void DamageEnemy(int damage = 1)
    {
        healthComponent.TakeDamage(damage);
        if (healthComponent.IsDead())
        {
            DestroyEnemy();
        }
        else
        {
            if (damageRoutine != null) StopCoroutine(damageRoutine);
            damageRoutine = StartCoroutine(ShowDamaged());
        }
    }

    protected override void DestroyEnemy()
    {
        state = States.Dead;
        if (damageRoutine != null) StopCoroutine(damageRoutine);
        patrolComponent.Deactivate();
        anim.Play("Explosion", 0, 0f);
        coll.enabled = false;
        body.isKinematic = true;
        transform.localScale = Vector3.one * 1.8f;
    }

    private IEnumerator ShowDamaged()
    {
        state = States.TakingDamage;
        patrolComponent.Deactivate();
        yield return new WaitForSeconds(1.2f);
        patrolComponent.Activate();
        state = States.Patrolling;
    }

}

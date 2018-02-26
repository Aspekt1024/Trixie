using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGuy : BaseEnemy {
    
    private BasicPatrolComponent patrolComponent;

    //private enum States
    //{
    //    None, Patrolling, Dead, TakingDamage
    //}
    //private States state;
    

    private void Start()
    {
        patrolComponent = GetComponent<BasicPatrolComponent>();
        patrolComponent.Activate();
    }

    //protected override void Update()
    //{
    //    switch (state)
    //    {
    //        case States.None:
    //            break;
    //        case States.Patrolling:
    //            break;
    //        case States.Dead:
    //            break;
    //        default:
    //            break;
    //    }
    //}

    //public override void DamageEnemy(Vector2 direction, int damage = 1)
    //{
    //    healthComponent.TakeDamage(damage);
    //    if (healthComponent.IsDead())
    //    {
    //        DestroyEnemy();
    //    }
    //    else
    //    {
    //        if (damageRoutine != null) StopCoroutine(damageRoutine);
    //        damageRoutine = StartCoroutine(ShowDamaged(direction));
    //    }
    //}

    //protected override void DestroyEnemy()
    //{
    //    state = States.Dead;
    //    body.velocity = Vector2.zero;
    //    if (damageRoutine != null) StopCoroutine(damageRoutine);
    //    patrolComponent.Deactivate();
    //    anim.Play("Explosion", 0, 0f);
    //    AudioMaster.PlayAudio(AudioMaster.AudioClips.Explosion1);
    //    coll.enabled = false;
    //    body.isKinematic = true;
    //    transform.localScale = Vector3.one * 1.8f;
    //}
    
    protected override IEnumerator ShowDamaged(Vector2 direction)
    {
        patrolComponent.Deactivate();
        SetSpriteColour(new Color(1f, 0f, 0f, 0.5f));
        body.velocity = (direction  + Vector2.up * 4).normalized * 14f;

        AudioMaster.PlayAudio(AudioMaster.AudioClips.Hit1);

        yield return new WaitForSeconds(0.8f);

        SetSpriteColour(Color.white);
        patrolComponent.Activate();
    }

    protected override void DestroyEnemy()
    {
        base.DestroyEnemy();
        patrolComponent.Deactivate();
    }

}

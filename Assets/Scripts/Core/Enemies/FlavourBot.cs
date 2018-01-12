using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlavourBot : BaseEnemy {

    public BallisticsComponent TurretUpper;
    public BallisticsComponent TurretLower;

    private Collider2D enemyCollider;

    private SpriteRenderer sr;

    private enum States
    {
        Normal, Dead
    }
    private States state;
    
    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        enemyCollider = GetComponent<CircleCollider2D>();
        TurretUpper.Activate();
        //TurretLower.Activate();
    }

    protected override void Update()
    {
        switch(state)
        {
            case States.Normal:
                // patrol, move etc
                // can we move and aim and shoot all at the same time?
                break;
            case States.Dead:
                break;
        }

        //if (transform.eulerAngles.z > 90 || transform.eulerAngles.z < -90)
        //{
        //    sr.flipX = true;
        //    sr.flipY = true;
        //}
        //else
        //{
        //    sr.flipX = false;
        //    sr.flipY = false;
        //}
    }

    // TODO enable
    


    protected override void DestroyEnemy()
    {
        TurretUpper.Deactivate();
        TurretLower.Deactivate();
        enemyCollider.enabled = false;
        //anim.Play("Explosion", 0, 0f);
        gameObject.SetActive(false);
        state = States.Dead;
    }

}

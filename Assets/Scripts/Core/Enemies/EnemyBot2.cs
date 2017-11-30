using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBot2 : BaseEnemy {

    public GameObject Flame;

    private Collider2D enemyCollider;
    private ShootComponent shootComponent;

    private enum States
    {
        Normal, Dead
    }
    private States state;
    
    private void Start()
    {
        enemyCollider = GetComponent<CircleCollider2D>();
        shootComponent = GetComponent<ShootComponent>();
        shootComponent.SetTarget(Player.Instance.transform);
        shootComponent.Activate();
    }

    private void Update()
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
    }

    // TODO enable
    


    protected override void DestroyEnemy()
    {
        shootComponent.Deactivate();
        Flame.SetActive(false);
        enemyCollider.enabled = false;
        anim.Play("Explosion", 0, 0f);
        state = States.Dead;
    }

}

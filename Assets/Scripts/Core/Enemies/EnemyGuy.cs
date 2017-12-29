using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGuy : BaseEnemy {
    
    private BasicPatrolComponent patrolComponent;

    private enum States
    {
        None, Patrolling, Dead
    }
    private States state;

    private void Start()
    {
        patrolComponent = GetComponent<BasicPatrolComponent>();
        state = States.Patrolling;
        patrolComponent.Activate();
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


    protected override void DestroyEnemy()
    {
        state = States.Dead;
        patrolComponent.Deactivate();
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGuy : BaseEnemy {

    public float MovementSpeed = 7f;

    private EnemyPatrolComponent patrolComponent;

    private enum States
    {
        None, Patrolling, Dead
    }
    private States state;

    private void Start()
    {
        patrolComponent = GetComponent<EnemyPatrolComponent>();
        state = States.Patrolling;
        patrolComponent.Activate();
        patrolComponent.SetMoveSpeed(MovementSpeed);
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

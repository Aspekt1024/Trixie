using System;
using ReGoap.Utilities;
using UnityEngine;

// generic goto state, can be used in most games, override Tick and Enter if you are using 
//  a navmesh / pathfinding library 
//  (ex. tell the library to search a path in Enter, when done move to the next waypoint in Tick)

[RequireComponent(typeof(GoapStateMachine))]
[RequireComponent(typeof(IdleState))]
[RequireComponent(typeof(EnemyAITest))]
public class GotoState : MachineState
{
    private EnemyAITest pathfinder;
    private Action onDoneMovementCallback;
    private Action onFailureMovementCallback;
    private Vector3 targetPosition;

    private enum States
    {
        Disabled, Pulsed, Active, Success, Failure
    }
    private States state;
    
    // additional feature, check for stuck, userful when using rigidbody or raycasts for movements
    private Vector3 lastStuckCheckUpdatePosition;
    private float stuckCheckCooldown;
    public bool CheckForStuck;
    public float StuckCheckDelay = 1f;
    public float MaxStuckDistance = 0.1f;

    public void SetTargetPosition(Vector3 position)
    {
        targetPosition = position;
    }
    
    protected override void Awake()
    {
        base.Awake();
        pathfinder = GetComponentInParent<EnemyAITest>();
    }

    #region Work

    protected override void Update()
    {
        base.Update();
        MoveTo(targetPosition);
    }

    protected virtual void MoveTo(Vector3 position)
    {
        pathfinder.Activate(position);
        
        if (pathfinder.HasFinishedPathing())
        {
            state = States.Success;
        }
        return;
    }

    private bool CheckIfStuck()
    {
        // TODO update this
        if (Time.time > stuckCheckCooldown)
        {
            stuckCheckCooldown = Time.time + StuckCheckDelay;
            if ((lastStuckCheckUpdatePosition - transform.position).magnitude < MaxStuckDistance)
            {
                ReGoapLogger.Log("[GotoState] '" + name + "' is stuck.");
                return true;
            }
            lastStuckCheckUpdatePosition = transform.position;
        }
        return false;
    }

    #endregion

    #region StateHandler
    public override void Init(GoapStateMachine stateMachine)
    {
        base.Init(stateMachine);
        var transition = new SmTransition(GetPriority(), Transition);
        var doneTransition = new SmTransition(GetPriority(), DoneTransition);
        stateMachine.GetComponent<IdleState>().Transitions.Add(transition);
        Transitions.Add(doneTransition);
    }

    private Type DoneTransition(IMachineState state)
    {
        if (this.state != States.Active)
            return typeof(IdleState);
        return null;
    }

    private Type Transition(IMachineState state)
    {
        if (this.state == States.Pulsed)
            return typeof(GotoState);
        return null;
    }

    public void GoTo(Vector3 position, Action onDoneMovement, Action onFailureMovement)
    {
        targetPosition = position;
        GoTo(onDoneMovement, onFailureMovement);
    }

    void GoTo(Action onDoneMovement, Action onFailureMovement)
    {
        state = States.Pulsed;
        onDoneMovementCallback = onDoneMovement;
        onFailureMovementCallback = onFailureMovement;
    }

    public override void Enter()
    {
        base.Enter();
        state = States.Active;
    }

    public override void Exit()
    {
        pathfinder.CancelPath();
        if (state == States.Success)
        {
            onDoneMovementCallback();
        }
        else
        {
            onFailureMovementCallback();
        }
    }
    #endregion
}

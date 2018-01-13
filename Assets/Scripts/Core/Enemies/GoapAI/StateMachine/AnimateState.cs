using System;
using ReGoap.Utilities;
using UnityEngine;

// generic goto state, can be used in most games, override Tick and Enter if you are using 
//  a navmesh / pathfinding library 
//  (ex. tell the library to search a path in Enter, when done move to the next waypoint in Tick)

[RequireComponent(typeof(GoapStateMachine))]
[RequireComponent(typeof(IdleState))]
public class AnimateState : MachineState
{
    private Action onDoneAnimationCallback;
    private Action onFailureAnimationCallback;
    private float animTimer;
    private float animDuration;

    private enum States
    {
        Disabled, Pulsed, Active, Success, Failure
    }
    private States state;
    
    #region Work

    protected override void Update()
    {
        base.Update();
        //if (state == States.Disabled || state == States.Success || state == States.Failure) return;

        animTimer += Time.deltaTime;
        if (animTimer > animDuration)
        {
            state = States.Success;
        }
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
            return typeof(AnimateState);
        return null;
    }

    public void Animate(float animDuration, Action onDoneAnimate, Action onFailAnimate)
    {
        this.animDuration = animDuration;
        Animate(onDoneAnimate, onFailAnimate);
    }

    public void Animate(Animator anim, string animation, Action onDoneAnimate, Action onFailAnimate)
    {
        animDuration = 0f;
        if (anim != null)
        {
            anim.Play(animation, 0, 0f);
        }
        Animate(onDoneAnimate, onFailAnimate);
    }

    private void Animate(Action onDoneAnimate, Action onFailAnimate)
    {
        animTimer = 0f;
        state = States.Pulsed;
        onDoneAnimationCallback = onDoneAnimate;
        onFailureAnimationCallback = onFailAnimate;
    }

    public override void Enter()
    {
        base.Enter();
        state = States.Active;
    }

    public override void Exit()
    {
        if (state == States.Success)
        {
            onDoneAnimationCallback();
        }
        else
        {
            onFailureAnimationCallback();
        }
    }
    #endregion
}

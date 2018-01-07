using System.Collections.Generic;
using UnityEngine;

public class MachineState : MonoBehaviour, IMachineState
{
    public List<ISmTransition> Transitions { get; set; }
    public int priority;

    #region UnityFunctions
    protected virtual void Awake()
    {
        Transitions = new List<ISmTransition>();
    }
    protected virtual void Start()
    {

    }
    protected virtual void FixedUpdate()
    {
    }
    protected virtual void Update()
    {
    }
    #endregion

    #region ISmState
    public virtual void Enter()
    {
    }

    public virtual void Exit()
    {
    }

    public virtual void Init(GoapStateMachine stateMachine)
    {
    }

    public virtual bool IsActive()
    {
        return enabled;
    }

    public virtual int GetPriority()
    {
        return priority;
    }
    #endregion
}
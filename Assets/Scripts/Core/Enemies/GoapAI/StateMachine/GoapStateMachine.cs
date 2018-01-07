using System;
using System.Collections.Generic;
using UnityEngine;

using GoapLabels = GoapConditions.Labels;

public class GoapStateMachine : MonoBehaviour
{
    private Dictionary<Type, IMachineState> states;
    private Dictionary<GoapLabels, object> values;
    private static Dictionary<GoapLabels, object> globalValues;
    private List<ISmTransition> genericTransitions;

    public bool enableStackedStates;
    public Stack<IMachineState> currentStates;

    public IMachineState CurrentState
    {
        get
        {
            if (enableStackedStates)
                return currentStates.Count == 0 ? null : currentStates.Peek();
            return currentState;
        }
    }

    private IMachineState currentState;

    public MonoBehaviour initialState;

    public bool permitLoopTransition = true;

    public bool orderTransitions;

    void OnDisable()
    {
        if (CurrentState != null)
            CurrentState.Exit();
    }

    void Awake()
    {
        enabled = true;
        states = new Dictionary<Type, IMachineState>();
        values = new Dictionary<GoapLabels, object>();
        currentStates = new Stack<IMachineState>();
        genericTransitions = new List<ISmTransition>();
        globalValues = new Dictionary<GoapLabels, object>();
    }

    void Start()
    {
        foreach (var state in GetComponents<IMachineState>())
        {
            AddState(state);
            var monoB = (MonoBehaviour) state;
            monoB.enabled = false;
        }
        Switch(initialState.GetType());
    }

    public void AddState(IMachineState state)
    {
        state.Init(this);
        states[state.GetType()] = state;
    }

    public void AddGenericTransition(ISmTransition func)
    {
        genericTransitions.Add(func);
        if (orderTransitions)
            genericTransitions.Sort();
    }

    public T GetValue<T>(GoapLabels key)
    {
        if (!HasValue(key))
            return default(T);
        return (T) values[key];
    }

    public bool HasValue(GoapLabels key)
    {
        return values.ContainsKey(key);
    }

    public void SetValue<T>(GoapLabels key, T value)
    {
        values[key] = value;
    }

    public void RemoveValue(GoapLabels key)
    {
        values.Remove(key);
    }

    public static T GetGlobalValue<T>(GoapLabels key)
    {
        return (T) globalValues[key];
    }

    public static bool HasGlobalValue(GoapLabels key)
    {
        return globalValues.ContainsKey(key);
    }

    public static void SetGlobalValue<T>(GoapLabels key, T value)
    {
        globalValues[key] = value;
    }

    void FixedUpdate()
    {
        Check();
    }

    void Check()
    {
        for (var index = genericTransitions.Count - 1; index >= 0; index--)
        {
            var trans = genericTransitions[index];
            var result = trans.TransitionCheck(CurrentState);
            if (result != null)
            {
                Switch(result);
                return;
            }
        }
        if (CurrentState == null) return;
        for (var index = CurrentState.Transitions.Count - 1; index >= 0; index--)
        {
            var trans = CurrentState.Transitions[index];
            var result = trans.TransitionCheck(CurrentState);
            if (result != null)
            {
                Switch(result);
                return;
            }
        }
    }

    public void Switch<T>() where T : MonoBehaviour, IMachineState
    {
        Switch(typeof(T));
    }

    public void Switch(Type T)
    {
        if (CurrentState != null)
        {
            if (!permitLoopTransition && (CurrentState.GetType() == T)) return;
            ((MonoBehaviour) CurrentState).enabled = false;
            CurrentState.Exit();
        }
        if (enableStackedStates)
            currentStates.Push(states[T]);
        else
            currentState = states[T];
        ((MonoBehaviour) CurrentState).enabled = true;
        CurrentState.Enter();

        if (orderTransitions)
            CurrentState.Transitions.Sort();
    }

    public void PopState()
    {
        if (!enableStackedStates)
        {
            throw new UnityException(
                "[GoapStateMachine] Trying to pop a state from a state machine with disabled stacked states.");
        }
        currentStates.Peek().Exit();
        ((MonoBehaviour) currentStates.Pop()).enabled = false;
        ((MonoBehaviour) currentStates.Peek()).enabled = true;
        currentStates.Peek().Enter();
    }
}
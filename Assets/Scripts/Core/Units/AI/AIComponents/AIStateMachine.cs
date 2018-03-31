using System;
using System.Collections.Generic;

namespace Aspekt.AI
{
    public class AIStateMachine
    {
        private AIMachineState currentState;
        private Queue<AIMachineState> stateQueue;
        private AIAgent agent;
        
        public event Action OnComplete = delegate { };

        private enum States
        {
            Stopped, Paused, Active, TransitionToNextState, TransitionToIdleState
        }
        private States state;

        public AIStateMachine(AIAgent agent)
        {
            this.agent = agent;
            stateQueue = new Queue<AIMachineState>();
            SetIdleState();
        }

        public void Tick(float deltaTime)
        {
            if (state == States.Stopped || state == States.Paused) return;

            if (state == States.TransitionToNextState)
            {
                GotoNextState();
            }
            else if (state == States.TransitionToIdleState)
            {
                if (OnComplete != null) OnComplete();
                currentState.OnComplete -= StateCompleted;
                SetIdleState();
            }
            else if (currentState.GetType().Equals(typeof(IdleState)) && stateQueue.Count > 0)
            {
                GotoNextState();
            }
            else
            {
                currentState.Tick(deltaTime);
            }
        }

        public bool IsIdle { get { return currentState == null || currentState.GetType().Equals(typeof(IdleState)); } }
        public string GetState() { return state.ToString(); }
        public string GetCurrentState() { return currentState.ToString(); }

        public void Enqueue(AIMachineState newState)
        {
            stateQueue.Enqueue(newState);
        }

        public T AddState<T>() where T: AIMachineState, new()
        {
            T newState = new T();
            newState.SetParentAgent(agent);
            Enqueue(newState);
            return newState;
        }

        public void Stop()
        {
            state = States.Stopped;
            // TODO clear states
        }

        public void Pause()
        {
            state = States.Paused;
        }

        public void Activate()
        {
            if (state == States.Stopped)
            {
                SetIdleState();
            }
            state = States.Active;
        }

        private void GotoNextState()
        {
            state = States.Active;
            currentState.OnComplete -= StateCompleted;
            currentState = stateQueue.Dequeue();
            currentState.OnComplete += StateCompleted;
            currentState.Enter();
        }

        private void SetIdleState()
        {
            state = States.Active;
            if (stateQueue.Count > 0)
            {
                GotoNextState();
                return;
            }
            IdleState initialState = AddState<IdleState>();
            initialState.OnComplete += StateCompleted;
            initialState.Enter();
            currentState = stateQueue.Dequeue();
        }

        private void StateCompleted()
        {
            if (state == States.TransitionToIdleState || state == States.TransitionToNextState) return;

            if (stateQueue.Count > 0)
            {
                state = States.TransitionToNextState;
            }
            else
            {
                state = States.TransitionToIdleState;
            }
        }
    }
}

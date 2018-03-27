using System;
using System.Collections.Generic;

namespace Aspekt.AI
{
    public class AIExecutor
    {
        public event Action OnFinishedPlan = delegate { };

        private Queue<AIAction> actionPlan;
        private AIAction currentAction;
        private AIGoal currentGoal;
        private AIStateMachine stateMachine;
        private AIAgent agent;

        private enum States
        {
            Stopped, Running, Paused
        }
        private States state;

        public AIExecutor(AIAgent agent)
        {
            this.agent = agent;
            stateMachine = new AIStateMachine(agent);
        }
        
        public void Tick(float deltaTime)
        {
            if (currentAction == null) return;

            switch (state)
            {
                case States.Stopped:
                    break;
                case States.Running:
                    currentAction.Tick(deltaTime);
                    stateMachine.Tick(deltaTime);
                    break;
                case States.Paused:
                    break;
                default:
                    break;
            }

        }

        public void ExecutePlan(Queue<AIAction> newActionPlan, AIGoal goal)
        {
            if (currentAction != null)
            {
                stateMachine.Stop();
                stateMachine.Activate();    // TODO.. stop activate should be simpler
            }
            currentGoal = goal;
            actionPlan = newActionPlan;
            BeginNextAction();
        }

        public void Stop()
        {
            if (state == States.Stopped)
            {
                AILogger.CreateMessage("Already stopped", agent);
            }
            else
            {
                state = States.Stopped;
                stateMachine.Stop();
                actionPlan = null;
                currentAction = null;
            }
        }

        public void Pause()
        {
            if (state == States.Running)
            {
                state = States.Paused;
                stateMachine.Pause();
            }
            else
            {
                AILogger.CreateMessage("Cannot Pause when not running. Current state = " + state.ToString(), agent);
            }
        }

        public void Unpause()
        {
            if (state == States.Paused)
            {
                state = States.Running;
                // TODO Unpause state machine
            }
            else
            {
                AILogger.CreateMessage("Error, cannot unpause from non-paused state. Current state = " + state.ToString(), agent);
            }
        }

        public AIGoal GetCurrentGoal()
        {
            return currentGoal;
        }

        public AIAction GetCurrentAction()
        {
            return currentAction;
        }

        public List<AIAction> GetActionPlan()
        {
            if (actionPlan == null) return new List<AIAction>();

            List<AIAction> plan = new List<AIAction>();
            foreach (var action in actionPlan)
            {
                plan.Add(action);
            }

            if (currentAction !=  null)
            {
                plan.Add(currentAction);
            }
            return plan;
        }

        private void BeginNextAction()
        {
            if (actionPlan.Count == 0)
            {
                currentAction = null;
                state = States.Stopped;
                if (OnFinishedPlan != null) OnFinishedPlan();
            }
            else
            {
                state = States.Running;

                currentAction = actionPlan.Dequeue();
                currentAction.Enter(stateMachine, ActionSuccess, ActionFailure);
            }
        }

        private void ActionSuccess()
        {
            // TODO assess this behaviour
            if (currentAction != null)
            {
                foreach (var effect in currentAction.GetEffects())
                {
                    agent.GetMemory().UpdateCondition(effect.Key, effect.Value);
                }
            }

            bool goalAchieved = true;
            foreach (var condition in currentGoal.GetConditions())
            {
                if (agent.GetMemory().ConditionMet(condition.Key, condition.Value) == false)
                {
                    goalAchieved = false;
                    break;
                }
            }

            if (goalAchieved)
            {
                Stop();
                if (OnFinishedPlan != null) OnFinishedPlan();
            }
            else
            {
                BeginNextAction();
            }
        }

        private void ActionFailure()
        {
            AILogger.CreateMessage("Action failed: " + currentAction.ToString(), agent);
            Stop();
            if (OnFinishedPlan != null) OnFinishedPlan();
        }
    }
}

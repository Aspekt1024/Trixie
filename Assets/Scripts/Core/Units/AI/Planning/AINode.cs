using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aspekt.AI.Planning
{
    public class AINode
    {
        private float g;    // Node Cost
        private float h;    // Heuristic

        private AIAgent agent;
        private AIAction action;
        private AINode parent;
        private AIState state;

        public AINode(AIAgent agent, AIPlanner planner, AIAction action = null, AINode parent = null)
        {
            this.agent = agent;
            this.action = action;
            this.parent = parent;

            if (action == null)
            {
                // This is a goal node
                state = new AIState(planner.GetGoal(), agent.GetMemory().CloneState());
                state.AddUnmetPreconditions(planner.GetGoal().GetConditions());
                g = 0;
            }
            else
            {
                // Initialise Action node
                state = new AIState();
                g = float.MaxValue;
            }
        }

        public void Update(AINode newParent)
        {
            if (newParent.g + action.Cost < g)
            {
                parent = newParent;
                SetNodeActionDetails();
            }
        }

        private void SetNodeActionDetails()
        {
            g = parent.g + action.Cost;
            state = parent.CloneState();
            state.ClearMetPreconditions(action.GetEffects());
            state.AddUnmetPreconditions(action.GetPreconditions());
            h = GetNumUnmetPreconditions();
        }

        private int GetNumUnmetPreconditions()
        {
            return state.GetPreconditions().Count;
        }

        public AIState CloneState()
        {
            return new AIState(state);
        }

        public AIState GetState()
        {
            return state;
        }

        public AIAction GetAction()
        {
            return action;
        }

        public AINode GetParent()
        {
            return parent;
        }

        public bool ConditionsMet()
        {
            bool conditionsMet = state.GetPreconditions().Count == 0;

            if (!conditionsMet)
            {
                conditionsMet = true;
                foreach (var precondition in state.GetPreconditions())
                {
                    if (!agent.GetMemory().ConditionMet(precondition.Key, precondition.Value))
                    {
                        conditionsMet = false;
                        break;
                    }
                }
            }

            return conditionsMet;
        }

        public float GetFCost()
        {
            return g + h;
        }
    }
}


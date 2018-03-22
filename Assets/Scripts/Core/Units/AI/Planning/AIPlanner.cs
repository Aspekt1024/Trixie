using System;
using System.Linq;
using System.Collections.Generic;

namespace Aspekt.AI.Planning
{
    public class AIPlanner
    {
        private AIAgent agent;
        private AIGoal currentGoal;
        private Queue<AIAction> actions = new Queue<AIAction>();
        private AIAStar aStar = new AIAStar();

        public event Action OnActionPlanFound = delegate { };

        public AIPlanner (AIAgent agent)
        {
            this.agent = agent;
        }

        public void CalculateNewGoal()
        {
            AILogger.CreateMessage("Calculating new goal.", agent);

            List<AIGoal> goals = new List<AIGoal>(agent.GetGoals());
            goals.Sort((x, y) => x.Priority.CompareTo(y.Priority));
            
            if (goals.Count == 0) return;
            
            for (int i = 0; i < goals.Count; i++)
            {
                currentGoal = goals[i];
                if (!GoalAchieveableByActions(currentGoal)) continue;

                if (aStar.FindActionPlan(agent, this))
                {
                    actions = aStar.GetActionPlan();
                }
                else
                {
                    AILogger.CreateMessage("failed to find action plan.", agent);
                }
            }

            if (actions.Count > 0 && OnActionPlanFound != null)
            {
                AILogger.CreateMessage("Action plan found.", agent);
                OnActionPlanFound();
            }
        }

        public Queue<AIAction> GetActionPlan()
        {
            return actions;
        }

        public AIGoal GetGoal()
        {
            return currentGoal;
        }

        private bool GoalAchieveableByActions(AIGoal goal)
        {
            // Note that this won't check if each action's preconditions will be met.
            // This will be calculated by aStar

            Dictionary<string, bool> conditionsMet = new Dictionary<string, bool>();
            foreach (var condition in goal.GetConditions())
            {
                conditionsMet.Add(condition.Key, false);
            }

            foreach (var stateValue in agent.GetMemory().GetState())
            {
                if (conditionsMet.ContainsKey(stateValue.Key) && conditionsMet[stateValue.Key].Equals(goal.GetConditions()[stateValue.Key]))
                {
                    conditionsMet[stateValue.Key] = true;
                }
            }

            foreach (var action in agent.GetActions())
            {
                if (!action.CheckProceduralPrecondition()) continue;

                foreach (var effect in action.GetEffects())
                {
                    if (conditionsMet.ContainsKey(effect.Key) && effect.Value.Equals(goal.GetConditions()[effect.Key]))
                    {
                        conditionsMet[effect.Key] = true;
                    }
                }
            }

            return !conditionsMet.ContainsValue(false);
        }
    }
}

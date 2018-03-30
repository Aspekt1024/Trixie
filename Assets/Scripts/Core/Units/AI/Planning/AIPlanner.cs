using System;
using System.Linq;
using System.Collections.Generic;

using System.Diagnostics;

namespace Aspekt.AI.Planning
{
    public class AIPlanner
    {
        private AIAgent agent;
        private AIGoal currentGoal;
        private Queue<AIAction> actions = new Queue<AIAction>();
        private AIAStar aStar = new AIAStar();

        private List<AIGoal> goals = new List<AIGoal>();
        
        public event Action OnActionPlanFound = delegate { };

        private bool calculatingPlan;

        public AIPlanner (AIAgent agent)
        {
            this.agent = agent;
        }

        public void CalculateNewPlan()
        {
            if (calculatingPlan || agent.GetGoals().Length == 0) return;
            calculatingPlan = true;
            
            AILogger.CreateMessage("Calculating new plan.", agent);
            
            goals = new List<AIGoal>(agent.GetGoals());
            goals.Sort((x, y) => x.Priority.CompareTo(y.Priority));
            
            for (int i = 0; i < goals.Count; i++)
            {
                currentGoal = goals[i];
                if (!GoalAchieveableByActions(currentGoal)) continue;

                bool actionPlanFound = aStar.FindActionPlan(agent, this);
                if (actionPlanFound)
                {
                    ActionPlanFound();
                    return;
                }
            }

            calculatingPlan = false;
            AILogger.CreateMessage("Failed to find action plan.", agent);
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
                bool conditionMet = false;
                if (agent.GetMemory().GetState().ContainsKey(condition.Key))
                {
                    conditionMet = agent.GetMemory().GetState()[condition.Key].Equals(condition.Value);
                }
                conditionsMet.Add(condition.Key, conditionMet);
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

        private void ActionPlanFound()
        {
            AILogger.CreateMessage("Action plan found.", agent);
            actions = aStar.GetActionPlan();
            OnActionPlanFound();
            calculatingPlan = false;
        }
    }
}

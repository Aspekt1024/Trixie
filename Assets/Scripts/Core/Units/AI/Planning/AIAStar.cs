using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aspekt.AI.Planning
{
    public class AIAStar
    {
        private List<AINode> nullNodes = new List<AINode>();
        private List<AINode> openNodes = new List<AINode>();
        private List<AINode> closedNodes = new List<AINode>();

        private AINode currentNode;

        public bool FindActionPlan(AIAgent agent, AIPlanner planner)
        {
            InitialiseNodeLists(agent, planner);

            while (!currentNode.ConditionsMet())
            {
                FindNeighbouringNodes();

                if (openNodes.Count == 0) return false;

                currentNode = FindCheapestNode();
                closedNodes.Add(currentNode);
                openNodes.Remove(currentNode);
            }

            return true;
        }

        public Queue<AIAction> GetActionPlan()
        {
            Queue<AIAction> queue = new Queue<AIAction>();
            while (currentNode.GetAction() != null)
            {
                queue.Enqueue(currentNode.GetAction());
                currentNode = currentNode.GetParent();
            }

            Queue<AIAction> actionPlan = new Queue<AIAction>();
            while (queue.Count > 0)
            {
                actionPlan.Enqueue(queue.Dequeue());
            }

            return actionPlan;
        }
        
        private void InitialiseNodeLists(AIAgent agent, AIPlanner planner)
        {
            nullNodes = new List<AINode>();
            openNodes = new List<AINode>();
            closedNodes = new List<AINode>();

            currentNode = new AINode(agent, planner);
            closedNodes.Add(currentNode);

            foreach (var action in agent.GetActions())
            {
                nullNodes.Add(new AINode(agent, planner, action));
            }
        }
        
        private void FindNeighbouringNodes()
        {
            for (int i = nullNodes.Count - 1; i >= 0; i--)
            {
                if (!nullNodes[i].GetAction().CheckProceduralPrecondition()) continue;

                if (AchievesPrecondition(nullNodes[i]))
                {
                    nullNodes[i].Update(currentNode);
                    openNodes.Add(nullNodes[i]);
                    nullNodes.Remove(nullNodes[i]);
                }
            }
        }

        private bool AchievesPrecondition(AINode node)
        {
            foreach (var effect in node.GetAction().GetEffects())
            {
                if (currentNode.GetState().GetPreconditions().ContainsKey(effect.Key) && currentNode.GetState().GetPreconditions()[effect.Key].Equals(effect.Value))
                {
                    return true;
                }
            }
            return false;
        }

        private AINode FindCheapestNode()
        {
            AINode cheapestNode = openNodes[0];
            for (int i = 1; i < openNodes.Count; i++)
            {
                if (openNodes[i].GetFCost() < cheapestNode.GetFCost())
                {
                    cheapestNode = openNodes[i];
                }
            }
            return cheapestNode;
        }
    }
}

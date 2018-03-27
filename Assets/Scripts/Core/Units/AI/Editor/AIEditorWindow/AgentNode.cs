using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aspekt.AI.Editor
{
    public class AgentNode : BaseNode
    {
        private AIAgent agent;
        public AIAgent Agent
        {
            get { return agent; }
            set
            {
                agent = value;
            }
        }

        private List<ActionNode> actionNodes = new List<ActionNode>();
        private GoalNode goalNode;
        private bool agentLoaded;
        
        protected string nodeType;
        
        public bool HasCurrentGoal { get { return Agent.GetCurrentGoal() != null; } }

        public GoalNode GetGoalNode()
        {
            return goalNode;
        }

        public List<ActionNode> GetActionNodes()
        {
            return actionNodes;
        }

        protected override void SetupNode()
        {
            isActive = true;
            nodeType = "Unit";
            SetSize(new Vector2(200f, 50f));
        }

        protected override void DrawContent()
        {
            if (!agentLoaded)
            {
                if (Agent == null || agent.GetPlanner() == null) return;

                agent.GetPlanner().OnActionPlanFound += NewActionPlan;
                title = Agent.GetNameSlow();
                agentLoaded = true;
            }
            
            AIGUI.LabelLayout(agent.GetState());

            AIAction currentAction = agent.GetExecutor().GetCurrentAction();
            if (currentAction == null) return;

            foreach (var actionNode in actionNodes)
            {
                if (actionNode.Action == currentAction)
                {
                    actionNode.SetActive();
                }
                else
                {
                    actionNode.SetInactive();
                }
            }

        }

        protected override string GetNodeType()
        {
            return nodeType.ToString();
        }

        ~AgentNode()
        {
            Agent.GetPlanner().OnActionPlanFound -= NewActionPlan;
        }

        private void NewActionPlan()
        {
            goalNode = new GoalNode();
            actionNodes = new List<ActionNode>();

            goalNode.Goal = agent.GetCurrentGoal();
            goalNode.SetPosition(position + new Vector2(0, GetSize().y + 10f));

            float cumulativeActionWindowHeight = 0;

            List<AIAction> actions = Agent.GetActionPlan();
            for (int i = 0; i < actions.Count; i++)
            {
                ActionNode actionNode = new ActionNode();
                actionNode.SetPosition(position + new Vector2(0f, GetSize().y + goalNode.GetSize().y + 20f + cumulativeActionWindowHeight + 10f * i));
                actionNode.Action = actions[i];
                actionNodes.Add(actionNode);

                cumulativeActionWindowHeight += actionNode.GetSize().y;
            }
        }
    }
}

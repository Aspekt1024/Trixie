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
        
        public void ClearForDeconstruction()
        {
            if (agentLoaded)
            {
                agent.GetPlanner().OnActionPlanFound -= NewActionPlan;
            }
        }

        protected override void SetupNode()
        {
            isActive = true;
            nodeType = "Unit";
            SetSize(new Vector2(200f, 70f));
        }

        protected override void DrawContent()
        {
            // TODO check this, seems to be a memory leak calling NewActionPlan multiple times per frame
            if (!agentLoaded)
            {
                if (Agent == null || agent.GetPlanner() == null) return;

                agent.GetPlanner().OnActionPlanFound += NewActionPlan;
                title = Agent.GetNameSlow();
                agentLoaded = true;
            }
            
            AIGUI.LabelLayout(agent.GetState());

            AIStateMachine stateMachine = agent.GetExecutor().GetStateMachine();

            AIGUI.LabelLayout(stateMachine.GetCurrentState() + " (" + stateMachine.GetState() + ")");

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

        private void NewActionPlan()
        {
            goalNode = new GoalNode();
            actionNodes = new List<ActionNode>();

            goalNode.Goal = agent.GetCurrentGoal();
            goalNode.SetAgent(agent);
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Aspekt.AI.Editor
{
    public class AIEditorWindow : BaseEditor
    {
        private List<AgentNode> agents = new List<AgentNode>();

        [MenuItem("Window/Aspekt AI")]
        private static void ShowEditor()
        {
            AIEditorWindow editor = GetWindow<AIEditorWindow>();
            editor.LoadEditor();
        }

        protected override void SetTheme()
        {
            // TODO set image titleContent.image = (Texture)Resournces.Load("imagelocation");
            titleContent.text = "Aspekt AI";

        }

        protected override void GUIUpdate()
        {
            GetAllObjects();
        }

        private void GetAllObjects()
        {
            // TODO setup event for when aiagents are added or removed from the editor
            AIAgent[] agents = FindObjectsOfType<AIAgent>();
            List<AgentNode> nodesToRemove = CloneNodes();
            foreach (var agent in agents)
            {
                AgentNode agentNode = GetAgentNode(agent);
                if (agentNode ==  null)
                {
                    CreateNode(agent);
                }
                else
                {
                    UpdateNode(agentNode);
                    nodesToRemove.Remove(agentNode);
                }
            }

            foreach (AgentNode node in nodesToRemove)
            {
                GetNodes().Remove(node);
            }

        }

        private void CreateNode(AIAgent agent)
        {
            AgentNode newNode = new AgentNode();
            newNode.Agent = agent;
            newNode.SetPosition(new Vector2(10, 10 + GetNodes().Count * 120));
            AddNode(newNode);
            
            //foreach (var action in actionPlan)
            //{
            //    ActionNode newActionNode = new ActionNode();
            //    newActionNode.Action = action;
            //    newActionNode.SetPosition(new Vector2(10, 10 + GetNodes().Count * 120));
            //    AddNode(newActionNode);
            //}

        }

        private void UpdateNode(AgentNode agentNode)
        {
            if (!agentNode.HasCurrentGoal) return;
            agentNode.GetGoalNode().Draw(Vector2.zero);
            List<ActionNode> actionNodes = agentNode.GetActionNodes();

            foreach (var actionNode in actionNodes)
            {
                actionNode.Draw(Vector2.zero);
            }
        }

        private AgentNode GetAgentNode(AIAgent agent)
        {
            foreach (var node in GetNodes())
            {
                if (node.GetType().Equals(typeof(AgentNode)) && ((AgentNode)node).Agent == agent)
                {
                    return (AgentNode)node;
                }
            }
            return null;
        }

        private List<AgentNode> CloneNodes()
        {
            List<AgentNode> allNodes = new List<AgentNode>();
            foreach (var node in GetNodes())
            {
                allNodes.Add((AgentNode)node);
            }
            return allNodes;
        }

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Aspekt.AI.Editor
{
    public class AIEditorWindow : BaseEditor
    {
        private AIAgent agent;

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
            ShowAgentAndActionPlan();
        }

        private void ShowAgentAndActionPlan()
        {
            AIAgent[] agents = FindObjectsOfType<AIAgent>();
            if (agents == null || agents.Length == 0) return;
            agent = agents[0];
            
            AgentNode agentNode = CreateNode(agent);
            agentNode.Draw(Vector2.zero);

            CreateAgentMemoryNode(agentNode).Draw(Vector2.zero);
        }

        private AgentNode CreateNode(AIAgent agent)
        {
            AgentNode newNode = new AgentNode();
            newNode.Agent = agent;
            newNode.SetPosition(new Vector2(10, 10));
            return newNode;
        }

        private MemoryNode CreateAgentMemoryNode(AgentNode agentNode)
        {
            MemoryNode memoryNode = new MemoryNode();
            memoryNode.Agent = agent;
            memoryNode.SetPosition(new Vector2(20 + agentNode.GetSize().x, 10f));
            return memoryNode;
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

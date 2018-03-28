using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aspekt.AI.Editor
{
    public class GoalNode : BaseNode
    {
        public AIGoal Goal { get; set; }
        
        protected string nodeType;

        private AIAgent agent;
        
        public void SetAgent(AIAgent agent)
        {
            this.agent = agent;
        }

        protected override void SetupNode()
        {
            isActive = true;
            nodeType = "Goal";
            SetSize(new Vector2(200f, 80f));
        }

        protected override void DrawContent()
        {
            title = Goal.ToString();

            Color originalColour = GUI.skin.label.normal.textColor;
            
            Dictionary<string, object> conditions = Goal.GetConditions();
            foreach (var condition in conditions)
            {
                if (agent.GetMemory().ConditionMet(condition.Key, condition.Value))
                {
                    GUI.skin.label.normal.textColor = Color.green;
                }
                else
                {
                    GUI.skin.label.normal.textColor = Color.red;
                }
                AIGUI.LabelLayout(condition.Key);
            }

            GUI.skin.label.normal.textColor = originalColour;
        }

        protected override string GetNodeType()
        {
            return nodeType.ToString();
        }
    }
}

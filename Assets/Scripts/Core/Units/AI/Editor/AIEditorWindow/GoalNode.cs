using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aspekt.AI.Editor
{
    public class GoalNode : BaseNode
    {
        public AIGoal Goal { get; set; }
        
        protected string nodeType;
        
        protected override void SetupNode()
        {
            isActive = true;
            nodeType = "Goal";
            SetSize(new Vector2(200f, 80f));
        }

        protected override void DrawContent()
        {
            title = Goal.ToString();
        }

        protected override string GetNodeType()
        {
            return nodeType.ToString();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aspekt.AI.Editor
{
    public class ActionNode : BaseNode
    {
        public AIAction Action { get; set; }
        
        private string nodeType;
        
        public void SetActive()
        {
            isActive = true;
        }

        public void SetInactive()
        {
            isActive = false;
        }

        protected override void SetupNode()
        {
            nodeType = "Action";
            SetSize(new Vector2(200f, 100f));
        }

        protected override void DrawContent()
        {
            title = Action.ToString();
        }

        protected override string GetNodeType()
        {
            return nodeType.ToString();
        }
    }
}

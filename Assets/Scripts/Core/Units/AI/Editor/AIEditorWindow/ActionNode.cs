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
            SetSize(new Vector2(200f, 150f));
        }

        protected override void DrawContent()
        {
            title = Action.ToString();
            Dictionary<string, object> effects = Action.GetEffects();
            foreach (var effect in effects)
            {
                AIGUI.LabelLayout(effect.Key);
            }
            
            Color originalColour = GUI.skin.label.normal.textColor;
            
            Dictionary<string, object> preconditions = Action.GetPreconditions();
            foreach (var precondition in preconditions)
            {
                if (Action.GetAgent().GetMemory().ConditionMet(precondition.Key, precondition.Value))
                {
                    GUI.skin.label.normal.textColor = Color.green;
                }
                else
                {
                    GUI.skin.label.normal.textColor = Color.red;
                }
                AIGUI.LabelLayout(precondition.Key);
            }

            GUI.skin.label.normal.textColor = originalColour;
        }

        protected override string GetNodeType()
        {
            return nodeType.ToString();
        }
    }
}

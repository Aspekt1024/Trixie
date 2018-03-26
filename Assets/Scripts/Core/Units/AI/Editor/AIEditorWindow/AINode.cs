using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aspekt.AI.Editor
{
    public class AINode : BaseNode
    {
        protected enum NodeTypes
        {
            Action, Goal, Unit
        }
        protected NodeTypes nodeType;


        protected override void SetupNode()
        {
            nodeType = NodeTypes.Unit;
            SetSize(new Vector2(150f, 100f));
        }

        protected override void DrawContent()
        {
            nodeType = (NodeTypes)AIGUI.EnumPopupLayout("nodeType", nodeType);
        }

        protected override string GetNodeType()
        {
            return nodeType.ToString();
        }
    }
}

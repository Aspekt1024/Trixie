using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aspekt.AI.Editor
{
    public class MemoryNode : BaseNode
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
        
        private bool agentLoaded;
        private AIMemory memory;
        
        protected override void SetupNode()
        {
            isActive = true;
            title = "Unit Memory";
            SetSize(new Vector2(300f, 400f));
        }

        protected override void DrawContent()
        {
            if (!agentLoaded)
            {
                if (Agent == null || agent.GetMemory() == null) return;
                memory = agent.GetMemory();
                agentLoaded = true;
            }
            
            AIGUI.LabelLayout("Memory State:");
            AIGUI.Space();

            Dictionary<string, object> memoryState = memory.GetState();

            foreach (var item in memoryState)
            {
                AIGUI.LabelLayout(item.Key + " : " + item.Value.ToString());
            }

            float contentSize = Mathf.Clamp(AIGUI.GetContentHeight(), 50f, float.MaxValue);

        }

        protected override string GetNodeType()
        {
            return "Unit";
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Aspekt.AI;

namespace TrixieCore.Units
{
    public class SauceRobotBeta : BaseUnit
    {
        private void Start()
        {
            AIAgent agent = AI.GetComponent<AIAgent>();
            agent.Activate();
        }
    }
}


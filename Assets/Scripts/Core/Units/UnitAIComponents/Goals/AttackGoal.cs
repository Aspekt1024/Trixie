using Aspekt.AI;
using TestUnitLabels;
using UnityEngine;

namespace TrixieCore.Units
{
    public class AttackGoal : AIGoal
    {
        public override void ExitGoal(AIAgent agent)
        {
            if (Player.Instance.IsAlive)
            {
                agent.GetMemory().Set(SauceLabels.AttackGoalComplete, false);
            }
        }

        protected override void SetConditions()
        {
            //AddCondition(SauceLabels.IsAggrivated, true);
            AddCondition(SauceLabels.AttackGoalComplete, true);
        }
    }
}

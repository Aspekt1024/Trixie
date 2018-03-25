using UnityEngine;
using Aspekt.AI;
using TestUnitLabels;
using TrixieCore.Units;

public class SauceBotSensor : AISensor {

    VisionComponent vision;


    private void Start()
    {
        vision = agent.BaseUnit.GetAbility<VisionComponent>();
        vision.Activate();
    }

    private void Update()
    {
        agent.GetMemory().UpdateCondition(SauceLabels.HasCorrectProjectColour, true);
        agent.GetMemory().UpdateCondition(SauceLabels.CanSeeTarget, vision.CanSeePlayer());

        bool canSee = agent.GetMemory().ConditionMet(SauceLabels.CanSeeTarget, true);


        // TODO this should go somewhere else - this is so the ai can attack again
        agent.GetMemory().UpdateCondition(SauceLabels.TargetAttacked, false);
    }
}

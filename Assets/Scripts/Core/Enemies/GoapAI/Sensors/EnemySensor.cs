using ReGoap.Unity;
using UnityEngine;
using TrixieCore.Goap;

public class EnemySensor : ReGoapSensor<GoapLabels, object>
{
    private BaseEnemy enemyScript;
    private VisionComponent vision;

    private void Start()
    {
        var agent = GetComponentInParent<EnemyGoapAgent>();
        enemyScript = agent.Parent;
        vision = enemyScript.GetComponent<VisionComponent>();
        memory = agent.GetMemory();
    }

    public override void UpdateSensor()
    {
        var worldState = memory.GetWorldState();

        worldState.Set(GoapLabels.TargetFound, vision.CanSeePlayer());
        worldState.Set(GoapLabels.CanSeePlayer, vision.CanSeePlayer());
        worldState.Set(GoapLabels.HasSeenPlayerRecently, vision.HasSeenPlayerRecenty());
        worldState.Set(GoapLabels.NotSeenPlayerRecently, !vision.HasSeenPlayerRecenty());
        worldState.Set(GoapLabels.CanSensePlayer, Vector2.Distance(enemyScript.transform.position, Player.Instance.transform.position) < enemyScript.AggroRadius);

        Vector2 lookAtPosition = transform.position;
        if ((bool)worldState.Get(GoapLabels.CanSensePlayer) || (bool)worldState.Get(GoapLabels.CanSeePlayer))
        {
            lookAtPosition = Player.Instance.transform.position;
        }
        else if ((bool)worldState.Get(GoapLabels.HasSeenPlayerRecently))
        {
            lookAtPosition = vision.GetLastKnownPlayerPosition();
        }
        else
        {
            enemyScript.LostAggro();
            return;
        }

        enemyScript.HasAggro();
        worldState.Set(GoapLabels.LastKnownPlayerPosition, lookAtPosition);
        enemyScript.LookAtPosition(lookAtPosition);

    }
}

using ReGoap.Unity;
using UnityEngine;
using TrixieCore.Goap;

public class GuySensor : ReGoapSensor<GoapLabels, object>
{
    public Transform WallSensor;
    public Transform EdgeSensor;
    public Transform GroundSensor;

    private BaseEnemy enemyScript;
    private VisionComponent vision;
    private AttackAction attackAction;

    private void Start()
    {
        var agent = GetComponentInParent<EnemyGoapAgent>();
        enemyScript = agent.Parent;
        memory = agent.GetMemory();
    }

    public override void UpdateSensor()
    {
        var worldState = memory.GetWorldState();

        RaycastHit2D hit = Physics2D.Raycast(transform.position, WallSensor.position - transform.position, (WallSensor.position - transform.position).x, 1 << TrixieCore.TrixieLayers.GetMask(TrixieCore.Layers.Terrain));
        worldState.Set(GoapLabels.Guy_WallForward, hit.collider != null);
        
        hit = Physics2D.Raycast(transform.position, EdgeSensor.position - transform.position, (EdgeSensor.position - transform.position).magnitude, 1 << TrixieCore.TrixieLayers.GetMask(TrixieCore.Layers.Terrain));
        worldState.Set(GoapLabels.Guy_GroundedForward, hit.collider != null);

        hit = Physics2D.Raycast(transform.position, GroundSensor.position - transform.position, (GroundSensor.position - transform.position).magnitude, 1 << TrixieCore.TrixieLayers.GetMask(TrixieCore.Layers.Terrain));
        worldState.Set(GoapLabels.Guy_Grounded, hit.collider != null);
    }
}

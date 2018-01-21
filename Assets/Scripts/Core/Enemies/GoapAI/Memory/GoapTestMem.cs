using ReGoap.Unity;
using UnityEngine;
using ReGoap.Core;
using TrixieCore.Goap;

public class GoapTestMem : ReGoapMemory<GoapLabels, object>
{
    public float SensorsUpdateDelay = 0.3f;
    private EnemyGoapAgent agent;
    private float sensorsUpdateCooldown;
    private IReGoapSensor<GoapLabels, object>[] sensors;

    protected override void Awake()
    {
        agent = GetComponent<EnemyGoapAgent>();
        state = ReGoapState<GoapLabels, object>.Instantiate();
        sensors = agent.GetSensors();

        foreach (var sensor in sensors)
        {
            sensor.Init(this);
        }

        GetWorldState().Set(GoapLabels.HasCorrectProjectileColour, true);
    }

    private void Update()
    {
        if (Time.time > sensorsUpdateCooldown)
        {
            sensorsUpdateCooldown = Time.time + SensorsUpdateDelay;

            foreach (var sensor in sensors)
            {
                sensor.UpdateSensor();
            }
        }
    }
    
    public Vector2 GetLastKnownPlayerPosition()
    {
        if (GetWorldState().HasKey(GoapLabels.LastKnownPlayerPosition))
        {
            return (Vector2)GetWorldState().Get(GoapLabels.LastKnownPlayerPosition);
        }
        else
        {
            return Vector2.zero;
        }
    }

    public bool CheckCondition(GoapLabels conditionLabel)
    {
        if (GetWorldState().HasKey(conditionLabel))
        {
            return (bool)GetWorldState().Get(conditionLabel);
        }
        else
        {
            return false;
        }
    }



    


}

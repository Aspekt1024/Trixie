using ReGoap.Unity;
using UnityEngine;
using ReGoap.Core;
using TrixieCore.Goap;

public class GuyMemory : ReGoapMemory<GoapLabels, object>
{
    private EnemyGoapAgent agent;
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
    }

    private void Update()
    {
        foreach (var sensor in sensors)
        {
            sensor.UpdateSensor();
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

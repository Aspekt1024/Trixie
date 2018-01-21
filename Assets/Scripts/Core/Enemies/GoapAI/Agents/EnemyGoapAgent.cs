using System.Collections.Generic;
using ReGoap.Unity;
using TrixieCore.Goap;
using ReGoap.Core;
using ReGoap.Utilities;
using UnityEngine;

public class EnemyGoapAgent : ReGoapAgent<GoapLabels, object>
{
    public GameObject GoalsObject;
    public GameObject ActionsObject;
    public GameObject SensorsObject;

    private BaseEnemy parent;

    public BaseEnemy Parent
    {
        get { return parent; }
    }

    protected override void Awake()
    {
        base.Awake();
        parent = GetComponentInParent<BaseEnemy>();
    }

    public A GetAction<A>() where A : ReGoapAction<GoapLabels, object>
    {
        return ActionsObject.GetComponent<A>();
    }

    public S GetSensor<S>() where S : ReGoapSensor<GoapLabels, object>
    {
        return SensorsObject.GetComponent<S>();
    }

    public IReGoapSensor<GoapLabels, object>[] GetSensors()
    {
        return SensorsObject.GetComponentsInChildren<IReGoapSensor<GoapLabels, object>>();
    }

    public override IReGoapMemory<GoapLabels, object> GetMemory()
    {
        if (memory == null)
        {
            memory = GetComponent<IReGoapMemory<GoapLabels, object>>();
        }
        return memory;
    }

    public override void RefreshActionsSet()
    {
        actions = new List<IReGoapAction<GoapLabels, object>>(ActionsObject.GetComponents<IReGoapAction<GoapLabels, object>>());
    }
    
    public override void RefreshGoalsSet()
    {
        goals = new List<IReGoapGoal<GoapLabels, object>>(GoalsObject.GetComponents<IReGoapGoal<GoapLabels, object>>());
        possibleGoalsDirty = true;
    }
}

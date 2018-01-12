using System;
using System.Collections;
using UnityEngine;
using ReGoap.Core;
using ReGoap.Unity;
using TrixieCore.Goap;

[RequireComponent(typeof(EnemyProjectileSensor))]
public class SwitchProjectileColourAction : ReGoapAction<GoapLabels, object> {

    private GoapTestMem memory;
    private EnemyProjectileSensor projectileSensor;
    private ShootComponent shootComponent;

    protected override void Awake()
    {
        base.Awake();
        memory = GetComponent<GoapTestMem>();
        projectileSensor = GetComponent<EnemyProjectileSensor>();
    }

    public override ReGoapState<GoapLabels, object> GetEffects(ReGoapState<GoapLabels, object> goalState, IReGoapAction<GoapLabels, object> next = null)
    {
        effects.Clear();
        effects.Set(GoapLabels.HasCorrectProjectileColour, true);
        return effects;
    }

    public override ReGoapState<GoapLabels, object> GetPreconditions(ReGoapState<GoapLabels, object> goalState, IReGoapAction<GoapLabels, object> next = null)
    {
        preconditions.Clear();
        return preconditions;
    }

    public override bool CheckProceduralCondition(IReGoapAgent<GoapLabels, object> goapAgent, ReGoapState<GoapLabels, object> goalState, IReGoapAction<GoapLabels, object> next = null)
    {
        if (memory.CheckCondition(GoapLabels.HasCorrectProjectileColour))
        {
            return false;
        }
        return base.CheckProceduralCondition(goapAgent, goalState, next);
    }

    public override void Run(IReGoapAction<GoapLabels, object> previous, IReGoapAction<GoapLabels, object> next, IReGoapActionSettings<GoapLabels, object> settings, ReGoapState<GoapLabels, object> goalState, Action<IReGoapAction<GoapLabels, object>> done, Action<IReGoapAction<GoapLabels, object>> fail)
    {
        base.Run(previous, next, settings, goalState, done, fail);
        
        if (shootComponent == null)
        {
            shootComponent = GetComponent<EnemyGoapAgent>().Parent.GetComponent<ShootComponent>();
        }
        
        switch (projectileSensor.GetWeakColour())
        {
            case EnergyTypes.Colours.Blue:
                shootComponent.ProjectileSettings.ProjectileColour = EnergyTypes.Colours.Yellow;
                break;
            case EnergyTypes.Colours.Pink:
                break;
            case EnergyTypes.Colours.Yellow:
                shootComponent.ProjectileSettings.ProjectileColour = EnergyTypes.Colours.Blue;
                break;
            default:
                break;
        }

        doneCallback(this);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenShieldAbility : BaseShieldAbility
{
    protected override void Start()
    {
        base.Start();
        Colour = EnergyTypes.Colours.Green;
    }

    public override void ActivatePressed()
    {
        throw new System.NotImplementedException();
    }

    public override bool ActivateReleased()
    {
        throw new System.NotImplementedException();
    }

    public override void DisableShield()
    {
        throw new System.NotImplementedException();
    }

    public override void ReturnShield()
    {
        throw new System.NotImplementedException();
    }

    public override void UpdateCharge(float deltaTime)
    {
        throw new System.NotImplementedException();
    }
}

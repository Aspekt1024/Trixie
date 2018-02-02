﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldPower {

    private float maxCharge = 3;
    private float charge = 0;
    private BaseShieldAbility shieldParent;

    public ShieldPower(BaseShieldAbility parent, float maxCharge = 3, float initialCharge = 0)
    {
        this.maxCharge = maxCharge;
        charge = initialCharge;
        shieldParent = parent;
        UpdateShieldUI();
    }

    public bool ShieldFullyCharged()
    {
        return charge == maxCharge;
    }

    public void SetMaxCharge(float newMaxCharge)
    {
        maxCharge = newMaxCharge;
    }

    public float GetPower()
    {
        return charge;
    }

    public void AddPower(float powerToAdd = 1f)
    {
        charge = Mathf.Clamp(charge + powerToAdd, 0, maxCharge);
        UpdateShieldUI();
    }

    public void ReducePower(float powerToRemove)
    {
        AddPower(-powerToRemove);
    }

    private void UpdateShieldUI()
    {
        GameUIManager.SetShieldPower(shieldParent.Colour, charge / maxCharge);
    }

}

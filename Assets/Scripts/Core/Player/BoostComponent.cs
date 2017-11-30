using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostComponent : MonoBehaviour {
    
    public enum BoostRechargeTypes
    {
        OverTime, OnPickup, OnBullets, OnPad, InField
    }
    public BoostRechargeTypes RechargeType;

    public GameObject Jetpacks;
    public float MaxBoostTime = 1.2f;
    public float BoostVelocity = 10f;

    public float RechargeRate; // over time only

    [HideInInspector] public bool CanRecharge;

    private float boostAvailable;
    
    private enum States
    {
        None, Active
    }
    private States state;

    private void Start ()
    {
        Jetpacks.SetActive(false);
    }

    private void Update()
    {
        if (RechargeType == BoostRechargeTypes.OverTime && state == States.None && CanRecharge)
        {
            AddBoostCharge(Time.deltaTime * RechargeRate);
        }
        GameUIManager.UpdateBoostPercentage(boostAvailable / MaxBoostTime);
    }

    public void SetRechargeType(BoostRechargeTypes newType)
    {
        RechargeType = newType;
    }

    public void AddBoostCharge(float timeToAdd)
    {
        boostAvailable += timeToAdd;
        if (boostAvailable > MaxBoostTime)
        {
            boostAvailable = MaxBoostTime;
        }
    }

    public bool ActivateBoosters()
    {
        if (boostAvailable > 0f || InChargeField())
        {
            Jetpacks.SetActive(true);
            state = States.Active;
            return true;
        }
        return false;
    }

    public void DeactivateBoosters()
    {
        Jetpacks.SetActive(false);
        state = States.None;
    }

    public bool UseBoost(float boostTimeToUse)
    {
        if (RechargeType == BoostRechargeTypes.InField) return true;

        if (boostAvailable < boostTimeToUse)
        {
            state = States.None;
            return false;
        }

        boostAvailable -= boostTimeToUse;
        return true;
    }

    private bool InChargeField()
    {
        if (RechargeType == BoostRechargeTypes.InField)
        {
            return true;
        }
        return false;
    }
}

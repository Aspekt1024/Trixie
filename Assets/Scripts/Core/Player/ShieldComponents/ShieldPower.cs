using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldPower {

    private const int MAX_POWER = 3;

    private int bluePower;
    private int pinkPower;
    private int yellowPower;

    public ShieldPower()
    {
        bluePower = 3;
        pinkPower = 0;
        yellowPower = 0;
        UpdateShieldUI();
    }

    public bool ShieldFullyCharged(EnergyTypes.Colours colour)
    {
        int charge = 0;
        switch (colour)
        {
            case EnergyTypes.Colours.Blue:
                charge = bluePower;
                break;
            case EnergyTypes.Colours.Pink:
                charge = pinkPower;
                break;
            case EnergyTypes.Colours.Yellow:
                charge = yellowPower;
                break;
            default:
                charge = 0;
                break;
        }
        return charge == MAX_POWER;
    }

    public int GetPower(EnergyTypes.Colours colour)
    {
        switch (colour)
        {
            case EnergyTypes.Colours.Blue:
                return bluePower;
            case EnergyTypes.Colours.Pink:
                return pinkPower;
            case EnergyTypes.Colours.Yellow:
                return yellowPower;
            default:
                return 0;
        }
    }

    public void AddPower(EnergyTypes.Colours colour, int powerToAdd = 1)
    {
        switch (colour)
        {
            case EnergyTypes.Colours.Blue:
                bluePower = Mathf.Clamp(bluePower + 1, 0, MAX_POWER);
                break;
            case EnergyTypes.Colours.Pink:
                pinkPower = Mathf.Clamp(pinkPower + 1, 0, MAX_POWER);
                break;
            case EnergyTypes.Colours.Yellow:
                yellowPower = Mathf.Clamp(yellowPower + 1, 0, MAX_POWER);
                break;
            default:
                break;
        }
        UpdateShieldUI();
    }

    public void ReducePower(EnergyTypes.Colours colour, int powerToRemove)
    {
        AddPower(colour, -powerToRemove);
    }

    private void UpdateShieldUI()
    {
        GameUIManager.SetShieldPower(EnergyTypes.Colours.Blue, bluePower);
        GameUIManager.SetShieldPower(EnergyTypes.Colours.Yellow, yellowPower);
        GameUIManager.SetShieldPower(EnergyTypes.Colours.Pink, pinkPower);
    }

}

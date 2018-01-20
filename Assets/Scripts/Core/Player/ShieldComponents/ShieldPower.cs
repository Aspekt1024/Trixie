using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldPower {

    private const int MAX_POWER = 3;

    private int bluePower;
    private int redPower;
    private int greenPower;

    public ShieldPower()
    {
        bluePower = 3;
        redPower = 3;
        greenPower = 3;
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
            case EnergyTypes.Colours.Red:
                charge = redPower;
                break;
            case EnergyTypes.Colours.Green:
                charge = greenPower;
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
            case EnergyTypes.Colours.Red:
                return redPower;
            case EnergyTypes.Colours.Green:
                return greenPower;
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
            case EnergyTypes.Colours.Red:
                redPower = Mathf.Clamp(redPower + 1, 0, MAX_POWER);
                break;
            case EnergyTypes.Colours.Green:
                greenPower = Mathf.Clamp(greenPower + 1, 0, MAX_POWER);
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
        GameUIManager.SetShieldPower(EnergyTypes.Colours.Green, greenPower);
        GameUIManager.SetShieldPower(EnergyTypes.Colours.Red, redPower);
    }

}

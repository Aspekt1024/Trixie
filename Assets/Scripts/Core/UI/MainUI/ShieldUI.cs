using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShieldUI : MonoBehaviour {
    
    public ShieldUIModule BlueShieldModule;
    public ShieldUIModule PinkShieldModule;
    public ShieldUIModule YellowShieldModule;

    private void Start ()
    {
        SetShieldColour(EnergyTypes.Colours.Blue);
	}

    public void SetShieldColour(EnergyTypes.Colours shieldColour)
    {
        switch (shieldColour)
        {
            case EnergyTypes.Colours.Blue:
                BlueShieldModule.SelectShield();
                PinkShieldModule.DeselectShield();
                YellowShieldModule.DeselectShield();
                break;
            case EnergyTypes.Colours.Pink:
                PinkShieldModule.SelectShield();
                BlueShieldModule.DeselectShield();
                YellowShieldModule.DeselectShield();
                break;
            case EnergyTypes.Colours.Yellow:
                YellowShieldModule.SelectShield();
                PinkShieldModule.DeselectShield();
                BlueShieldModule.DeselectShield();
                break;
            default:
                break;
        }
    }

    public void SetShieldPower(EnergyTypes.Colours colour, int power)
    {
        switch (colour)
        {
            case EnergyTypes.Colours.Blue:
                BlueShieldModule.SetShieldPower(power);
                break;
            case EnergyTypes.Colours.Pink:
                PinkShieldModule.SetShieldPower(power);
                break;
            case EnergyTypes.Colours.Yellow:
                YellowShieldModule.SetShieldPower(power);
                break;
            default:
                break;
        }
    }

    public void HideShieldIndicator()
    {
        // TODO hide shields when not available
    }

    public void ShowShieldIndicator()
    {
        // TODO show shields when available
    }

}

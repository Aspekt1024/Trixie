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
            case EnergyTypes.Colours.Red:
                PinkShieldModule.SelectShield();
                BlueShieldModule.DeselectShield();
                YellowShieldModule.DeselectShield();
                break;
            case EnergyTypes.Colours.Green:
                YellowShieldModule.SelectShield();
                PinkShieldModule.DeselectShield();
                BlueShieldModule.DeselectShield();
                break;
            default:
                break;
        }
    }

    public void SetShieldPower(EnergyTypes.Colours colour, float powerRatio)
    {
        switch (colour)
        {
            case EnergyTypes.Colours.Blue:
                BlueShieldModule.SetShieldPower(powerRatio);
                break;
            case EnergyTypes.Colours.Red:
                PinkShieldModule.SetShieldPower(powerRatio);
                break;
            case EnergyTypes.Colours.Green:
                YellowShieldModule.SetShieldPower(powerRatio);
                break;
            default:
                break;
        }
    }

    public void HideShieldIndicator()
    {
        // TODO hide shields when not available
        PinkShieldModule.Hide();
        BlueShieldModule.Hide();
        YellowShieldModule.Hide();
    }

    public void ShowShieldIndicator()
    {
        // TODO show shields when available
        PinkShieldModule.Show();
        BlueShieldModule.Show();
        YellowShieldModule.Show();
    }

    public void ShowShieldsEnabled()
    {
        // TODO show animation
        ShowShieldIndicator();
    }

    public void ShowShieldsDisabled()
    {
        // TODO show animation
        HideShieldIndicator();
    }

}

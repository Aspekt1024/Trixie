using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TrixieCore.UI
{
    public class ShieldUI : MonoBehaviour
    {
        public ShieldUIModule BlueShieldModule;
        public ShieldUIModule RedShieldModule;
        public ShieldUIModule GreenShieldModule;

        private void Start()
        {
            SetShieldColour(EnergyTypes.Colours.Blue);
        }

        public void SetShieldColour(EnergyTypes.Colours shieldColour)
        {
            switch (shieldColour)
            {
                case EnergyTypes.Colours.Blue:
                    BlueShieldModule.SelectShield();
                    RedShieldModule.DeselectShield();
                    GreenShieldModule.DeselectShield();
                    break;
                case EnergyTypes.Colours.Red:
                    RedShieldModule.SelectShield();
                    BlueShieldModule.DeselectShield();
                    GreenShieldModule.DeselectShield();
                    break;
                case EnergyTypes.Colours.Green:
                    GreenShieldModule.SelectShield();
                    RedShieldModule.DeselectShield();
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
                    RedShieldModule.SetShieldPower(powerRatio);
                    break;
                case EnergyTypes.Colours.Green:
                    GreenShieldModule.SetShieldPower(powerRatio);
                    break;
                default:
                    break;
            }
        }

        public void HideShieldIndicator()
        {
            // TODO hide shields when not available
            RedShieldModule.Hide();
            BlueShieldModule.Hide();
            GreenShieldModule.Hide();
        }

        public void ShowShieldIndicator(ShieldStats stats)
        {
            if (stats.ColourUnlocked(EnergyTypes.Colours.Red)) { RedShieldModule.Show(); }
            if (stats.ColourUnlocked(EnergyTypes.Colours.Blue)) { BlueShieldModule.Show(); }
            if (stats.ColourUnlocked(EnergyTypes.Colours.Green)) { GreenShieldModule.Show(); }
        }

        public void ShowShieldsEnabled()
        {
            RedShieldModule.Enable();
            BlueShieldModule.Enable();
            GreenShieldModule.Enable();
        }

        public void ShowShieldsDisabled()
        {
            RedShieldModule.Disable();
            BlueShieldModule.Disable();
            GreenShieldModule.Disable();
        }
    }
}

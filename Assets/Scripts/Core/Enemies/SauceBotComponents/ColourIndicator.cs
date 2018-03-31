using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrixieCore.Units
{
    public class ColourIndicator : UnitEffect
    {
        public Transform BlueIndicator;
        public Transform RedIndicator;
        public Transform GreenIndicator;
        public ParticleSystem IndicatorEffect;
        
        private void Start()
        {
        }

        public void Show(EnergyTypes.Colours colour)
        {
            IndicatorEffect.Stop();
            switch (colour)
            {
                case EnergyTypes.Colours.Blue:
                    IndicatorEffect.transform.position = BlueIndicator.position;
                    break;
                case EnergyTypes.Colours.Red:
                    IndicatorEffect.transform.position = RedIndicator.position;
                    break;
                case EnergyTypes.Colours.Green:
                    IndicatorEffect.transform.position = GreenIndicator.position;
                    break;
                default:
                    break;
            }
            IndicatorEffect.Play();
        }

        public override void Stop()
        {
            IndicatorEffect.Stop();
        }
    }
}

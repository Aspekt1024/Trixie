using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrixieCore.Units
{
    public class UnitEffectsHandler
    {
        List<UnitEffect> effects = new List<UnitEffect>();

        public void ActivateEffect<T>() where T : UnitEffect
        {
            foreach (var effect in effects)
            {
                if (effect.GetType().Equals(typeof(T)))
                {
                    effect.Activate();
                    break;
                }
            }
        }

        public void Initialise(Transform effectsTf)
        {
            GetEffects(effectsTf);
        }
        
        private void GetEffects(Transform effectsTf)
        {
            UnitEffect[] availableEffects = effectsTf.GetComponentsInChildren<UnitEffect>();
            foreach (var effect in availableEffects)
            {
                effects.Add(effect);
            }
        }
    }
}

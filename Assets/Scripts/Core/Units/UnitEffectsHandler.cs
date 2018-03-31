using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrixieCore.Units
{
    public class UnitEffectsHandler
    {
        List<UnitEffect> effects = new List<UnitEffect>();

        public T GetEffect<T>() where T : UnitEffect
        {
            foreach (var effect in effects)
            {
                if (effect.GetType().Equals(typeof(T)))
                {
                    return (T)effect;
                }
            }
            return null;
        }

        public void Initialise(Transform effectsTf)
        {
            GetEffects(effectsTf);
        }

        public void StopAll()
        {
            foreach (var effect in effects)
            {
                effect.Stop();
            }
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

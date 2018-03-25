using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrixieCore.Units
{
    public class UnitAbilityHandler
    {
        List<UnitAbility> abilities = new List<UnitAbility>();

        public UnitAbility GetAbility<T>()
        {
            foreach (var ability in abilities)
            {
                if (typeof(T).IsInterface)
                {
                    if (ability is T)
                    {
                        return ability;
                    }
                }
                else
                {
                    if (ability.GetType().Equals(typeof(T)))
                    {
                        return ability;
                    }
                }
            }
            return null;
        }

        public void Initialise(Transform abilityTf)
        {
            GetEffects(abilityTf);
        }

        private void GetEffects(Transform abilityTf)
        {
            UnitAbility[] availableAbilities = abilityTf.GetComponentsInChildren<UnitAbility>();
            foreach (var ability in availableAbilities)
            {
                abilities.Add(ability);
            }
        }
    }
}


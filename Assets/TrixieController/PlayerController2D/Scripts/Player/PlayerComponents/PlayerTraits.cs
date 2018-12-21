using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aspekt.PlayerController
{
    public class PlayerTraits
    {

        public enum Traits
        {
            CanJump,
            CanDoubleJump,
            CanStomp,
            CanWallJump,
            CanBoost
        }

        private Dictionary<Traits, bool> traits = new Dictionary<Traits, bool>();

        public void AddTrait(Traits trait)
        {
            if (traits.ContainsKey(trait))
            {
                traits[trait] = true;
            }
            else
            {
                traits.Add(trait, true);
            }
        }

        public void Set(Traits trait, bool value)
        {
            if (traits.ContainsKey(trait))
            {
                traits[trait] = value;
            }
            else
            {
                traits.Add(trait, value);
            }
        }

        public bool HasTrait(Traits trait)
        {
            if (traits.ContainsKey(trait))
            {
                return traits[trait];
            }
            else
            {
                return false;
            }
        }
    }
}



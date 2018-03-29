using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrixieCore.Items;

namespace TrixieCore
{
    public class ShieldStats
    {
        private bool blueUnlocked;
        private bool yellowUnlocked;
        private bool pinkUnlocked;

        private bool shieldShootUnlocked;
        private bool shieldMeleeUnlocked;

        public ShieldStats()
        {
            // does this need any initialised variables?
        }

        public void ObtainedUnlock(ItemUnlock.UnlockType unlockType)
        {
            switch (unlockType)
            {
                case ItemUnlock.UnlockType.ShieldBlue:
                    UnlockColour(EnergyTypes.Colours.Blue);
                    break;
                case ItemUnlock.UnlockType.ShieldGreen:
                    UnlockColour(EnergyTypes.Colours.Green);
                    break;
                case ItemUnlock.UnlockType.ShieldRed:
                    UnlockColour(EnergyTypes.Colours.Red);
                    break;
                case ItemUnlock.UnlockType.ShieldShoot:
                    UnlockShoot();
                    break;
                case ItemUnlock.UnlockType.ShieldMelee:
                    UnlockMelee();
                    break;
                case ItemUnlock.UnlockType.Boosters:
                    Debug.LogWarning("Boosters unlock sent to shield stats - this shouldn't have happened");
                    break;
                default:
                    Debug.LogWarning(unlockType + " sent to shield stats - this has not been managed by the shield stats");
                    break;
            }
        }

        public bool ShieldUnlocked()
        {
            return blueUnlocked || yellowUnlocked || pinkUnlocked;
        }

        public bool ColourUnlocked(EnergyTypes.Colours colour)
        {
            switch (colour)
            {
                case EnergyTypes.Colours.Blue:
                    return blueUnlocked;
                case EnergyTypes.Colours.Red:
                    return pinkUnlocked;
                case EnergyTypes.Colours.Green:
                    return yellowUnlocked;
                default:
                    return false;
            }
        }

        public bool ShootUnlocked()
        {
            return shieldShootUnlocked;
        }

        public bool MeleeUnlocked()
        {
            return shieldMeleeUnlocked;
        }

        private void UnlockColour(EnergyTypes.Colours colour)
        {
            switch (colour)
            {
                case EnergyTypes.Colours.Blue:
                    blueUnlocked = true;
                    break;
                case EnergyTypes.Colours.Red:
                    pinkUnlocked = true;
                    break;
                case EnergyTypes.Colours.Green:
                    yellowUnlocked = true;
                    break;
                default:
                    break;
            }
        }

        private void UnlockShoot()
        {
            shieldShootUnlocked = true;
        }

        private void UnlockMelee()
        {
            shieldMeleeUnlocked = true;
        }
    }
}

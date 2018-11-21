using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrixieCore.Units
{
    public interface IDamageable
    {
        void TakeDamage(int damage, Vector2 direction, EnergyTypes.Colours damageType);
        bool IsDestroyed();
    }
}

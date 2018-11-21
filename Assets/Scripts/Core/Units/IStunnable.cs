using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrixieCore.Units
{
    public interface IStunnable
    {
        void Stun(Vector2 direction, float duration, EnergyTypes.Colours energyType);
        void Unstunned();
    }
}

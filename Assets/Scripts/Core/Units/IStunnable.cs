using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrixieCore.Units
{
    public interface IStunnable
    {
        void Stun(float duration, EnergyTypes.Colours energyType);
        void Unstunned();
    }
}

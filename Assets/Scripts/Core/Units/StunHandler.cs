using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrixieCore.Units
{
    public class StunHandler : MonoBehaviour
    {
        private StunEffect effect;
        private IStunnable unit;
        private float stunTimer;

        private enum States
        {
            None, Stunned
        }
        private States state;

        private void Start()
        {
            unit = GetComponent<IStunnable>();
            effect = GetComponent<BaseUnit>().GetEffect<StunEffect>();
        }

        private void Update()
        {
            if (stunTimer > 0)
            {
                stunTimer -= Time.deltaTime;
            }
            else if (state == States.Stunned)
            {
                state = States.None;
                unit.Unstunned();
                effect.Stop();
            }
        }

        public void BeginStun(float duration)
        {
            state = States.Stunned;
            stunTimer = duration;
            effect.Play();
        }
    }
}

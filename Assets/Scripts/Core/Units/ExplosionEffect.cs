using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrixieCore.Units
{
    public class ExplosionEffect : UnitEffect
    {
        private void Start()
        {
            gameObject.SetActive(false);
        }

        public void Explode()
        {
            gameObject.SetActive(true);
        }

        public override void Stop()
        {
            gameObject.SetActive(false);
        }
    }

}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrixieCore.Units
{
    public class StunEffect : UnitEffect
    {
        private Animator anim;

        private void Start()
        {
            anim = GetComponent<Animator>();
            gameObject.SetActive(false);
        }

        public void Play()
        {
            gameObject.SetActive(true);
        }

        public override void Stop()
        {
            gameObject.SetActive(false);
        }
    }
}


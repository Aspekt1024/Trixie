using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aspekt.PlayerController
{
    public class WallAttachEffect : PlayerEffect
    {
        private ParticleSystem particles;

        private void Start()
        {
            particles = GetComponent<ParticleSystem>();
            Stop();
        }

        private void Update()
        {
            if (!particles.isPlaying)
            {
                Stop();
            }
        }

        public override void Play()
        {
            gameObject.SetActive(true);
            particles.Stop();
            particles.Play();
        }

        public override void Stop()
        {
            gameObject.SetActive(false);
        }
    }
}


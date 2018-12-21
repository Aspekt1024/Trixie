using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Aspekt.PlayerController;

namespace Aspekt.TestObjects
{
    public class GravityField : MonoBehaviour
    {
        public float FieldStrength = 10f;

        private void Start()
        {
            ParticleSystem particles = GetComponentInChildren<ParticleSystem>();
            ParticleSystem.MainModule mainParticleModule = particles.main;
            mainParticleModule.startSpeed = FieldStrength;

            Collider2D coll = GetComponent<Collider2D>();
            mainParticleModule.startLifetime = coll.bounds.size.y / FieldStrength;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            Player player = collision.GetComponent<Player>();
            if (player != null)
            {
                player.EnterGravityField(FieldStrength);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            Player player = collision.GetComponent<Player>();
            if (player != null)
            {
                player.ExitGravityField();
            }
        }
    }
}


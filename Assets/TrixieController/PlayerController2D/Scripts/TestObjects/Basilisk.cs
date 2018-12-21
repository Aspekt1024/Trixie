using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Aspekt.PlayerController;

namespace Aspekt.TestObjects
{
    public class Basilisk : MonoBehaviour
    {
        public GameObject StunEffect;

        private void Start()
        {
            StunEffect.SetActive(false);
        }

        private void Update()
        {
            if (!StunEffect.GetComponent<ParticleSystem>().isPlaying)
            {
                StunEffect.SetActive(false);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            Player player = collision.GetComponent<Player>();
            if (player != null)
            {
                LolImABasilisk(player);
            }
        }

        private void LolImABasilisk(Player player)
        {
            if (player.CheckState(StateLabels.IsStunned)) return;
            player.Stun(2f);
            StunEffect.SetActive(true);
        }
    }
}

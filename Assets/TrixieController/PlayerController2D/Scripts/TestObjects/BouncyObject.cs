using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Aspekt.PlayerController;

namespace Aspekt.TestObjects
{
    public class BouncyObject : MonoBehaviour
    {
        public float Bounciness = 1.8f;

        private void OnCollisionEnter2D(Collision2D collision)
        {
            Player player = collision.collider.GetComponent<Player>();
            if (player != null)
            {
                player.Bounce(Bounciness);
            }
        }
        
    }
}

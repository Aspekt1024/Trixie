using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Aspekt.PlayerController;

namespace Aspekt.TestObjects
{
    public class ObstacleEnemy : MonoBehaviour
    {
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.collider.tag == "Player")
            {
                Player.Instance.Knockback(Player.Instance.transform.position - transform.position, 5f);
            }
        }
    }
}
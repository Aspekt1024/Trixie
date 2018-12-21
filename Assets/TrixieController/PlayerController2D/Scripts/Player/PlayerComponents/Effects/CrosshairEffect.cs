using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Aspekt.PlayerController;

namespace Aspekt.TestObjects
{
    public class CrosshairEffect : PlayerEffect
    {
        public float MoveSpeed = 12f;

        private ParticleSystem particles;

        private void Start()
        {
            gameObject.SetActive(false);
        }
        
        public override void Play()
        {
            gameObject.SetActive(true);
        }

        public override void Stop()
        {
            gameObject.SetActive(false);
        }

        public void Move(Vector3 moveDirection, float maxDistance)
        {
            transform.position += moveDirection * MoveSpeed * Time.deltaTime / Time.timeScale;

            Vector2 distVector = transform.position - Player.Instance.transform.position;
            if (distVector.magnitude > maxDistance)
            {
                transform.position = (Vector2)Player.Instance.transform.position + distVector.normalized * maxDistance;
            }
        }

        public void SetPosition(Vector2 position)
        {
            transform.position = position;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aspekt.TestObjects
{

    public class EndLevelButton : MonoBehaviour
    {
        public GameObject WinEffects;
        public Transform Button;

        private bool isPressed;
        private const float pressedDistance = 0.5f;
        
        private void Start()
        {
            WinEffects.SetActive(false);
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (isPressed) return;

            PlayerController.Player player = collider.GetComponent<PlayerController.Player>();
            if (player != null)
            {
                if (player.CheckState(PlayerController.StateLabels.IsStomping))
                {
                    StartCoroutine(Press());
                }
            }
        }

        private IEnumerator Press()
        {
            isPressed = true;
            float pressedDuration = 0.1f;
            float pressedTimer = 0f;

            while (pressedTimer < pressedDuration)
            {
                pressedTimer += Time.deltaTime;
                Button.transform.localPosition = new Vector3(0f, -pressedDistance * (pressedTimer / pressedDuration), 0f);
                yield return null;
            }

            WinEffects.SetActive(true);
        }
    }
}
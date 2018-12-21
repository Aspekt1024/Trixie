using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Aspekt.TestObjects;

namespace Aspekt.PlayerController
{
    public class BlinkAbility : PlayerAbility
    {
        public float TimeScale = 0.2f;
        public float BlinkDuration = 0.4f;
        public float MaxDistance = 17f;

        private CrosshairEffect crosshair;
        private IO.PlayerController controller;

        private float blinkTime;

        private enum States
        {
            Inactive, Active, Blinking
        }
        private States state;
        
        public void Init(Player player, IO.PlayerController playerController)
        {
            crosshair = player.GetEffect<CrosshairEffect>();
            controller = playerController;
        }

        public void Activate()
        {
            state = States.Active;
            Time.timeScale = TimeScale;
            crosshair.Play();
            crosshair.SetPosition(Player.Instance.transform.position);
        }

        public void Engage()
        {
            state = States.Blinking;
            crosshair.Stop();
            blinkTime = 0f;
            Player.Instance.transform.position = crosshair.transform.position;
        }

        public void Stop()
        {
            state = States.Inactive;
            Time.timeScale = 1f;
        }

        private void Update()
        {
            switch (state)
            {
                case States.Inactive:
                    break;
                case States.Active:
                    crosshair.Move(controller.GetDynamicDirection(crosshair.transform.position), MaxDistance);
                    break;
                case States.Blinking:
                    blinkTime += Time.deltaTime;
                    Time.timeScale = Mathf.Lerp(TimeScale, 1, blinkTime / BlinkDuration);
                    if (blinkTime > BlinkDuration)
                    {
                        Stop();
                    }
                    break;
                default:
                    break;
            }
        }
    }
}

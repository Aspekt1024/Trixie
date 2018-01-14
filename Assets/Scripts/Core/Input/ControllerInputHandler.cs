using System;
using System.Collections.Generic;
using UnityEngine;

namespace TrixieInput
{
    public class ControllerInputHandler
    {
        public string HORIZONTAL_AXIS = "Horizontal";
        public string SHOOT = "Fire1";
        public string AIM_AXIS_X = "AimX";
        public string AIM_AXIS_Y = "AimY";

        private bool movedByAxis;
        private bool shotByAxis;

#if UNITY_STANDALONE_WIN
        // These key bindings are for windows
        // see http://wiki.unity3d.com/index.php?title=Xbox360Controller
        private static KeyCode A_BUTTON = KeyCode.JoystickButton0;
        private static KeyCode B_BUTTON = KeyCode.JoystickButton1;
        private static KeyCode X_BUTTON = KeyCode.JoystickButton2;
        private static KeyCode Y_BUTTON = KeyCode.JoystickButton3;
        private static KeyCode L_BUMPER = KeyCode.JoystickButton4;
        private static KeyCode R_BUMPER = KeyCode.JoystickButton5;
        private static KeyCode BACK_BUTTON = KeyCode.JoystickButton6;
        private static KeyCode START_BUTTON = KeyCode.JoystickButton7;
        private static KeyCode L_STICK = KeyCode.JoystickButton8;
        private static KeyCode R_STICK = KeyCode.JoystickButton9;
#else
        // TODO define other platforms
#endif
        private GameManager gameManager;

        private Dictionary<KeyCode, Action> getKeyDownBindings = new Dictionary<KeyCode, Action>();
        private Dictionary<KeyCode, Action> getKeyUpBindings = new Dictionary<KeyCode, Action>();
        private Dictionary<KeyCode, Action> getKeyBindings = new Dictionary<KeyCode, Action>();

        private Vector2 aimDirection;

        public ControllerInputHandler(GameManager manager)
        {
            gameManager = manager;
            aimDirection = Vector2.right;

            getKeyDownBindings.Add(A_BUTTON, gameManager.JumpPressed);
            getKeyDownBindings.Add(L_BUMPER, gameManager.ShieldPressed);
            getKeyDownBindings.Add(X_BUTTON, gameManager.MeleePressed);
            getKeyDownBindings.Add(Y_BUTTON, gameManager.InteractPressed);
            getKeyDownBindings.Add(B_BUTTON, gameManager.CycleShieldColourPressed);
            getKeyDownBindings.Add(START_BUTTON, gameManager.ToggleMenu);

            getKeyUpBindings.Add(A_BUTTON, gameManager.JumpReleased);
            getKeyUpBindings.Add(L_BUMPER, gameManager.ShieldReleased);
        }

        public bool ProcessInput()
        {
            bool inputReceived = false;
            foreach (var binding in getKeyDownBindings)
            {
                if (Input.GetKeyDown(binding.Key))
                {
                    inputReceived = true;
                    gameManager.DirectInput(binding.Value);
                }
            }

            foreach (var binding in getKeyUpBindings)
            {
                if (Input.GetKeyUp(binding.Key))
                {
                    inputReceived = true;
                    gameManager.DirectInput(binding.Value);
                }
            }

            foreach (var binding in getKeyBindings)
            {
                if (Input.GetKey(binding.Key))
                {
                    inputReceived = true;
                    gameManager.DirectInput(binding.Value);
                }
            }

            if (Input.GetAxis(HORIZONTAL_AXIS) > 0.5f)
            {
                inputReceived = true;
                movedByAxis = true;
                gameManager.DirectInput(gameManager.MoveRightPressed);
            }
            else if (Input.GetAxis(HORIZONTAL_AXIS) < -0.5f)
            {
                inputReceived = true;
                movedByAxis = true;
                gameManager.DirectInput(gameManager.MoveLeftPressed);
            }
            else if (movedByAxis)
            {
                movedByAxis = false;
                gameManager.DirectInput(gameManager.MoveReleased);
            }

            if (Input.GetAxis(SHOOT) > 0.5f)
            {
                if (shotByAxis == false)
                {
                    shotByAxis = true;
                    inputReceived = true;
                    gameManager.DirectInput(gameManager.ShootPressed);
                }
            }
            else if (shotByAxis)
            {
                shotByAxis = false;
                gameManager.DirectInput(gameManager.ShootReleased);
            }

            return inputReceived;
        }

        public Vector2 GetAimDirection()
        {
            Vector2 aimAxis = new Vector2(Input.GetAxis(AIM_AXIS_X), Input.GetAxis(AIM_AXIS_Y));

            if (aimAxis.magnitude > 0.2f)
            {
                aimDirection = new Vector2(aimAxis.x, -aimAxis.y);
            }

            return aimDirection + (Vector2)Player.Instance.transform.position;
        }
    }
}


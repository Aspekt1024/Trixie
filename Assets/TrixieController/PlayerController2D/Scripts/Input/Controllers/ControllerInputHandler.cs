using Aspekt.PlayerController;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Aspekt.IO
{
    public class ControllerInputHandler
    {
        public const string VERTICAL_AXIS = "Vertical";
        public const string HORIZONTAL_AXIS = "Horizontal";
        public const string SHOOT = "Fire1";
        public const string AIM_AXIS_X = "AimX";
        public const string AIM_AXIS_Y = "AimY";

        private bool movedByAxis;
        private bool shotByAxis;

        public enum BindableButtons
        {
            A, B, X, Y, LBumper, RBumper
        }

        public readonly Dictionary<BindableButtons, KeyCode> ButtonBindings = new Dictionary<BindableButtons, KeyCode>()
        {
            {BindableButtons.A, A_BUTTON },
            {BindableButtons.B, B_BUTTON },
            {BindableButtons.X, X_BUTTON },
            {BindableButtons.Y, Y_BUTTON },
            {BindableButtons.LBumper, L_BUMPER },
            {BindableButtons.RBumper, R_BUMPER },
        };

#if UNITY_STANDALONE_WIN || UNITY_WEBGL
        // These key bindings are for windows
        // see http://wiki.unity3d.com/index.php?title=Xbox360Controller
        private const KeyCode A_BUTTON = KeyCode.JoystickButton0;
        private const KeyCode B_BUTTON = KeyCode.JoystickButton1;
        private const KeyCode X_BUTTON = KeyCode.JoystickButton2;
        private const KeyCode Y_BUTTON = KeyCode.JoystickButton3;
        private const KeyCode L_BUMPER = KeyCode.JoystickButton4;
        private const KeyCode R_BUMPER = KeyCode.JoystickButton5;
        private const KeyCode BACK_BUTTON = KeyCode.JoystickButton6;
        private const KeyCode START_BUTTON = KeyCode.JoystickButton7;
        private const KeyCode L_STICK = KeyCode.JoystickButton8;
        private const KeyCode R_STICK = KeyCode.JoystickButton9;
#else
        // TODO define other platforms
#endif

        private readonly Dictionary<KeyCode, PlayerControllerButtonHandler> getKeyDownBindings = new Dictionary<KeyCode, PlayerControllerButtonHandler>();
        private readonly Dictionary<KeyCode, PlayerControllerButtonHandler> getKeyUpBindings = new Dictionary<KeyCode, PlayerControllerButtonHandler>();

        private VirtualController virtualController;

        public ControllerInputHandler(VirtualController vc, List<Keybinding> bindings)
        {
            virtualController = vc;
            

            foreach (var binding in bindings)
            {
                getKeyDownBindings.Add(ButtonBindings[binding.ControllerBinding], binding.ButtonHandler);
                getKeyUpBindings.Add(ButtonBindings[binding.ControllerBinding], binding.ButtonHandler);
            }
        }

        public bool ProcessInput()
        {
            bool inputReceived = false;
            foreach (var binding in getKeyDownBindings)
            {
                if (Input.GetKeyDown(binding.Key))
                {
                    inputReceived = true;
                    virtualController.GotKeyDown(binding.Value);
                }
            }

            foreach (var binding in getKeyUpBindings)
            {
                if (Input.GetKeyUp(binding.Key))
                {
                    inputReceived = true;
                    virtualController.GotKeyUp(binding.Value);
                }
            }
            
            if (Input.GetAxis(HORIZONTAL_AXIS) > 0.5f)
            {
                inputReceived = true;
                movedByAxis = true;
                virtualController.InputReceived(InputLabels.MoveRightPressed);
            }
            else if (Input.GetAxis(HORIZONTAL_AXIS) < -0.5f)
            {
                inputReceived = true;
                movedByAxis = true;
                virtualController.InputReceived(InputLabels.MoveLeftPressed);
            }
            else if (movedByAxis)
            {
                movedByAxis = false;
                virtualController.InputReceived(InputLabels.MoveReleased);
            }

            if (Input.GetAxis(SHOOT) > 0.5f)
            {
                if (shotByAxis == false)
                {
                    shotByAxis = true;
                    inputReceived = true;
                    virtualController.InputReceived(InputLabels.ShootPressed);
                }
            }
            else if (shotByAxis)
            {
                shotByAxis = false;
                virtualController.InputReceived(InputLabels.ShootReleased);
            }

            return inputReceived;
        }

        public Vector2 GetAimDirection()
        {
            Vector2 aimAxis = new Vector2(Input.GetAxis(AIM_AXIS_X), Input.GetAxis(AIM_AXIS_Y));
            Vector2 aimDirection = Vector2.zero;
            if (aimAxis.magnitude > 0.3f)
            {
                aimDirection = new Vector2(aimAxis.x, -aimAxis.y);
            }

            return aimDirection;
        }

        public Vector2 GetMoveDirection()
        {
            Vector2 moveAxis = new Vector2(Input.GetAxis(HORIZONTAL_AXIS), Input.GetAxis(VERTICAL_AXIS));
            Vector2 moveDirection = Vector2.zero;

            if (moveAxis.magnitude > 0.3f)
            {
                moveDirection = moveAxis;
            }
            return moveDirection;
        }
    }
}


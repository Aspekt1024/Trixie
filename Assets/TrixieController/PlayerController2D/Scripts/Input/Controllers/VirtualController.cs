using Aspekt.PlayerController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aspekt.IO
{
    public class VirtualController
    {
        internal enum InputMode
        {
            Keyboard, Controller
        }
        internal InputMode Mode { get; private set; }

        private PlayerController playerController;

        private KeyboardInputHandler keyboardInput;
        private ControllerInputHandler controllerInput;

        public VirtualController(PlayerController pc, List<Keybinding> bindings)
        {
            playerController = pc;
            
            keyboardInput = new KeyboardInputHandler(this, bindings);
            controllerInput = new ControllerInputHandler(this, bindings);
        }

        public void CheckForInput()
        {
            bool receivedControllerInput = controllerInput.ProcessInput();
            bool receivedKeyboardMouseInput = keyboardInput.ProcessInput();

            if (receivedControllerInput)
            {
                Mode = InputMode.Controller;
            }
            else if (receivedKeyboardMouseInput)
            {
                Mode = InputMode.Keyboard;
            }
        }

        public void InputReceived(InputLabels input)
        {
            playerController.OnInputReceived(input);
        }

        public Vector2 GetAimDirection(Vector2 relativeToPoint)
        {
            switch (Mode)
            {
                case InputMode.Keyboard:
                    return keyboardInput.GetAimDirection(relativeToPoint);
                case InputMode.Controller:
                    return controllerInput.GetAimDirection();
                default:
                    return Vector2.zero;
            }
        }

        public Vector2 GetMoveDirection()
        {
            switch (Mode)
            {
                case InputMode.Keyboard:
                    return keyboardInput.GetMoveDirection();
                case InputMode.Controller:
                    return controllerInput.GetMoveDirection();
                default:
                    return Vector2.zero;
            }
        }

        public void GotKeyDown(PlayerControllerButtonHandler handler)
        {
            handler.Pressed();
        }

        public void GotKeyUp(PlayerControllerButtonHandler handler)
        {
            handler.Released();
        }
    }
}


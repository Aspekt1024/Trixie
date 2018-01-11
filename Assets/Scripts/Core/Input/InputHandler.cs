using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrixieInput
{
    public class InputHandler
    {

        private enum InputMode
        {
            Keyboard, Controller
        }
        private InputMode mode;

        private GameManager gameManager;
        private KeyboardInputHandler keyboardInput;
        private ControllerInputHandler controllerInput;

        public InputHandler(GameManager manager)
        {
            gameManager = manager;
            keyboardInput = new KeyboardInputHandler(manager);
            controllerInput = new ControllerInputHandler(manager);
        }

        public void ProcessInput()
        {
            bool receivedControllerInput = controllerInput.ProcessInput();
            bool receivedKeyboardMouseInput = keyboardInput.ProcessInput();

            if (receivedControllerInput)
            {
                mode = InputMode.Controller;
            }
            else if (receivedKeyboardMouseInput)
            {
                mode = InputMode.Keyboard;
            }
        }

        public Vector2 GetAimDirection()
        {
            switch (mode)
            {
                case InputMode.Keyboard:
                    return Camera.main.ScreenToWorldPoint(Input.mousePosition);
                case InputMode.Controller:
                    return controllerInput.GetAimDirection();
                default:
                    return Vector2.right;
            }
        }

    }
}


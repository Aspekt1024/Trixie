using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Aspekt.PlayerController;

namespace Aspekt.IO
{
    public class KeyboardInputHandler
    {
        // Can use system.enum.parse to serialize these
        private const KeyCode MOVE_LEFT = KeyCode.A;
        private const KeyCode MOVE_RIGHT = KeyCode.D;
        
        private Dictionary<KeyCode, PlayerControllerButtonHandler> getKeyDownBindings = new Dictionary<KeyCode, PlayerControllerButtonHandler>();
        private Dictionary<KeyCode, PlayerControllerButtonHandler> getKeyUpBindings = new Dictionary<KeyCode, PlayerControllerButtonHandler>();
        
        private VirtualController virtualController;

        private bool movedByKey;

        public KeyboardInputHandler(VirtualController vc, List<Keybinding> bindings)
        {
            virtualController = vc;

            foreach (var binding in bindings)
            {
                getKeyDownBindings.Add(binding.KeyboardBinding, binding.ButtonHandler);
                getKeyUpBindings.Add(binding.KeyboardBinding, binding.ButtonHandler);
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

            if (Input.GetKeyDown(MOVE_LEFT) || Input.GetKey(MOVE_LEFT))
            {
                virtualController.InputReceived(InputLabels.MoveLeftPressed);
                inputReceived = true;
                movedByKey = true;
            }
            else if (Input.GetKeyDown(MOVE_RIGHT) || (Input.GetKey(MOVE_RIGHT) && !Input.GetKeyUp(MOVE_RIGHT)))
            {
                virtualController.InputReceived(InputLabels.MoveRightPressed);
                inputReceived = true;
                movedByKey = true;
            }
            else if (movedByKey)
            {
                virtualController.InputReceived(InputLabels.MoveReleased);
                movedByKey = false;
            }
            
            return inputReceived;
        }

        public Vector2 GetAimDirection(Vector2 relativeToPoint)
        {
            return (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - relativeToPoint;
        }

        public Vector2 GetMoveDirection()
        {
            Vector2 moveDirection = new Vector2();
            if (Input.GetKey(MOVE_LEFT))
            {
                moveDirection += Vector2.left;
            }
            if (Input.GetKey(MOVE_RIGHT))
            {
                moveDirection += Vector2.right;
            }
            if (Input.GetKey(KeyCode.S))
            {
                moveDirection += Vector2.down;
            }
            if (Input.GetKey(KeyCode.W))
            {
                moveDirection += Vector2.up;
            }
            return moveDirection;
        }
    }
}


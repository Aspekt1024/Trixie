using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace TrixieInput
{
    public class KeyboardInputHandler
    {
        // Can use system.enum.parse to serialize these
        private KeyCode MOVE_LEFT = KeyCode.A;
        private KeyCode MOVE_RIGHT = KeyCode.D;

        private GameManager gameManager;
        private Dictionary<KeyCode, Action> getKeyDownBindings = new Dictionary<KeyCode, Action>();
        private Dictionary<KeyCode, Action> getKeyUpBindings = new Dictionary<KeyCode, Action>();
        private Dictionary<KeyCode, Action> getKeyBindings = new Dictionary<KeyCode, Action>();
        
        public KeyboardInputHandler(GameManager manager)
        {
            gameManager = manager;

            getKeyDownBindings.Add(KeyCode.A, gameManager.MoveLeftPressed);
            getKeyDownBindings.Add(KeyCode.D, gameManager.MoveRightPressed);
            getKeyDownBindings.Add(KeyCode.W, gameManager.JumpPressed);
            getKeyDownBindings.Add(KeyCode.Space, gameManager.JumpPressed);
            getKeyDownBindings.Add(KeyCode.F, gameManager.MeleePressed);
            getKeyDownBindings.Add(KeyCode.Mouse0, gameManager.ShootPressed);
            getKeyDownBindings.Add(KeyCode.Mouse1, gameManager.ShieldPressed);
            getKeyDownBindings.Add(KeyCode.R, gameManager.CycleShieldColourPressed);
            getKeyDownBindings.Add(KeyCode.Escape, gameManager.ToggleMenu);
            
            getKeyUpBindings.Add(KeyCode.W, gameManager.JumpReleased);
            getKeyUpBindings.Add(KeyCode.Space, gameManager.JumpReleased);
            getKeyUpBindings.Add(KeyCode.Mouse0, gameManager.ShootReleased);
            getKeyUpBindings.Add(KeyCode.Mouse1, gameManager.ShieldReleased);
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
            
            if (Input.GetKeyUp(MOVE_RIGHT) || Input.GetKeyUp(MOVE_LEFT))
            {
                if (!Input.GetKey(MOVE_LEFT) && !Input.GetKey(MOVE_RIGHT))
                {
                    gameManager.DirectInput(gameManager.MoveReleased);
                }
                else
                {
                    inputReceived = true;
                }
            }
            
            return inputReceived;
        }
    }
}


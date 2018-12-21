using System.Collections.Generic;
using UnityEngine;
using Aspekt.PlayerController;
using System;

namespace Aspekt.IO
{
    public class PlayerController : MonoBehaviour
    {
        private VirtualController controller;

        private Player player;
        private MoveComponent move;

        private bool gamePaused;

        public List<Keybinding> KeyBindings = new List<Keybinding>();
        private List<PlayerControllerButtonHandler> buttonHandlers = new List<PlayerControllerButtonHandler>();
        
        private void Start()
        {
            player = GetComponent<Player>();
            
            // Create button handlers from keybindings
            for (int i = 0; i < KeyBindings.Count; i++)
            {
                Type type = Type.GetType(KeyBindings[i].HandlerTypeString);
                var binding = KeyBindings[i];
                binding.ButtonHandler = (PlayerControllerButtonHandler)Activator.CreateInstance(type, player, this);
                KeyBindings[i] = binding;
                buttonHandlers.Add(KeyBindings[i].ButtonHandler);
            }

            controller = new VirtualController(this, KeyBindings);
            
            move = player.GetAbility<MoveComponent>();
        }
        
        private void Update()
        {
            controller.CheckForInput();

            foreach (var button in buttonHandlers)
            {
                button.Tick(Time.deltaTime);
            }
        }

        public static PlayerController Get()
        {
            return Player.Instance.GetComponent<PlayerController>();
        }

        public Vector2 GetMoveDirection()
        {
            return controller.GetMoveDirection();
        }

        public Vector2 GetAimDirection(Vector2 relativeToPoint)
        {
            return controller.GetAimDirection(relativeToPoint);
        }

        /// <summary>
        /// Returns the controller movement direction, or the mouse aim direction
        /// </summary>
        public Vector2 GetDynamicDirection(Vector2 relativeToPoint)
        {
            if (controller.Mode == VirtualController.InputMode.Controller)
            {
                return controller.GetMoveDirection();
            }
            else
            {
                return controller.GetAimDirection(relativeToPoint);
            }
        }

        public void OnInputReceived(InputLabels input)
        {
            if (InMenus())
            {
                ActionInputsInMenu(input);
            }
            else if (InGame())
            {
                if (player.IsIncapacitated)
                {
                    ActionInputsInGameIncapacitated(input);
                }
                else
                {
                    ActionInputsInGame(input);
                }
            }

            ActionInputsAlways(input);

        }

        private void ActionInputsInMenu(InputLabels input)
        {
        }

        private void ActionInputsInGame(InputLabels input)
        {
            switch (input)
            {
                case InputLabels.MoveLeftPressed:
                    move.MoveLeft();
                    break;
                case InputLabels.MoveRightPressed:
                    move.MoveRight();
                    break;
                default:
                    break;
            }
        }

        private void ActionInputsInGameIncapacitated(InputLabels input)
        {

        }

        private void ActionInputsAlways(InputLabels input)
        {
            switch (input)
            {
                case InputLabels.ToggleMenu:
                    TogglePaused();
                    break;
                case InputLabels.MoveReleased:
                    move.MoveReleased();
                    break;
                default:
                    break;
            }
        }

        private bool InMenus()
        {
            return gamePaused;
        }

        private bool InGame()
        {
            return !gamePaused;
        }

        private void TogglePaused()
        {
            gamePaused = !gamePaused;
            if (gamePaused)
            {
                Time.timeScale = 0f;
            }
            else
            {
                Time.timeScale = 1f;
            }
        }
    }
}

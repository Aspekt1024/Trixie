using Aspekt.PlayerController;
using System;
using UnityEngine;

namespace Aspekt.IO
{
    [Serializable]
    public struct Keybinding
    {
        public string HandlerTypeString;
        public KeyCode KeyboardBinding;
        public ControllerInputHandler.BindableButtons ControllerBinding;

        public PlayerControllerButtonHandler ButtonHandler { get; set; }

        public Keybinding(Type handler, KeyCode keyboardBinding, ControllerInputHandler.BindableButtons controllerBinding)
        {
            ButtonHandler = null;
            HandlerTypeString = handler.ToString();
            KeyboardBinding = keyboardBinding;
            ControllerBinding = controllerBinding;
        }
    }
}

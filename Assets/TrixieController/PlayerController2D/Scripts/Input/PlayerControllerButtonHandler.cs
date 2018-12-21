using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aspekt.PlayerController
{
    public abstract class PlayerControllerButtonHandler
    {
        protected Player player;
        protected IO.PlayerController controller;

        public PlayerControllerButtonHandler(Player player, IO.PlayerController controller)
        {
            this.player = player;
            this.controller = controller;
        }

        public abstract void Pressed();
        public abstract void Released();
        public virtual void Tick(float deltaTime) { }
    }
}


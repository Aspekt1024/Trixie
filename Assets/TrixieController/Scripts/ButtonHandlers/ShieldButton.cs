﻿using Aspekt.PlayerController;

namespace TrixieCore.Controller
{
    public class ShieldButton : PlayerControllerButtonHandler
    {
        private ShieldAbility shield;
        
        public ShieldButton(Player player, Aspekt.IO.PlayerController controller) : base(player, controller)
        {
            shield = player.GetAbility<ShieldAbility>();
        }

        public override void Pressed()
        {
            shield.ShieldActivatePressed();
        }

        public override void Released()
        {
            shield.ShieldDeactivatePressed();
        }
    }
}

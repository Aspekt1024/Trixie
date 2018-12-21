using Aspekt.PlayerController;
using System;

namespace TrixieCore.Controller
{
    public class ShieldButton : PlayerControllerButtonHandler
    {
        private ShieldComponent shield;
        
        public ShieldButton(Player player, Aspekt.IO.PlayerController controller) : base(player, controller)
        {
            shield = player.GetAbility<ShieldComponent>();
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

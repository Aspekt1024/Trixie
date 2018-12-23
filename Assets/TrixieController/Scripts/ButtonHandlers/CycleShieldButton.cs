using Aspekt.PlayerController;

namespace TrixieCore.Controller
{
    public class CycleShieldButton : PlayerControllerButtonHandler
    {
        private ShieldAbility shield;

        public CycleShieldButton(Player player, Aspekt.IO.PlayerController controller) : base(player, controller)
        {
            shield = player.GetAbility<ShieldAbility>();
        }

        public override void Pressed()
        {
            shield.CycleShieldColour();
        }

        public override void Released()
        {
        }
    }
}

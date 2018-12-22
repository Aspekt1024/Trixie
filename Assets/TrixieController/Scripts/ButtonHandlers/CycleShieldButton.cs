using Aspekt.PlayerController;

namespace TrixieCore.Controller
{
    public class CycleShieldButton : PlayerControllerButtonHandler
    {
        private ShieldComponent shield;

        public CycleShieldButton(Player player, Aspekt.IO.PlayerController controller) : base(player, controller)
        {
            shield = player.GetAbility<ShieldComponent>();
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

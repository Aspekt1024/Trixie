using Aspekt.PlayerController;

namespace TrixieCore.Controller
{
    public class UtilityButton : PlayerControllerButtonHandler
    {
        private ShieldAbility shield;

        public UtilityButton(Player player, Aspekt.IO.PlayerController controller) : base(player, controller)
        {
            shield = player.GetAbility<ShieldAbility>();
        }

        public override void Pressed()
        {
            shield.UtilityPressed();
        }

        public override void Released()
        {
            shield.UtilityReleased();
        }
    }
}

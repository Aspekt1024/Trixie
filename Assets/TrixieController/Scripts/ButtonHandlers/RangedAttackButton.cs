using Aspekt.PlayerController;

namespace TrixieCore.Controller
{
    public class RangedAttackButton : PlayerControllerButtonHandler
    {
        private RangedAbility rangedAbility;

        public RangedAttackButton(Player player, Aspekt.IO.PlayerController controller) : base(player, controller)
        {
            rangedAbility = player.GetAbility<RangedAbility>();
        }

        public override void Pressed()
        {
            rangedAbility.RangedPressed();
        }

        public override void Released()
        {
            rangedAbility.RangedReleased();
        }
    }
}

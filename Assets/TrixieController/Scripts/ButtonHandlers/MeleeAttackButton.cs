using Aspekt.PlayerController;

namespace TrixieCore.Controller
{
    public class MeleeAttackButton : PlayerControllerButtonHandler
    {
        private MeleeAbility melee;

        public MeleeAttackButton(Player player, Aspekt.IO.PlayerController controller) : base(player, controller)
        {
            melee = player.GetAbility<MeleeAbility>();
        }

        public override void Pressed()
        {
            melee.MeleePressed();
        }

        public override void Released()
        {
            melee.MeleeReleased();
        }
    }
}

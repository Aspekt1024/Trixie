using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aspekt.PlayerController
{
    public class ShootButton : PlayerControllerButtonHandler
    {
        private ShootAbility shoot;

        public ShootButton(Player player, IO.PlayerController controller) : base(player, controller)
        {
            shoot = player.GetAbility<ShootAbility>();
            shoot.Init(player, controller);
        }

        public override void Pressed()
        {
            shoot.Activate();
        }

        public override void Released()
        {
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aspekt.PlayerController
{
    public class SpecialButton : PlayerControllerButtonHandler
    {
        private BlinkAbility blink;

        public SpecialButton(Player player, IO.PlayerController controller) : base(player, controller)
        {
            blink = player.GetAbility<BlinkAbility>();
            blink.Init(player, controller);
        }

        public override void Pressed()
        {
            blink.Activate();
        }

        public override void Released()
        {
            blink.Engage();
        }
    }
}

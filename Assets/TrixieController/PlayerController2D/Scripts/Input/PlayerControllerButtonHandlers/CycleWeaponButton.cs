using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aspekt.IO;
using UnityEngine;

namespace Aspekt.PlayerController
{
    public class CycleWeaponButton : PlayerControllerButtonHandler
    {
        private ShootAbility shoot;

        public CycleWeaponButton(Player player, IO.PlayerController controller) : base(player, controller)
        {
            shoot = player.GetAbility<ShootAbility>();
        }

        public override void Pressed()
        {
            shoot.CycleBullet();
        }

        public override void Released()
        {
        }
    }
}

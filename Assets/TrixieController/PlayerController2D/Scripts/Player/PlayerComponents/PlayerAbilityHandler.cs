using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aspekt.PlayerController
{
    public class PlayerAbilityHandler : MonoBehaviour
    {
        public PlayerTraits.Traits[] EnabledTraits = new PlayerTraits.Traits[0];

        private PlayerAbility[] abilities;
        private Player player;

        private void Awake()
        {
            player = gameObject.GetComponentInParent<Player>();
            abilities = transform.GetComponentsInChildren<PlayerAbility>();
        }

        private void Start()
        {
            foreach (var trait in EnabledTraits)
            {
                player.SetTrait(trait, true);
            }
        }

        public T GetAbility<T>() where T : PlayerAbility
        {
            for (int i = 0; i < abilities.Length; i++)
            {
                if (abilities[i].GetType().Equals(typeof(T)))
                {
                    return (T)abilities[i];
                }
            }
            return null;
        }
    }
}

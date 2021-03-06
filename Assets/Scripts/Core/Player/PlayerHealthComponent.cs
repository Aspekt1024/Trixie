﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrixieCore
{
    public class PlayerHealthComponent : MonoBehaviour
    {

        public int MaxHealth = 5;
        public bool InvincibilityMode = false;

        public float InvincibilityTimeAfterDamaged = 3f;
        public float FlashFrequency = 10f;

        public SpriteRenderer SpriteRenderer;

        private enum States
        {
            None, TakingDamage
        }
        private States state;

        private int health;

        private void Start()
        {
            health = MaxHealth;
        }

        private const string INVINCIBLE_LAYER = "PlayerInvincible";
        private const string NORMAL_LAYER = "Player";

        public void TakeDamage(int damage = 1)
        {
            if (InvincibilityMode || state == States.TakingDamage) return;

            state = States.TakingDamage;
            StartCoroutine(DamageAnimation());

            health -= damage;
            health = Mathf.Max(0, health);

            UI.GameUIManager.UpdateHealth(health);
        }

        public void AddHealth(int additionalHealth = 1)
        {
            health += additionalHealth;
            health = Mathf.Min(MaxHealth, health);
            UI.GameUIManager.UpdateHealth(health);
        }

        public bool IsAlive()
        {
            return health > 0;
        }

        public bool IsDead()
        {
            return health <= 0;
        }

        public int GetHealth()
        {
            return health;
        }

        private IEnumerator DamageAnimation()
        {
            float timeDamageTaken = Time.time;
            float flashTimer = 0f;
            float flashDuration = 0.5f / FlashFrequency;
            bool flashOn = false;
            SetInvincibleLayer();

            Color damagedColour = new Color(1f, 0f, 0f, 0.2f);

            while (timeDamageTaken + InvincibilityTimeAfterDamaged > Time.time)
            {
                flashTimer += Time.deltaTime;
                if (flashTimer > flashDuration)
                {
                    flashTimer = 0f;
                    flashOn = !flashOn;
                    if (flashOn)
                    {
                        SpriteRenderer.color = damagedColour;
                    }
                    else
                    {
                        SpriteRenderer.color = Color.white;
                    }
                }
                yield return null;
            }
            SetNormalLayer();
            SpriteRenderer.color = Color.white;
            state = States.None;
        }

        private void SetInvincibleLayer()
        {
            gameObject.layer = LayerMask.NameToLayer(INVINCIBLE_LAYER);
        }

        private void SetNormalLayer()
        {
            gameObject.layer = LayerMask.NameToLayer(NORMAL_LAYER);
            return;
        }
    }
}


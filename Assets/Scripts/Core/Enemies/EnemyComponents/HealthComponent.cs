using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthComponent : MonoBehaviour {

    public int MaxHealth = 5;
    public float InvincibilityTimeAfterHit = 0f;

    private enum States
    {
        None, Invincible
    }
    private States state;

    private int health;

    private void Start()
    {
        health = MaxHealth;
    }

    public void TakeDamage(int damage = 1)
    {
        if (state == States.Invincible) return;

        if (InvincibilityTimeAfterHit > 0f)
        {
            state = States.Invincible;
        }
        
        health = Mathf.Clamp(health - damage, 0, MaxHealth);
    }

    public void AddHealth(int additionalHealth = 1)
    {
        health = Mathf.Clamp(health - additionalHealth, 0, MaxHealth);
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
}

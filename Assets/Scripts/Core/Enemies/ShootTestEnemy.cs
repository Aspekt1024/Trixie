using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootTestEnemy : BaseEnemy {

    public float ShootCooldown = 1f;
    public Transform Turrets;

    private float cooldown;

    private ShootComponent[] shooters;

    private float cdTimer = 0f;

    private void Start()
    {
        cooldown = Random.Range(ShootCooldown, ShootCooldown * 2f);
        shooters = GetComponents<ShootComponent>();
    }

    private void Update()
    {
        if (Turrets != null)
        {
            Vector2 distVector = Player.Instance.transform.position - Turrets.transform.position;
            float targetRotation = Mathf.Atan2(distVector.y, distVector.x) * Mathf.Rad2Deg;
            Turrets.eulerAngles = new Vector3(0f, 0f, targetRotation);
        }

        if (cdTimer < cooldown)
        {
            cdTimer += Time.deltaTime;
        }
        else
        {
            cooldown = Random.Range(ShootCooldown, ShootCooldown * 2f);
            cdTimer = 0f;
            foreach (ShootComponent shooter in shooters)
            {
                shooter.Shoot(Player.Instance.gameObject);
            }
        }
    }

    public override void DamageEnemy(Vector2 direction, int damage = 1)
    {
        Debug.Log(name + "says 'Ow!'");
    }
}

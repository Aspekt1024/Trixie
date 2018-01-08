using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootTestEnemy : MonoBehaviour {

    public float ShootCooldown = 1f;
    public Transform Turrets;

    private ShootComponent[] shooters;

    private float cdTimer = 0f;

    private void Start()
    {
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

        if (cdTimer < ShootCooldown)
        {
            cdTimer += Time.deltaTime;
        }
        else
        {
            cdTimer = 0f;
            foreach (ShootComponent shooter in shooters)
            {
                shooter.Shoot(Player.Instance.gameObject);
            }
        }
    }
}

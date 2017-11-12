using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBot2 : BaseEnemy {

    public GameObject Turrets;
    public float ProjectileCooldown;
    public float ProjectileSpeed;

    private const float playerInRangeDistance = 10f;

    private float timeLastShot;

    private void Start()
    {
        timeLastShot = Time.deltaTime;
    }

    private void Update()
    {
        Vector2 distVector = Player.Instance.transform.position - transform.position;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, distVector, playerInRangeDistance, 1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("Terrain"));
        
        if (hit.collider != null && hit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            float targetRotation = Mathf.Atan2(distVector.y, distVector.x) * Mathf.Rad2Deg;
            Turrets.transform.localEulerAngles = new Vector3(0f, 0f, targetRotation);

            if (timeLastShot + ProjectileCooldown < Time.time)
            {
                Shoot();
            }
        }
    }

    private void Shoot()
    {
        GameObject projectile = ObjectPooler.Instance.GetPooledProjectile(Projectile.ProjectileType.Pink);
        if (projectile == null) return;

        projectile.SetActive(true);
        projectile.transform.position = Turrets.transform.position;
        projectile.transform.localRotation = Turrets.transform.rotation;
        projectile.transform.localEulerAngles = new Vector3(0f, 0f, projectile.transform.localEulerAngles.z - 90f);
        projectile.GetComponent<Rigidbody2D>().velocity = Turrets.transform.right * ProjectileSpeed;

        timeLastShot = Time.time;
    }

}

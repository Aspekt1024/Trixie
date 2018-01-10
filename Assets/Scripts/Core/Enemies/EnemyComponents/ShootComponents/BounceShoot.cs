using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceShoot : ShootComponent {

    public EnergyTypes.Colours Colour;

    public override void Shoot(GameObject target)
    {
        this.target = target;
        Shoot();
    }

    public override void Shoot()
    {
        ActivateProjecile();
    }

    private void ActivateProjecile()
    {
        Projectile projectilePrefabScript = ProjectilePrefab.GetComponent<Projectile>();
        GameObject projectile = ObjectPooler.Instance.GetPooledProjectile(projectilePrefabScript.name);
        if (projectile == null) return;

        Vector2 distVector = transform.right;
        if (target != null)
        {
            distVector = target.transform.position - transform.position;
        }
        float targetRotation = Mathf.Atan2(distVector.y, distVector.x) * Mathf.Rad2Deg;

        projectile.SetActive(true);
        projectile.GetComponent<Projectile>().Activate(ShootPoint.transform.position, targetRotation, ProjectileSpeed, Colour);
        projectile.GetComponent<Projectile>().BouncesOffTerrain = true;
    }
}

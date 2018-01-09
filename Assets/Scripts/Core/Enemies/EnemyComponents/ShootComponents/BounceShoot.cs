using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceShoot : ShootComponent {

    public override void Shoot(GameObject target)
    {
        Target = target;
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
        if (Target != null)
        {
            distVector = Target.transform.position - transform.position;
        }
        float targetRotation = Mathf.Atan2(distVector.y, distVector.x) * Mathf.Rad2Deg;

        projectile.SetActive(true);
        projectile.GetComponent<Projectile>().Activate(ShootPoint.transform.position, targetRotation, ProjectileSpeed, Colour);
        projectile.GetComponent<Projectile>().BouncesOffTerrain = true;
    }
}

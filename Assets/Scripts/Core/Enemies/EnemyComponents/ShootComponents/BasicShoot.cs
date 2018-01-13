using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrixieCore;

public class BasicShoot : ShootComponent {

    public EnergyTypes.Colours Colour;

    public override Projectile[] Shoot(GameObject target)
    {
        this.target = target;
        return Shoot();
    }

    public override Projectile[] Shoot()
    {
        Projectile[] projectile = new Projectile[1];
        projectile[0] = ActivateProjecile();
        return projectile;
    }

    private Projectile ActivateProjecile()
    {
        GameObject projectile = ObjectPooler.Instance.GetPooledProjectile(ProjectilePrefab.name);
        if (projectile == null) return null;
        
        Vector2 distVector = transform.right;
        if (target != null)
        {
            distVector = target.transform.position - transform.position;
        }
        float targetRotation = Mathf.Atan2(distVector.y, distVector.x) * Mathf.Rad2Deg;

        Projectile projectileScript = projectile.GetComponent<Projectile>();
        projectileScript.Activate(ShootPoint.transform.position, targetRotation, ProjectileSpeed, Colour);
        return projectileScript;
    }

}

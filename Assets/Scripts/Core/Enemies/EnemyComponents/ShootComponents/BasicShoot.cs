using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicShoot : ShootComponent {

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
        GameObject projectile = ObjectPooler.Instance.GetPooledProjectile(ProjectilePrefab.name);
        if (projectile == null) return;
        
        Vector2 distVector = transform.right;
        if (target != null)
        {
            distVector = target.transform.position - transform.position;
        }
        float targetRotation = Mathf.Atan2(distVector.y, distVector.x) * Mathf.Rad2Deg;
        
        projectile.GetComponent<Projectile>().Activate(ShootPoint.transform.position, targetRotation, ProjectileSpeed, Colour);
    }

}

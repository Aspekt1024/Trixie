using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicShoot : ShootComponent {
    
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
        Vector2 distVector = transform.right;
        if (Target != null)
        {
            distVector = Target.transform.position - transform.position;
        }
        float targetRotation = Mathf.Atan2(distVector.y, distVector.x) * Mathf.Rad2Deg;

        Projectile projectilePrefabScript = ProjectilePrefab.GetComponent<Projectile>();

        GameObject projectile = ObjectPooler.Instance.GetPooledProjectile(projectilePrefabScript.name);
        if (projectile == null) return;

        projectile.SetActive(true);
        projectile.transform.position = ShootPoint.transform.position;
        projectile.transform.eulerAngles = new Vector3(0f, 0f, targetRotation);
        
        projectile.GetComponent<Rigidbody2D>().velocity = projectile.transform.right * ProjectileSpeed;

        if (projectilePrefabScript.Behaviour == Projectile.ProjectileBehaviours.Homing)
        {
            projectile.GetComponent<Projectile>().SetHomingTarget(Target.transform);
        }

    }

}

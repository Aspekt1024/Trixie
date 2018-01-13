using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallisticShotComponent : ShootComponent {
    
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
        Projectile projectilePrefabScript = ProjectilePrefab.GetComponent<Projectile>();
        GameObject projectile = ObjectPooler.Instance.GetPooledProjectile(projectilePrefabScript.name);
        if (projectile == null) return null;
        
        float targetRotation = CalculateThrowingAngle(transform.position, target.transform.position, false, ProjectileSpeed);
        
        Projectile projectileScript = projectile.GetComponent<Projectile>();
        projectileScript.Activate(ShootPoint.transform.position, targetRotation, ProjectileSpeed, ProjectileSettings);

        return projectileScript;
    }
    
    private float CalculateThrowingAngle(Vector3 startPos, Vector3 targetPos, bool upperPath, float s)
    {
        if (!ProjectileSettings.HasGravity)
        {
            Vector2 distVector = targetPos - startPos;
            return Mathf.Atan2(distVector.y, distVector.x) * Mathf.Rad2Deg;
        }

        // Source: https://en.wikipedia.org/wiki/Trajectory_of_a_projectile#Angle_required_to_hit_coordinate_.28x.2Cy.29
        float g = -Physics2D.gravity.y * ProjectileSettings.GravityScale;
        float x = startPos.x - targetPos.x;
        float y = targetPos.y - startPos.y;

        bool backwards = x < 0;
        if (backwards)
        {
            x = -x;
        }

        float angle;
        if (upperPath)
            angle = Mathf.Atan((s * s + Mathf.Sqrt(Mathf.Pow(s, 4) - g * (g * x * x + 2 * y * s * s))) / (g * x));
        else
            angle = Mathf.Atan((s * s - Mathf.Sqrt(Mathf.Pow(s, 4) - g * (g * x * x + 2 * y * s * s))) / (g * x));

        if (float.IsNaN(angle)) angle = 0;

        angle *= Mathf.Rad2Deg;

        return backwards ? angle : 180f - angle;
    }
}

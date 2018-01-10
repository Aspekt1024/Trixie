using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallisticShotComponent : ShootComponent {

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

        projectile.GetComponent<Rigidbody2D>().gravityScale = 0.5f;
        float targetRotation = CalculateThrowingAngle(transform.position, Target.transform.position, false, ProjectileSpeed, 0.5f);

        projectile.GetComponent<Projectile>().Activate(ShootPoint.transform.position, targetRotation, ProjectileSpeed, Colour);
    }


    private float CalculateThrowingAngle(Vector3 startPos, Vector3 targetPos, bool upperPath, float s, float gravityScale)
    {
        // Source: https://en.wikipedia.org/wiki/Trajectory_of_a_projectile#Angle_required_to_hit_coordinate_.28x.2Cy.29
        float g = -Physics2D.gravity.y * gravityScale;
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

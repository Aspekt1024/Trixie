using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class BarrageShot : ShootComponent {
    
    public EnergyTypes.Colours[] Projectiles;
    public float Spread = 5f;
    
    private Transform objTf;
    private Rigidbody2D body;
    
    public override void Shoot(GameObject target)
    {
        this.target = target;
        Shoot();
    }

    public override void Shoot()
    {
        float angle = CalculateThrowingAngle();
        float angleOffset = 0f;
        for (int i = 0; i < Projectiles.Length; i++)
        {
            angleOffset = -Mathf.Sign(angleOffset) * (Mathf.Abs(angleOffset) + (i % 2 == 0 ? 0 : Spread));
            ActivateProjectile(Projectiles[i], angle + angleOffset);
        }
    }

    private void ActivateProjectile(EnergyTypes.Colours colour, float angle)
    {
        GameObject projectile = ObjectPooler.Instance.GetPooledProjectile(ProjectilePrefab.name);
        if (projectile == null) return;

        ProjectileSettings.ProjectileColour = colour;
        
        Projectile projectileScript = projectile.GetComponent<Projectile>();
        projectile.GetComponent<Projectile>().Activate(ShootPoint.transform.position, angle, ProjectileSpeed, ProjectileSettings);
    }

    public float CalculateThrowingAngle()
    {
        if (!ProjectileSettings.HasGravity)
        {
            Vector2 distVector = target.transform.position - ShootPoint.transform.position;
            return Mathf.Atan2(distVector.y, distVector.x) * Mathf.Rad2Deg;
        }

        // Source: https://en.wikipedia.org/wiki/Trajectory_of_a_projectile#Angle_required_to_hit_coordinate_.28x.2Cy.29
        Vector3 startPos = transform.position;
        float s = ProjectileSpeed;
        float g = -Physics2D.gravity.y * ProjectileSettings.GravityScale;
        float x = startPos.x - target.transform.position.x;
        float y = target.transform.position.y - startPos.y;
        bool upperPath = false;

        bool backwards = x < 0;
        if (backwards)
        {
            x = -x;
        }

        float angle = Mathf.Atan((s * s + (upperPath ? 1 : -1) * Mathf.Sqrt(Mathf.Pow(s, 4) - g * (g * x * x + 2 * y * s * s))) / (g * x));
        if (float.IsNaN(angle)) angle = 0;
        angle *= Mathf.Rad2Deg;


        return backwards ? angle : 180f - angle;
    }
}

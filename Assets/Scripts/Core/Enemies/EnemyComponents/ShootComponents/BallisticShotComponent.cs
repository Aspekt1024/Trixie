using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallisticShotComponent : ShootComponent {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

    }

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
        
        float targetRotation = CalculateThrowingAngle(transform.position, Target.transform.position, false, ProjectileSpeed, projectilePrefabScript.GetComponent<Rigidbody2D>().gravityScale);

        GameObject projectile = ObjectPooler.Instance.GetPooledProjectile(projectilePrefabScript.name);
        if (projectile == null) return;

        projectile.SetActive(true);
        projectile.transform.position = ShootPoint.transform.position;
        projectile.transform.eulerAngles = new Vector3(0f, 0f, targetRotation);

        projectile.transform.localEulerAngles = new Vector3(0f, 0f, projectile.transform.localEulerAngles.z);
        projectile.GetComponent<Rigidbody2D>().velocity = projectile.transform.right * ProjectileSpeed;

        if (projectilePrefabScript.Behaviour == Projectile.ProjectileBehaviours.Homing)
        {
            projectile.GetComponent<Projectile>().SetHomingTarget(Target.transform);
        }

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

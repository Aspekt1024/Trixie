using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SineShootComponent : ShootComponent {

    public float Amplitude = 4f;
    public float Wavelength = 4f;
    public float Phase = 0f;

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
        projectile.GetComponent<Projectile>().NewSetColour(Colour);

        if (projectilePrefabScript.Behaviour == Projectile.ProjectileBehaviours.Homing)
        {
            projectile.GetComponent<Projectile>().SetHomingTarget(Target.transform);
        }

        StartCoroutine(SinePath(projectile));

    }

    private IEnumerator SinePath(GameObject projectile)
    {
        Rigidbody2D body = projectile.GetComponent<Rigidbody2D>();

        float t = 0f;
        float initialY = body.transform.position.y;
        float y = 0f;

        while (projectile.activeSelf)
        {
            t += Time.deltaTime;
            float arg = Mathf.PI * 2 / (Wavelength / ProjectileSpeed);
            y = Mathf.Sin(arg * t + Phase) * Amplitude;
            float grad = Mathf.Cos(arg * t + Phase) * arg * Amplitude / ProjectileSpeed;
            float rotation = Mathf.Atan2(grad, 1) * Mathf.Rad2Deg;
            body.transform.eulerAngles = new Vector3(0f, 0f, 180  - rotation);
            body.transform.position = new Vector3(body.transform.position.x, initialY + y, body.transform.position.z);
            yield return null;
        }

    }
}

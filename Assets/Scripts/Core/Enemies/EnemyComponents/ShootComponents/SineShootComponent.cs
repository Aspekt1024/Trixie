using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrixieCore;

public class SineShootComponent : ShootComponent {

    public EnergyTypes.Colours Colour;
    public float Amplitude = 4f;
    public float Wavelength = 4f;
    public float Phase = 0f;

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
        
        Vector2 distVector = transform.right;
        if (target != null)
        {
            distVector = target.transform.position - transform.position;
        }
        float targetRotation = Mathf.Atan2(distVector.y, distVector.x) * Mathf.Rad2Deg;

        projectile.SetActive(true);
        Projectile projectileScript = projectile.GetComponent<Projectile>();
        projectileScript.Activate(ShootPoint.transform.position, targetRotation, ProjectileSpeed, Colour);
        projectileScript.SetSinePath(Amplitude, Wavelength, Phase, ProjectileSpeed);
        return projectileScript;
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

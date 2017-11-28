using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootComponent : MonoBehaviour {

    public GameObject ProjectilePrefab;
    public GameObject Turrets;
    public float ProjectileCooldown;
    public float ProjectileSpeed;
    public float VisibleRange;
    public Transform ShootPoint;
    public LayerMask[] TargetLayers;

    private Transform aimTarget;
    private float timeLastShot;

    private Projectile projectilePrefabScript;
    private int layerMask;

    private enum States
    {
        None, FindingTarget, Aiming, Shooting
    }
    private States state;

    private void Start()
    {
        state = States.Aiming;
        timeLastShot = Time.deltaTime;
        projectilePrefabScript = ProjectilePrefab.GetComponent<Projectile>();

        SetupLayerMask();
    }

    private void Update()
    {
        switch (state)
        {
            case States.None:
                break;
            case States.FindingTarget:
                if (TargetInLineOfSight())
                {
                    state = States.Aiming;
                }
                break;
            case States.Aiming:
                if (Aim())
                {
                    if (Shoot())
                    {
                        state = States.Shooting;
                    }
                }
                if (!TargetInLineOfSight())
                {
                    state = States.FindingTarget;
                }
                break;
            case States.Shooting:
                state = States.FindingTarget;
                break;
            default:
                break;
        }
    }

    public void SetTarget(Transform newTarget)
    {
        aimTarget = newTarget;
    }

    public void Deactivate()
    {
        state = States.None;
        Turrets.SetActive(false);
    }

    public void Activate()
    {
        state = States.FindingTarget;
        Turrets.SetActive(true);
    }

    private bool TargetInLineOfSight()
    {
        if (aimTarget == null) return false;

        Vector2 distVector = aimTarget.position - transform.position;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, distVector, VisibleRange, layerMask);
        
        if (hit.collider != null && hit.collider.gameObject.layer != LayerMask.NameToLayer("Terrain"))
        {
            return true;
        }
        return false;
    }

    private bool Aim()
    {
        Vector2 distVector = aimTarget.position - transform.position;
        float targetRotation = Mathf.Atan2(distVector.y, distVector.x) * Mathf.Rad2Deg;
        Turrets.transform.localEulerAngles = new Vector3(0f, 0f, targetRotation);
        return true;
    }

    private bool Shoot()
    {
        if (timeLastShot + ProjectileCooldown > Time.time) return false;
        
        GameObject projectile = ObjectPooler.Instance.GetPooledProjectile(projectilePrefabScript.name);
        if (projectile == null) return false;

        projectile.SetActive(true);
        projectile.transform.position = ShootPoint.transform.position;
        projectile.transform.localRotation = Turrets.transform.rotation;
        projectile.transform.localEulerAngles = new Vector3(0f, 0f, projectile.transform.localEulerAngles.z);
        projectile.GetComponent<Rigidbody2D>().velocity = Turrets.transform.right * ProjectileSpeed;

        if (projectilePrefabScript.Behaviour == Projectile.ProjectileBehaviours.Homing)
        {
            projectile.GetComponent<Projectile>().SetTarget(aimTarget);
        }

        timeLastShot = Time.time;
        return true;
    }

    private void SetupLayerMask()
    {
        for (int i = 0; i < TargetLayers.Length; i++)
        {
            layerMask |= TargetLayers[i];
        }
        layerMask |= 1 << LayerMask.NameToLayer("Terrain");
    }
}

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootComponentOld : MonoBehaviour {

    public enum ShootTargets
    {
        Player, None, CustomPoint
    }
    public ShootTargets ShootTarget;

    public enum ShootTypes
    {
        OneShot, Radial
    }
    public ShootTypes ShootType;

    public bool RadialClockwise;
    public float RadialStartDegree;
    public float RadialArc;
    public float RadialDistanceBetweenShots;
    public float DelayBetweenRadialShots;

    public GameObject ProjectilePrefab;
    public GameObject Turrets;
    public float ProjectileCooldown;
    public float ProjectileSpeed;
    public float VisibleRange;
    public Transform ShootPoint;
    public LayerMask[] TargetLayers;

    public Transform CustomTarget;

    private Transform aimTarget;
    private float timeLastShot;

    private Projectile projectilePrefabScript;
    private int layerMask;

    private Coroutine shootCoroutine;

    private enum States
    {
        None, FindingTarget, Aiming, Shooting
    }
    private States state;

    private void Start()
    {
        if (ShootTarget == ShootTargets.CustomPoint)
        {
            aimTarget = CustomTarget;
        }
        else if (ShootTarget == ShootTargets.Player)
        {
            aimTarget = Player.Instance.transform;
        }

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

                if (ShootType == ShootTypes.Radial)
                {
                    state = States.Shooting;
                }
                else if (TargetInLineOfSight())
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
                Shoot();
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Set externally by e.g. an enemy script on startup
    /// </summary>
    /// <param name="newTarget"></param>
    public void SetTarget(Transform newTarget)
    {
        if (ShootTarget != ShootTargets.CustomPoint)
        {
            aimTarget = newTarget;
        }
    }

    public void Deactivate()
    {
        if (shootCoroutine != null)
        {
            StopCoroutine(shootCoroutine);
        }
        state = States.None;
        if (Turrets != null)
        {
            Turrets.SetActive(false);
        }
    }

    public void Activate()
    {
        state = States.FindingTarget;
        if (Turrets != null)
        {
            Turrets.SetActive(true);
        }
    }

    private bool TargetInLineOfSight()
    {
        if (ShootTarget == ShootTargets.None || ShootTarget == ShootTargets.CustomPoint) return true;

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
        SetLookAngle(targetRotation);
        return true;
    }

    private void SetLookAngle(float angle)
    {
        if (Turrets == null) return;
        Turrets.transform.localEulerAngles = new Vector3(0f, 0f, angle);
    }

    private bool Shoot()
    {
        if (timeLastShot + ProjectileCooldown > Time.time) return false;

        timeLastShot = Time.time;

        if (ShootType == ShootTypes.OneShot)
        {
            ShootOneShot();
        }
        else
        {
            shootCoroutine = StartCoroutine(ShootRadial());
        }
        return true;
    }

    private void ShootOneShot()
    {
        ActivateProjecile();
        state = States.FindingTarget;
    }

    private IEnumerator ShootRadial()
    {
        float angle = RadialStartDegree;
        float arcCovered = 0f;

        float timeBetweenShots = 0f;

        while(arcCovered <= RadialArc)
        {
            SetLookAngle(angle);
            ActivateProjecile();

            timeBetweenShots = 0f;
            while (timeBetweenShots < DelayBetweenRadialShots)
            {
                timeBetweenShots += Time.deltaTime;
                yield return null;
            }

            if (RadialDistanceBetweenShots == 0) break;
            
            angle += (RadialClockwise ? -1 : 1) * RadialDistanceBetweenShots;
            arcCovered += RadialDistanceBetweenShots;
        }
    }

    private void ActivateProjecile()
    {
        GameObject projectile = ObjectPooler.Instance.GetPooledProjectile(projectilePrefabScript.name);
        if (projectile == null) return;

        projectile.SetActive(true);
        projectile.transform.position = ShootPoint.transform.position;

        if (Turrets != null)
        {
            projectile.transform.localRotation = Turrets.transform.rotation;
        }
        projectile.transform.localEulerAngles = new Vector3(0f, 0f, projectile.transform.localEulerAngles.z);

        projectile.GetComponent<Rigidbody2D>().velocity = projectile.transform.right * ProjectileSpeed;
        
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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrixieCore;

public class BallisticsComponent : MonoBehaviour
{

    public enum ShootTargets
    {
        Player, None, CustomPoint
    }
    public ShootTargets ShootTarget;

    public enum ShootTypes
    {
        SingleShot, Barrage
    }
    public ShootTypes ShootType;

    public enum AimTypes
    {
        TurretRotation, LeftRightOnly,
    }
    public AimTypes AimType;

    // TODO barrage target points
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

        state = States.None;
        timeLastShot = Time.deltaTime;
        projectilePrefabScript = ProjectilePrefab.GetComponent<Projectile>();

        SetupLayerMask();
    }

    private void Update()
    {
        Debug.Log(state);
        switch (state)
        {
            case States.None:
                break;
            case States.FindingTarget:
                if (ShootType == ShootTypes.Barrage)
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
                    if (CanShoot())
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

    private bool CanShoot()
    {
        return timeLastShot + ProjectileCooldown <= Time.time;
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
        state = States.None;

        if (shootCoroutine != null)
        {
            Debug.Log("stopping");
            StopCoroutine(shootCoroutine);
        }

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

        Debug.Log(aimTarget);
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
        float targetRotation = CalculateThrowingAngle(transform.position, aimTarget.position, false, ProjectileSpeed, projectilePrefabScript.GetComponent<Rigidbody2D>().gravityScale);
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
        Debug.Log("shooting");
        if (!CanShoot()) return false;

        timeLastShot = Time.time;

        if (ShootType == ShootTypes.SingleShot)
        {
            ShootSingle();
        }
        else
        {
            shootCoroutine = StartCoroutine(ShootRadial());
        }
        return true;
    }

    private void ShootSingle()
    {
        ActivateProjecile();
        state = States.FindingTarget;
    }

    private IEnumerator ShootRadial()
    {
        float timeBetweenShots = 0f;
        
        //SetLookAngle(angle);
        ActivateProjecile();

        timeBetweenShots = 0f;
        while (timeBetweenShots < DelayBetweenRadialShots)
        {
            timeBetweenShots += Time.deltaTime;
            yield return null;
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

    private void SetupLayerMask()
    {
        for (int i = 0; i < TargetLayers.Length; i++)
        {
            layerMask |= TargetLayers[i];
        }
        layerMask |= 1 << LayerMask.NameToLayer("Terrain");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicTurret : MonoBehaviour {

    public GameObject ProjectilePrefab;
    public float ProjectileCooldown;
    public float ProjectileSpeed;

    private float timeLastShot;

    private void Start()
    {
        timeLastShot = Time.deltaTime;
        if (ProjectilePrefab == null)
        {
            Debug.LogError("No projectile prefab set for Basic Turret");
        }
    }

    private void Update()
    {
        if (timeLastShot + ProjectileCooldown < Time.time)
        {
            Shoot();
        }
    }
    
    private void Shoot()
    {
        GameObject projectile = ObjectPooler.Instance.GetPooledObject("Projectile");
        if (projectile == null) return;

        projectile.SetActive(true);
        projectile.transform.position = transform.position;
        projectile.transform.localRotation = transform.rotation;
        projectile.GetComponent<Rigidbody2D>().velocity = transform.up * ProjectileSpeed;

        timeLastShot = Time.time;
    }
}

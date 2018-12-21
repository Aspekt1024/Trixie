using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public BulletTypes BulletType = BulletTypes.Normal;

    public enum BulletTypes
    {
        Normal, Frost, Fire
    }

    private float timeSpawned;
    private float maxLifetime = 2f;

    private Vector2 velocity;

    public void Fire(Vector2 origin, Vector2 direction, float speed)
    {
        transform.position = origin;
        velocity = direction.normalized * speed;
        timeSpawned = Time.time;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player") return;

    }

    private void FixedUpdate()
    {
        LayerMask mask = 1 << LayerMask.NameToLayer("Enemy") | 1<< LayerMask.NameToLayer("Terrain");
        RaycastHit2D hit = Physics2D.Raycast(transform.position, velocity, velocity.magnitude * Time.fixedDeltaTime, mask);
        if (hit.collider != null)
        {
            transform.position = hit.point;
            var damageableObject = hit.collider.GetComponent<IShootable>();
            damageableObject?.HandleShot(this);
            Explode();
        }
    }

    private void Update()
    {
        transform.Translate(velocity * Time.deltaTime);

        if (Time.time > timeSpawned + maxLifetime)
        {
            Explode();
        }
    }

    private void Explode()
    {
        Destroy(gameObject);
    }
}

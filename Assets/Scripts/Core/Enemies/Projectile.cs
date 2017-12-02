using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    public enum ProjectileBehaviours
    {
        Normal, Homing
    }

    public bool BounceOffShield;
    //public bool BounceOffWalls;
    
    public EnergyTypes.Colours ProjectileColour;
    public ProjectileBehaviours Behaviour;

    private Animator anim;
    private Rigidbody2D body;
    private Transform homingTarget;
    
    private void Awake()
    {
        anim = GetComponent<Animator>();
        body = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (Behaviour == ProjectileBehaviours.Homing)
        {
            FollowTarget();
        }

        Vector2 viewportPos = Camera.main.WorldToViewportPoint(transform.position);
        if (viewportPos.x < -0.5f || viewportPos.x > 1.5f || viewportPos.y < -0.5f || viewportPos.y > 1.5f)
        {
            gameObject.SetActive(false);
        }
    }

    public void SetTarget(Transform newTarget)
    {
        homingTarget = newTarget;
    }

    public void HitByShield(Vector2 shieldDirection)
    {
        if (BounceOffShield)
        {
            body.velocity = shieldDirection * body.velocity.magnitude;
            transform.eulerAngles = new Vector3(0f, 0f, Mathf.Atan2(shieldDirection.y, shieldDirection.x) * Mathf.Rad2Deg);
        }
        else
        {
            DestroyByCollision();
        }
    }
    
    public void DestroyByCollision()
    {
        gameObject.SetActive(false);
        // TODO hit object animation
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Shield")
        {
            // Do nothing. This is handled by the shield component
        }
        else
        {
            gameObject.SetActive(false);
            // TODO hit object animation
        }
    }

    private void OnEnable()
    {
        // TODO set initial state (animations etc)
        homingTarget = null;
    }

    private void FollowTarget()
    {
        if (homingTarget == null) return;

        Vector2 distVector = homingTarget.position - transform.position;
        float targetRotation = Mathf.Atan2(distVector.y, distVector.x) * Mathf.Rad2Deg;
        transform.localEulerAngles = new Vector3(0f, 0f, targetRotation);

        float speed = body.velocity.magnitude;
        body.velocity = transform.right * speed;
    }
}

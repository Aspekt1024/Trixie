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

    private bool inGravityField;
    private List<GravityField> gravityFields;
    private float currentFieldStrength;
    private float currentModifiedVelocity;
    
    private void Awake()
    {
        anim = GetComponent<Animator>();
        body = GetComponent<Rigidbody2D>();
        gravityFields = new List<GravityField>();
    }

    private void FixedUpdate()
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

        UpdateModifiedVelocity();
    }

    public void SetHomingTarget(Transform newTarget)
    {
        homingTarget = newTarget;
    }

    public void HitByShield(Vector2 shieldDirection)
    {
        if (BounceOffShield)
        {
            body.velocity = shieldDirection * body.velocity.magnitude;
            transform.eulerAngles = new Vector3(0f, 0f, Mathf.Atan2(body.velocity.y, body.velocity.x) * Mathf.Rad2Deg);
        }
        else
        {
            DestroyByCollision();
        }
    }
    
    public void DestroyByCollision()
    {
        //gameObject.SetActive(false);
        // TODO hit object animation
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "GravityField")
        {
            gravityFields.Add(collision.GetComponent<GravityField>());
            if (!inGravityField)
            {
                inGravityField = true;
                SetModifiedVelocity();
            }
        }
        else if (collision.tag == "Shield")
        {
            // Do nothing. This is handled by the shield / gravity field components
        }
        else
        {
            GetComponent<TrailRenderer>().Clear();
            gameObject.SetActive(false);
            // TODO hit object animation
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "GravityField")
        {
            gravityFields.Remove(collision.GetComponent<GravityField>());
            if (gravityFields.Count > 0)
            {
                SetModifiedVelocity();
            }
            else
            {
                inGravityField = false;
                RemoveModifiedVelocty();
            }
        }
    }

    private void UpdateModifiedVelocity()
    {
        body.velocity -= Vector2.up * currentModifiedVelocity / 3f;
        currentModifiedVelocity += currentFieldStrength * Time.deltaTime;
        body.velocity += Vector2.up * currentModifiedVelocity /3f;
        transform.eulerAngles = new Vector3(0f, 0f, Mathf.Atan2(body.velocity.y, body.velocity.x) * Mathf.Rad2Deg);
    }

    private void SetModifiedVelocity()
    {
        currentFieldStrength = gravityFields[0].Strength;
    }

    private void RemoveModifiedVelocty()
    {
        body.velocity -= Vector2.up * currentModifiedVelocity / 3f;
        currentFieldStrength = 0f;
        currentModifiedVelocity = 0f;
    }

    private void OnEnable()
    {
        homingTarget = null;
        inGravityField = false;
        currentFieldStrength = 0f;
        currentModifiedVelocity = 0f;
        body.velocity = Vector2.zero;
        gravityFields = new List<GravityField>();
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

    public void NewSetColour(EnergyTypes.Colours energyColour)
    {
        ProjectileColour = energyColour;
        Color color = new Color();
        switch (energyColour)
        {
            case EnergyTypes.Colours.Blue:
                color = Color.blue;
                break;
            case EnergyTypes.Colours.Pink:
                color = Color.red;
                break;
            case EnergyTypes.Colours.Yellow:
                color = Color.green;
                break;
            default:
                break;
        }
        GetComponent<SpriteRenderer>().color = color;
        GetComponent<TrailRenderer>().startColor = color;
        GetComponent<TrailRenderer>().endColor = new Color(color.r, color.g, color.b, 0f);
    }
}

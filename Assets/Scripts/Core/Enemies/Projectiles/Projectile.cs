using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    public enum ProjectileBehaviours
    {
        Normal, Homing
    }

    public bool BounceOffShield;
    public bool BouncesOffTerrain;
    
    public EnergyTypes.Colours ProjectileColour;
    public ProjectileBehaviours Behaviour;
    
    private Animator anim;
    private Rigidbody2D body;
    private Transform homingTarget;

    private bool inGravityField;
    private List<GravityField> gravityFields;
    private float currentFieldStrength;
    private float currentModifiedVelocity;

    private Coroutine pathRoutine;
    
    protected virtual void Awake()
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
            Deactivate();
        }

        //UpdateModifiedVelocity();
    }

    protected virtual void OnEnable()
    {
        homingTarget = null;
        inGravityField = false;
        currentFieldStrength = 0f;
        currentModifiedVelocity = 0f;
        body.velocity = Vector2.zero;
        GetComponent<Collider2D>().enabled = true;
        GetComponent<SpriteRenderer>().enabled = true;
        gravityFields = new List<GravityField>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "GravityField")
        {
            gravityFields.Add(collision.gameObject.GetComponent<GravityField>());
            if (!inGravityField)
            {
                inGravityField = true;
                SetModifiedVelocity();
            }
        }
        else if (BounceOffShield && collision.gameObject.layer == LayerMask.NameToLayer("Shield"))
        {
            body.velocity = collision.transform.right * body.velocity.magnitude;
            transform.eulerAngles = new Vector3(0f, 0f, Mathf.Atan2(body.velocity.y, body.velocity.x) * Mathf.Rad2Deg);
        }
        else if (BouncesOffTerrain && collision.gameObject.layer == LayerMask.NameToLayer("Terrain"))
        {
        }
        else
        {
            ShowImpact();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Shield"))
        {
            ShowImpact();
        }
    }

    protected virtual void ShowImpact()
    {
        body.velocity = Vector2.zero;
        body.gravityScale = 0f;
        GetComponent<Collider2D>().enabled = false;
        GetComponent<SpriteRenderer>().enabled = false;
        StartCoroutine(DeactivateAfterSeconds(1f));
    }

    protected virtual IEnumerator DeactivateAfterSeconds(float sec)
    {
        yield return new WaitForSeconds(sec);
        gameObject.SetActive(false);
    }

    protected virtual void PersistingExplosion()
    {
        // Not currently in use but would be a cool mechanic
        body.gravityScale = 10f;
        body.velocity = Vector2.zero;
        GetComponent<SpriteRenderer>().enabled = false;
    }
    
    protected virtual void Deactivate()
    {
        if (pathRoutine != null)
        {
            StopCoroutine(pathRoutine);
        }
        gameObject.SetActive(false);
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

    private void FollowTarget()
    {
        if (homingTarget == null) return;

        Vector2 distVector = homingTarget.position - transform.position;
        float targetRotation = Mathf.Atan2(distVector.y, distVector.x) * Mathf.Rad2Deg;
        transform.localEulerAngles = new Vector3(0f, 0f, targetRotation);

        float speed = body.velocity.magnitude;
        body.velocity = transform.right * speed;
    }

    public void Activate(Vector3 startPoint, float angle, float speed, EnergyTypes.Colours colour, Transform homingTarget = null)
    {
        gameObject.SetActive(true);
        transform.position = startPoint;
        transform.eulerAngles = new Vector3(0f, 0f, angle);

        body.velocity = transform.right * speed;
        SetColour(colour);

        if (homingTarget != null)
        {
            Behaviour = ProjectileBehaviours.Homing;
            this.homingTarget = homingTarget;
        }
    }

    public void SetSinePath(float amplitude, float wavelength, float phase, float speed)
    {
        pathRoutine = StartCoroutine(SinePath(amplitude, wavelength, phase, speed));
    }
    
    public void SetColour(EnergyTypes.Colours energyColour)
    {
        ProjectileColour = energyColour;
        Color color = new Color();
        switch (energyColour)
        {
            case EnergyTypes.Colours.Blue:
                color = new Color(0.2f, 0.4f, 1f, 1f);
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


    private IEnumerator SinePath(float amplitude, float wavelength, float phase, float speed)
    {
        float t = 0f;
        float y = 0f;
        float initialY = body.transform.position.y;
        float arg = 2f * Mathf.PI / (wavelength / speed);

        while (true)
        {
            t += Time.deltaTime;
            y = Mathf.Sin(arg * t + phase) * amplitude;

            float grad = Mathf.Cos(arg * t + phase) * arg * amplitude / speed;
            float rotation = Mathf.Atan2(grad, 1f) * Mathf.Rad2Deg;
            if (speed < 0f) rotation = 180f - rotation;

            body.transform.eulerAngles = new Vector3(0f, 0f, rotation);
            body.transform.position = new Vector3(body.transform.position.x, initialY + y, body.transform.position.z);
            yield return null;
        }
    }
}

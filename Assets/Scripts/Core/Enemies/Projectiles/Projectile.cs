using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrixieCore
{
    public class Projectile : MonoBehaviour
    {

        public delegate void DestroyEvent(Projectile projectile, bool destroyedBySameShieldColour);
        public DestroyEvent OnDestroyed;
        public void Destroyed(Projectile projectile, bool destroyedBySameShieldColour)
        {
            if (OnDestroyed != null)
            {
                OnDestroyed(projectile, destroyedBySameShieldColour);
            }
        }

        [System.Serializable]
        public struct ProjectileSettings
        {
            public EnergyTypes.Colours ProjectileColour;
            public bool IsHoming;
            public Transform HomingTarget;
            public bool IsAMine;
            public bool IsSticky;
            public float StickyTime;
            public bool BouncesOffShield;
            public bool BouncesOffTerrain;
            public int NumTimesToBounce;
            public bool HasGravity;
            public float GravityScale;
        }
        private ProjectileSettings settings;

        private Animator anim;
        private Rigidbody2D body;

        private int numTimesBounced;
        private float stickyTimer;
        private bool isStuck;
        private float originalSpeed;

        private bool inGravityField;
        private List<GravityField> gravityFields;
        private float currentFieldStrength;
        private float currentModifiedVelocity;

        private Coroutine pathRoutine;

        private bool destroyedBySameShieldColour;

        protected virtual void Awake()
        {
            anim = GetComponent<Animator>();
            body = GetComponent<Rigidbody2D>();
            gravityFields = new List<GravityField>();
        }

        private void FixedUpdate()
        {
            if (settings.IsHoming)
            {
                FollowTarget();
            }

            if (isStuck && (settings.IsAMine || settings.IsSticky))
            {
                stickyTimer += Time.fixedDeltaTime;
                if (stickyTimer > settings.StickyTime)
                {
                    ShowImpact();
                }
            }

            Vector2 viewportPos = Camera.main.WorldToViewportPoint(transform.position);
            if (viewportPos.x < -0.5f || viewportPos.x > 1.5f || viewportPos.y < -0.5f || viewportPos.y > 1.5f)
            {
                Deactivate();
            }

            //UpdateModifiedVelocity();
        }

        public ProjectileSettings GetSettings()
        {
            return settings;
        }

        protected virtual void OnEnable()
        {
            isStuck = false;
            numTimesBounced = 0;
            inGravityField = false;
            currentFieldStrength = 0f;
            currentModifiedVelocity = 0f;
            body.velocity = Vector2.zero;
            GetComponent<Collider2D>().enabled = true;
            GetComponent<SpriteRenderer>().enabled = true;
            gravityFields = new List<GravityField>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Shield") || collision.gameObject.layer == LayerMask.NameToLayer("Player") || collision.gameObject.layer == LayerMask.NameToLayer("PlayerProjectile"))
            {
                ShowImpact();
            }
            else if (collision.gameObject.layer == TrixieLayers.GetMask(Layers.Enemy))
            {
                collision.GetComponent<BaseEnemy>().DamageEnemy(collision.transform.position - transform.position);
                ShowImpact();
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

        private void OnCollisionEnter2D(Collision2D collision)
        {
            destroyedBySameShieldColour = false;
            if (collision.gameObject.layer == TrixieLayers.GetMask(Layers.Enemy))
            {
                collision.gameObject.GetComponent<BaseEnemy>().DamageEnemy(collision.transform.position - transform.position);
            }
            else if (collision.collider.tag == "GravityField")
            {
                gravityFields.Add(collision.gameObject.GetComponent<GravityField>());
                if (!inGravityField)
                {
                    inGravityField = true;
                    SetModifiedVelocity();
                }
            }
            else if (settings.BouncesOffShield && collision.gameObject.layer == LayerMask.NameToLayer("Shield"))
            {
                body.velocity = collision.transform.right * body.velocity.magnitude;
                transform.eulerAngles = new Vector3(0f, 0f, Mathf.Atan2(body.velocity.y, body.velocity.x) * Mathf.Rad2Deg);
            }
            else if (collision.gameObject.layer == LayerMask.NameToLayer("Terrain"))
            {
                if (settings.BouncesOffTerrain)
                {
                    if (numTimesBounced == settings.NumTimesToBounce)
                    {
                        ShowImpact();
                    }
                    numTimesBounced++;
                }
                else if (settings.IsSticky || settings.IsAMine)
                {
                    if (!isStuck)
                    {
                        SetAsMine();
                    }
                }
                else
                {
                    ShowImpact();
                }
            }
            else if (collision.gameObject.layer == TrixieLayers.GetMask(Layers.Shield))
            {
                ShieldComponent shield = Player.Instance.GetComponent<ShieldComponent>();
                if (shield.GetColour() == GetColour())
                {
                    destroyedBySameShieldColour = true;
                }
                shield.ProjectileImpact(this);
            }
            else
            {
                ShowImpact();
            }
        }

        public void Destroy()
        {
            // TODO this is used only for shield effects. On collision / trigger needs to be reworked for consistency
            ShowImpact();
        }

        public void Disable()
        {
            Deactivate();
        }

        protected virtual void ShowImpact()
        {
            body.velocity = Vector2.zero;
            body.gravityScale = 0f;
            GetComponent<Collider2D>().enabled = false;
            GetComponent<SpriteRenderer>().enabled = false;
            Destroyed(this, destroyedBySameShieldColour);
            StartCoroutine(DeactivateAfterSeconds(1f));
        }

        protected virtual IEnumerator DeactivateAfterSeconds(float sec)
        {
            yield return new WaitForSeconds(sec);
            gameObject.SetActive(false);
        }

        private void SetAsMine()
        {
            isStuck = true;
            stickyTimer = 0f;
            body.gravityScale = 0f;
            body.velocity = Vector2.zero;
        }

        protected virtual void PersistingExplosion()
        {
            body.gravityScale = 0f;
            body.velocity = Vector2.zero;
            //GetComponent<SpriteRenderer>().enabled = false;
        }

        protected virtual void Deactivate()
        {
            if (pathRoutine != null)
            {
                StopCoroutine(pathRoutine);
            }
            gameObject.SetActive(false);
        }

        private void UpdateModifiedVelocity()
        {
            body.velocity -= Vector2.up * currentModifiedVelocity / 3f;
            currentModifiedVelocity += currentFieldStrength * Time.deltaTime;
            body.velocity += Vector2.up * currentModifiedVelocity / 3f;
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
            if (!settings.IsHoming || settings.HomingTarget == null) return;

            Vector2 distVector = settings.HomingTarget.position - transform.position;
            float targetRotation = Mathf.Atan2(distVector.y, distVector.x) * Mathf.Rad2Deg;
            transform.localEulerAngles = new Vector3(0f, 0f, targetRotation);

            float speed = body.velocity.magnitude;
            body.velocity = transform.right * speed;
        }

        public void Activate(Vector3 startPoint, float angle, float speed, EnergyTypes.Colours colour, Transform homingTarget = null)
        {
            // TODO make obsolete. Replace with below
            settings.ProjectileColour = colour;
            if (homingTarget != null)
            {
                settings.IsHoming = true;
                settings.HomingTarget = homingTarget;
            }
            Activate(startPoint, angle, speed, settings);
        }

        public void Activate(Vector3 startPoint, float angle, float speed, ProjectileSettings newSettings)
        {
            SetProjectileSettings(newSettings);
            gameObject.SetActive(true);
            gameObject.layer = TrixieLayers.GetMask(Layers.Projectile);
            transform.position = startPoint;
            transform.eulerAngles = new Vector3(0f, 0f, angle);
            originalSpeed = speed;
            body.velocity = transform.right * speed;
        }

        public void SetSinePath(float amplitude, float wavelength, float phase, float speed)
        {
            pathRoutine = StartCoroutine(SinePath(amplitude, wavelength, phase, speed));
        }

        public void SetColour(EnergyTypes.Colours energyColour)
        {
            settings.ProjectileColour = energyColour;
            Color color = new Color();
            switch (energyColour)
            {
                case EnergyTypes.Colours.Blue:
                    color = new Color(0.2f, 0.4f, 1f, 1f);
                    break;
                case EnergyTypes.Colours.Red:
                    color = Color.red;
                    break;
                case EnergyTypes.Colours.Green:
                    color = Color.green;
                    break;
                default:
                    break;
            }
            SetColourGraphic(color);
        }

        public void SetSpeedMultiplier(float multiplier)
        {
            body.gravityScale = settings.GravityScale * multiplier;
            body.velocity = body.velocity.normalized * originalSpeed * multiplier;
        }

        protected virtual void SetColourGraphic(Color color)
        {
        }

        private void SetProjectileSettings(ProjectileSettings newSettings)
        {
            settings = newSettings;

            SetColour(settings.ProjectileColour);
            if (settings.HasGravity) { body.gravityScale = settings.GravityScale; }
            if (!settings.IsHoming) { settings.HomingTarget = null; }
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

        public EnergyTypes.Colours GetColour()
        {
            return settings.ProjectileColour;
        }
    }
}

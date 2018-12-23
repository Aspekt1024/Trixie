using System.Collections;
using UnityEngine;
using TrixieCore.Units;
using Aspekt.IO;
using Aspekt.PlayerController;

namespace TrixieCore
{
    public class RangedAbility : PlayerAbility
    {
        public float PowerupTime = 0.6f;
        public float RangedCooldown = 0.1f;

        public GameObject ProjectilePrefab;

        private ShieldAbility shield;
        private ChargeIndicator indicator;

        private bool isChargingUp;
        private float chargingTime;

        private void Start()
        {
            shield = GetComponent<ShieldAbility>();
            indicator = GetComponentInChildren<ChargeIndicator>();
            indicator.SetChargeState(ChargeIndicator.States.None);
        }

        private void Update()
        {
            if (isChargingUp)
            {
                chargingTime += Time.deltaTime;
                if (chargingTime > PowerupTime)
                {
                    indicator.SetChargeState(ChargeIndicator.States.StageOne);
                }
            }
        }

        public void RangedPressed()
        {
            isChargingUp = true;
            chargingTime = 0f;
            indicator.SetChargeState(ChargeIndicator.States.Charging);
            switch (shield.GetColour())
            {
                case EnergyTypes.Colours.Blue:
                    AttackBlue();
                    break;
                case EnergyTypes.Colours.Red:
                    AttackRed();
                    break;
                case EnergyTypes.Colours.Green:
                    AttackGreen();
                    break;
                default:
                    break;
            }
        }

        public void RangedReleased()
        {
            if (chargingTime > PowerupTime)
            {
                switch (shield.GetColour())
                {
                    case EnergyTypes.Colours.Blue:
                        PowerAttackBlue();
                        break;
                    case EnergyTypes.Colours.Red:
                        PowerAttackRed();
                        break;
                    case EnergyTypes.Colours.Green:
                        PowerAttackGreen();
                        break;
                    default:
                        break;
                }
            }
            isChargingUp = false;
            indicator.SetChargeState(ChargeIndicator.States.None);
        }

        private void AttackBlue()
        {

        }

        private void AttackRed()
        {
            float projectileSpeed = 26f;
            GameObject proj = Instantiate(ProjectilePrefab);
            proj.GetComponent<Rigidbody2D>().velocity = GetMoveDirection().normalized * projectileSpeed;
            proj.transform.position = (Vector2)transform.position + proj.GetComponent<Rigidbody2D>().velocity.normalized * 1f;
            proj.GetComponent<Projectile>().SetColour(EnergyTypes.Colours.Red);
        }

        private void AttackGreen()
        {
            float projectileSpeed = 36f;
            GameObject proj = Instantiate(ProjectilePrefab);
            proj.GetComponent<Rigidbody2D>().velocity = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * projectileSpeed;
            proj.transform.position = (Vector2)transform.position + proj.GetComponent<Rigidbody2D>().velocity.normalized;
            proj.GetComponent<Collider2D>().isTrigger = true;
            proj.GetComponent<Projectile>().SetColour(EnergyTypes.Colours.Green);

            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 15f, 1 << TrixieCore.TrixieLayers.GetMask(TrixieCore.Layers.Enemy));
            Collider2D enemy = null;
            foreach (Collider2D c in colliders)
            {
                if (c.tag == "Enemy")
                {
                    enemy = c;
                    break;
                }
            }
            if (enemy)
            {
                StartCoroutine(DirectGreenAtTarget(proj.GetComponent<Rigidbody2D>(), enemy.transform));
            }
            else
            {
                Vector2 target = (Vector2)transform.position + new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * 5f;
                StartCoroutine(DirectGreenTargetMissing(proj.GetComponent<Rigidbody2D>(), target));
            }
        }

        private IEnumerator DirectGreenTargetMissing(Rigidbody2D projectile, Vector2 target)
        {
            Vector3 originalScale = projectile.transform.localScale;
            Vector2 originalVelocity = projectile.velocity;
            float timer = 0f;
            float duration = .3f;
            while (timer < duration)
            {
                projectile.transform.localScale = originalScale * Mathf.Lerp(1f, 0.4f, timer / duration);
                projectile.velocity = originalVelocity.magnitude * Vector2.Lerp(originalVelocity, target - projectile.position, timer / duration).normalized;
                timer += Time.deltaTime;
                yield return null;
            }
            Destroy(projectile.gameObject);
        }

        private IEnumerator DirectGreenAtTarget(Rigidbody2D projectile, Transform target)
        {
            Vector2 originalVelocity = projectile.velocity;
            float timer = 0f;
            float duration = .4f;
            while (projectile.gameObject.activeSelf && !target.GetComponent<BaseEnemy>().IsDestroyed())
            {
                projectile.velocity = originalVelocity.magnitude * Vector2.Lerp(originalVelocity, (Vector2)target.position - projectile.position, timer / duration).normalized;
                timer += Time.deltaTime;
                yield return null;
            }
        }

        private void PowerAttackRed()
        {
            float projectileSpeed = 26f;
            float arc = 360f;
            float spread = 45f;
            float angle = 0f;
            while (angle < arc)
            {
                GameObject proj = Instantiate(ProjectilePrefab);
                proj.transform.localEulerAngles = new Vector3(0f, 0f, angle);
                proj.GetComponent<Rigidbody2D>().velocity = proj.transform.right * projectileSpeed;
                proj.transform.position = (Vector2)transform.position + proj.GetComponent<Rigidbody2D>().velocity.normalized * 1f;
                angle += spread;
            }
        }

        private void PowerAttackGreen()
        {

        }

        private void PowerAttackBlue()
        {

        }

        private Vector2 GetMoveDirection()
        {
            Vector2 dir = Trixie.Instance.GetComponent<PlayerController>().GetMoveDirection();
            if (dir.y > Mathf.Abs(dir.x))
            {
                return Vector2.up;
            }
            else if (dir.y < -Mathf.Abs(dir.x))
            {
                return Vector2.down;
            }
            else if (Trixie.Instance.IsFacingRight())
            {
                return Vector2.right;
            }
            else
            {
                return Vector2.left;
            }
        }
    }
}


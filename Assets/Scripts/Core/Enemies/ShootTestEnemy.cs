using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrixieCore
{

    public class ShootTestEnemy : BaseEnemy
    {

        public float ShootCooldown = 1f;
        public Transform Turrets;

        private float cooldown;

        private ShootComponent shootComponent;

        private float cdTimer = 0f;
        private Coroutine damageRoutine;
        private SpriteRenderer[] spriteRenderer;

        private enum States
        {
            None, TakingDamage, Dead, Stunned
        }
        private States state;

        private void Start()
        {
            cooldown = Random.Range(ShootCooldown, ShootCooldown * 2f);
            shootComponent = GetComponent<ShootComponent>();
            spriteRenderer = Model.GetComponentsInChildren<SpriteRenderer>();
        }

        protected override void Update()
        {
            base.Update();

            if (state == States.Dead || isStunned) return;

            if (Vector2.Distance(Player.Instance.transform.position, transform.position) < AggroRadius)
            {
                // TODO only if raycasted *or* has seen recently. proper aggro. not this through the walls crap
                HasAggro();
            }
            else if (hasAggro)
            {
                LostAggro();
            }

            if (Turrets != null)
            {
                Vector2 distVector = Player.Instance.transform.position - Turrets.transform.position;
                float targetRotation = Mathf.Atan2(distVector.y, distVector.x) * Mathf.Rad2Deg;
                Turrets.eulerAngles = new Vector3(0f, 0f, targetRotation);
            }

            if (cdTimer < cooldown)
            {
                if (state == States.None)
                {
                    cdTimer += Time.deltaTime;
                }
            }
            else
            {
                cooldown = Random.Range(ShootCooldown, ShootCooldown * 2f);
                cdTimer = 0f;
                shootComponent.Shoot(Player.Instance.gameObject);
            }
        }

        public override void DamageEnemy(Vector2 direction, EnergyTypes.Colours energyColour, int damage = 1)
        {
            healthComponent.TakeDamage(damage);
            if (healthComponent.IsDead())
            {
                DestroyEnemy();
            }
            else
            {
                if (damageRoutine != null) StopCoroutine(damageRoutine);
                damageRoutine = StartCoroutine(ShowDamaged(direction));
            }
        }

        protected override void DestroyEnemy()
        {
            state = States.Dead;
            if (damageRoutine != null) StopCoroutine(damageRoutine);
            body.velocity = Vector2.zero;
            body.isKinematic = true;
            coll.enabled = false;
            Model.SetActive(false);
            LostAggro();
            
            StunEffect.SetActive(false);
            Turrets.gameObject.SetActive(false);
            DeathEffect.SetActive(true);
            AudioMaster.PlayAudio(AudioMaster.AudioClips.Explosion1);
        }

        protected override IEnumerator ShowDamaged(Vector2 direction)
        {
            state = States.TakingDamage;

            body.velocity = direction.normalized * 14f;

            foreach (var r in spriteRenderer)
            {
                if (r.name != "FlameGreen")
                {
                    r.color = new Color(1f, 0f, 0f, 0.6f);
                }
            }
            yield return new WaitForSeconds(0.3f);
            foreach (var r in spriteRenderer)
            {
                if (r.name != "FlameGreen")
                {
                    r.color = Color.white;
                }
            }

            body.velocity = Vector2.zero;
            yield return new WaitForSeconds(0.7f);
            state = States.None;
        }

        private void SetRendererColour(Color color)
        {
            foreach (SpriteRenderer r in spriteRenderer)
            {
                r.color = color;
            }
        }
    }

}

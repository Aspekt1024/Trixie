using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrixieCore
{

    public abstract class BaseEnemy : MonoBehaviour
    {

        public GameObject AI;
        public GameObject Model;
        public GameObject DeathEffect;

        public float AggroRadius;
        public event Action OnDeathCallback = delegate { };

        protected HealthComponent healthComponent;
        protected Animator anim;
        protected Collider2D coll;
        protected Rigidbody2D body;

        protected bool hasAggro;

        private Coroutine damagedRoutine;
        private bool directionFlipped;

        protected virtual void Awake()
        {
            healthComponent = GetComponent<HealthComponent>();
            anim = GetComponent<Animator>();
            body = GetComponent<Rigidbody2D>();
            coll = GetComponent<Collider2D>();
            DeathEffect.SetActive(false);
        }

        protected virtual void Update() { }

        public virtual void Stun(Vector2 direction, float stunTime) { }

        public virtual void DamageEnemy(Vector2 direction, EnergyTypes.Colours energyType, int damage = 1)
        {
            healthComponent.TakeDamage(damage);
            if (healthComponent.IsDead())
            {
                DestroyEnemy();
            }
            else
            {
                OnDamaged(direction);
            }
        }

        public void HasAggro()
        {
            if (hasAggro) return;
            hasAggro = true;
            GameManager.Instance.MainCamera.GetComponent<CameraFollow>().AddObjectToFollow(transform);
        }

        public void LostAggro()
        {
            if (!hasAggro) return;
            hasAggro = false;
            GameManager.Instance.MainCamera.GetComponent<CameraFollow>().StopFollowingObject(transform);
        }

        public bool IsDead()
        {
            return healthComponent.IsDead();
        }

        public float GetHorizontalVelocity()
        {
            return body.velocity.x;
        }

        public void LookAtPosition(Vector2 position)
        {
            if (transform.position.x > position.x)
            {
                FaceInitialDirection();
            }
            else
            {
                FaceOppositeDirection();
            }
        }

        protected void FaceInitialDirection()
        {
            if (directionFlipped)
            {
                directionFlipped = false;
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, 1f);
            }
        }

        protected void FaceOppositeDirection()
        {
            if (!directionFlipped)
            {
                directionFlipped = true;
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, 1f);
            }
        }

        public bool DirectionFlipped
        {
            get { return directionFlipped; }
        }

        protected virtual void OnDamaged(Vector2 direction)
        {
            if (damagedRoutine != null)
            {
                StopCoroutine(damagedRoutine);
            }
            damagedRoutine = StartCoroutine(ShowDamaged(direction));
        }

        protected virtual IEnumerator ShowDamaged(Vector2 direction)
        {
            AudioMaster.PlayAudio(AudioMaster.AudioClips.Hit1);
            SetSpriteColour(new Color(1f, 0f, 0f, 0.5f));
            yield return new WaitForSeconds(0.2f);
            SetSpriteColour(Color.white);
        }

        protected void SetSpriteColour(Color color)
        {
            foreach (var sr in Model.GetComponentsInChildren<SpriteRenderer>())
            {
                sr.color = color;
            }
        }

        protected virtual void DestroyEnemy()
        {
            AI.SetActive(false);
            Model.SetActive(false);
            DeathEffect.SetActive(true);

            AudioMaster.PlayAudio(AudioMaster.AudioClips.Explosion1);

            body.gravityScale = 0;
            body.velocity = Vector2.zero;
            coll.enabled = false;

            if (OnDeathCallback != null)
            {
                OnDeathCallback.Invoke();
                OnDeathCallback = null;
            }

            if (hasAggro)
            {
                LostAggro();
            }
        }

        protected virtual void ResetEnemy()
        {
            AI.SetActive(true);
            Model.SetActive(true);
            DeathEffect.SetActive(false);
        }
    }
}


﻿using System;
using System.Collections;
using UnityEngine;

namespace TrixieCore.Units
{
    public abstract class BaseEnemy : MonoBehaviour, IDamageable
    {           
        public GameObject AI;
        public GameObject Model;
        public GameObject DeathEffect;
        public GameObject StunEffect;

        public float AggroRadius;
        public event Action OnDeathCallback = delegate { };

        protected HealthComponent healthComponent;
        protected Animator anim;
        protected Collider2D coll;
        protected Rigidbody2D body;

        protected bool hasAggro;

        private Coroutine damagedRoutine;
        private bool directionFlipped;

        protected bool isStunned;
        private float stunDuration;

        protected virtual void Awake()
        {
            healthComponent = GetComponent<HealthComponent>();
            DeathEffect.SetActive(false);
            StunEffect.SetActive(false);
        }

        protected virtual void Update()
        {
            if (IsDestroyed()) return;

            if (isStunned)
            {
                if (stunDuration > 0)
                {
                    stunDuration -= Time.deltaTime;
                }
                if (stunDuration <= 0)
                {
                    isStunned = false;
                    StunEffect.SetActive(false);
                    SetSpriteColour(Color.white);
                }
            }
        }

        public virtual void Stun(Vector2 direction, float stunTime)
        {
            if (isStunned || IsDestroyed()) return;

            SetSpriteColour(new Color(0.3f, 0.3f, 1f, 1f));
            StunEffect.SetActive(true);
            isStunned = true;
            stunDuration = stunTime;
        }

        public virtual void TakeDamage(int damage, Vector2 direction, EnergyTypes.Colours energyType)
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

        public bool IsDestroyed()
        {
            return healthComponent.IsDead();
        }

        public bool IsStunned { get { return isStunned; } }

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
            StunEffect.SetActive(false);

            if (damagedRoutine !=  null)
            {
                StopCoroutine(damagedRoutine);
            }

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


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEnemy : MonoBehaviour {

    public float AggroRadius;
    public bool hasAggro;

    protected HealthComponent healthComponent;
    protected Animator anim;
    private bool directionFlipped;
    private Action OnDeathCallback;

    private void Awake()
    {
        healthComponent = GetComponent<HealthComponent>();
        anim = GetComponent<Animator>();
    }
    
    protected virtual void Update() { }
    
    public void SetOnDeathAction(Action callback)
    {
        OnDeathCallback = callback;
    }

    public virtual void Stun(Vector2 direction, float stunTime) { }
    protected virtual void OnDamaged() { }

    public virtual void DamageEnemy(Vector2 direction, int damage = 1)
    {
        healthComponent.TakeDamage(damage);
        if (healthComponent.IsDead())
        {
            DestroyEnemy();
        }
        else
        {
            OnDamaged();
        }
    }
    
    protected virtual void DestroyEnemy()
    {
        // TODO setup delegate
        if (OnDeathCallback != null)
        {
            OnDeathCallback.Invoke();
        }

        if (hasAggro)
        {
            LostAggro();
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
}

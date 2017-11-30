using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemy : MonoBehaviour {

    protected Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Shield")
        {
            //DestroyEnemy();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Shield")
        {
            //DestroyEnemy();
        }
    }

    public virtual void DamageEnemy(int damage = 1)
    {
        DestroyEnemy();
    }

    protected virtual void DestroyEnemy() { }
}

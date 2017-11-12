using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    private Animator anim;


    private void Awake()
    {
        anim = GetComponent<Animator>();
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
    }
}

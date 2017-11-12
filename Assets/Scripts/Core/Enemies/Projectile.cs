using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    private Animator anim;

    public enum ProjectileType
    {
        Blue, Yellow, Red, Pink, Green
    }

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        Vector2 viewportPos = Camera.main.WorldToViewportPoint(transform.position);
        if (viewportPos.x < -0.5f || viewportPos.x > 1.5f || viewportPos.y < -0.5f || viewportPos.y > 1.5f)
        {
            gameObject.SetActive(false);
        }
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

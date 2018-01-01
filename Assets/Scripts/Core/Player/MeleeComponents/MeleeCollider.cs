using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeCollider : MonoBehaviour {

    private PolygonCollider2D meleeCollider;

    private void Start()
    {
        meleeCollider = GetComponent<PolygonCollider2D>();
        meleeCollider.enabled = false;
    }

    public void EnableCollider()
    {
        meleeCollider.enabled = true;
    }

    public void DisableCollider()
    {
        meleeCollider.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy")
        {
            collision.GetComponent<BaseEnemy>().DamageEnemy(collision.transform.position - transform.position, 1);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrixieCore;
using TrixieCore.Units;



/// <summary>
/// Prototype class to test enemy
/// </summary>
public class EnemyVertical : BaseEnemy {

    public float MovementSpeed;
    public GameObject Flame;
    public GameObject Spinner;
    
    private Collider2D enemyCollider;

    private enum States
    {
        Normal, Dead, Rising, Falling
    }
    private States state;

    private void Start()
    {
        body = GetComponent<Rigidbody2D>();
        enemyCollider = GetComponent<CircleCollider2D>();
        state = States.Rising;
        Flame.SetActive(true);
    }

    float riseTimer = 0f;
    float riseTime = 2f;
    float riseSpeed = 6f;
    float fallSpeed = -15f;
    float targetVelocity = 0f;
    float fallTime = 4f;
    float fallTimer = 0f;

    protected override void Update()
    {
        switch (state)
        {
            case States.Normal:
                // patrol, move etc
                // can we move and aim and shoot all at the same time?
                break;
            case States.Dead:
                break;
            case States.Rising:
                targetVelocity = riseSpeed;
                riseTimer += Time.deltaTime;
                if (riseTimer > riseTime)
                {
                    state = States.Falling;
                    fallTimer = 0f;
                    Flame.SetActive(false);
                }
                UpdateVelocity();
                break;
            case States.Falling:
                targetVelocity = fallSpeed;
                fallTimer += Time.deltaTime;
                if (fallTimer > fallTime)
                {
                    state = States.Rising;
                    riseTimer = 0f;
                    Flame.SetActive(true);
                }
                UpdateVelocity();
                break;
        }

    }

    float spinVelocity;

    private void UpdateVelocity()
    {
        body.velocity = new Vector2(body.velocity.x, Mathf.Lerp(body.velocity.y, targetVelocity, Time.deltaTime * 2f));

        if (targetVelocity > 0f)
        {
            spinVelocity = Mathf.Lerp(spinVelocity, body.velocity.y * 100f, Time.deltaTime * 2);
        }
        else
        {
            spinVelocity = Mathf.Lerp(spinVelocity, 0f, Time.deltaTime * 2);
        }
        Spinner.transform.Rotate(Vector3.back, Time.deltaTime * spinVelocity);
    }
    
    protected override void DestroyEnemy()
    {
        Flame.SetActive(false);
        enemyCollider.enabled = false;
        anim.Play("Explosion", 0, 0f);
        state = States.Dead;

        foreach (Rigidbody2D rb in Spinner.GetComponentsInChildren<Rigidbody2D>())
        {
            rb.isKinematic = false;
            rb.velocity = new Vector2(Random.Range(-15f, 15f), Random.Range(-15f, 15f));
            rb.angularVelocity = Random.Range(-500, 500f);
            rb.GetComponent<Collider2D>().enabled = false;
        }
    }
}

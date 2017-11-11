using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldComponent : MonoBehaviour {

    public GameObject ShieldObject;
    public Transform CenterPoint;

    private int shieldCharges;
    private float shieldDistance;
    private Rigidbody2D body;

    // TODO json!
    private const int maxShieldCharges = 3;
    private const float shootSpeed = 34f;
    private const float shieldShootDistance = 10f;
    private const float shieldRechargeTime = 3f;
    private const float distFromCenter = 2f;


    private enum States
    {
        None, Shielding, Firing,
    }
    private States state;

    private void Start()
    {
        body = ShieldObject.GetComponent<Rigidbody2D>();
        ShieldObject.SetActive(false);
    }

    private void Update()
    {
        switch (state)
        {
            case States.None:
                break;
            case States.Shielding:
                SetShieldPosition();
                break;
            case States.Firing:
                shieldDistance += Time.deltaTime * shootSpeed;
                if (shieldDistance >= shieldShootDistance)
                {
                    state = States.None;
                }
                break;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        switch (state)
        {
            case States.Shielding:
                BlockBullets(collision.collider.tag);
                break;
            case States.Firing:
                DestroyEnemies(collision.gameObject);
                break;
        }
    }

    private void BlockBullets(string tag)
    {
        if (tag == "Bullet")
        {
            RemoveCharge();
        }
    }

    private void DestroyEnemies(GameObject other)
    {
        // TODO check if shield hits enemy
    }

    public void ShieldPressed()
    {
        if (state == States.None)
        {
            state = States.Shielding;
            body.isKinematic = true;
            ShieldObject.SetActive(true);
            SetShieldPosition();
        }
    }

    public void ShieldReleased()
    {
        if (state == States.Shielding)
        {
            state = States.None;
            body.isKinematic = true;
            body.velocity = Vector2.zero;
            ShieldObject.SetActive(false);
        }
    }

    public void Shoot()
    {
        if (state == States.Shielding)
        {
            state = States.Firing;
            shieldDistance = 0f;
            body.isKinematic = false;
            body.velocity = shootSpeed * body.transform.right;
        }
    }

    private void SetShieldPosition()
    {
        Vector2 mPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 distVector = (mPos - (Vector2)CenterPoint.position).normalized * distFromCenter;
        ShieldObject.transform.position = CenterPoint.position + (Vector3)distVector;
        float angle = Mathf.Rad2Deg * Mathf.Atan2(distVector.y, distVector.x);
        ShieldObject.transform.localEulerAngles = new Vector3(0f, 0f, angle);
    }

    private void RemoveCharge(int chargesToRemove = 1)
    {
        shieldCharges = Mathf.Max(0, shieldCharges + chargesToRemove);
    }

    private void AddCharge(int chargesToAdd = 1)
    {
        shieldCharges = Mathf.Min(maxShieldCharges, shieldCharges + chargesToAdd);
    }
}

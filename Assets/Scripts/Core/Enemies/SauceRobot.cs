using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GoapLabels = GoapConditions.Labels;

public class SauceRobot : BaseEnemy {

    private Transform movementTf;
    private bool isShrunk;
    private Vector2 startPosition;
    
    private EnemyAITest pathFinder;
    private VisionComponent vision;
    private SpriteRenderer spriteRenderer;
    
    public void SetShrunkState()
    {
        isShrunk = true;
    }

    private void Start()
    {
        movementTf = new GameObject("MovementTf").transform;
        

        pathFinder = GetComponent<EnemyAITest>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        vision = GetComponent<VisionComponent>();
        vision.Activate();

        startPosition = transform.position;
    }

    public void Update()
    {
        UpdateLookDirection();
    }

    public GameObject GetTargetObject()
    {
        return movementTf.gameObject;
    }

    public void SetTargetPosition(Vector2 position)
    {
        movementTf.position = position;
    }

    public override void DamageEnemy(Vector2 direction, int damage = 1)
    {
        Debug.Log("success");
        HealthComponent healthComponent = GetComponent<HealthComponent>();
        healthComponent.TakeDamage(damage);
        if (healthComponent.IsDead())
        {
            DestroyEnemy();
        }
        else
        {
            // TODO can't overlap!
            StartCoroutine(ShowDamaged());
        }
    }
    
    private IEnumerator ShowDamaged()
    {
        spriteRenderer.color = new Color(1f, 0f, 0f, 0.5f);
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.color = Color.white;
    }

    private void UpdateLookDirection()
    {
        if (pathFinder.FinishedPathing()) return;
        if (transform.position.x > pathFinder.GetTargetPosition().x)
        {
            vision.FaceInitialDirection();
        }
        else
        {
            vision.FaceOppositeDirection();
        }
    }

    protected override void DestroyEnemy()
    {
        gameObject.SetActive(false);
    }
}

using System.Collections;
using UnityEngine;

public class SmartBot : BaseEnemy {
    
    private EnemyAITest pathFinder;
    private VisionComponent vision;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        pathFinder = GetComponent<EnemyAITest>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        vision = GetComponent<VisionComponent>();
        vision.Activate();
    }

    private void Update()
    {
        UpdateLookDirection();
    }
    
    public override void DamageEnemy(Vector2 direction, int damage = 1)
    {
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
        Vector2 lookAtPosition;
        if (vision.CanSeePlayer())
        {
            lookAtPosition = Player.Instance.transform.position;
        }
        else if (!pathFinder.FinishedPathing())
        {
            lookAtPosition = pathFinder.GetTargetPosition();
        }
        else
        {
            return;
        }
        
        if (transform.position.x > lookAtPosition.x)
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

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

    protected override void DestroyEnemy()
    {
        gameObject.SetActive(false);
    }

    protected override void OnDamaged()
    {
        // TODO can't overlap!
        StartCoroutine(ShowDamaged());
    }

    private IEnumerator ShowDamaged()
    {
        spriteRenderer.color = new Color(1f, 0f, 0f, 0.5f);
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.color = Color.white;
    }
    
}

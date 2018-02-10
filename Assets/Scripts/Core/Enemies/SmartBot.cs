using System.Collections;
using UnityEngine;

public class SmartBot : BaseEnemy {

    public GameObject Sprites;
    public GameObject ExplosionEffect;
    public GameObject AI;

    private EnemyAITest pathFinder;
    private VisionComponent vision;
    private Collider2D coll;
    private Rigidbody2D body;

    private void Start()
    {
        body = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        pathFinder = GetComponent<EnemyAITest>();
        vision = GetComponent<VisionComponent>();
        vision.Activate();
        ExplosionEffect.SetActive(false);
    }

    protected override void DestroyEnemy()
    {
        body.velocity = Vector2.zero;
        coll.enabled = false;
        AI.SetActive(false);
        Sprites.SetActive(false);
        ExplosionEffect.SetActive(true);
        AudioMaster.PlayAudio(AudioMaster.AudioClips.Explosion1);

        StartCoroutine(DestroyCompletionDelay(2f));
    }


    private IEnumerator DestroyCompletionDelay(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        base.DestroyEnemy();
    }


    protected override void OnDamaged()
    {
        // TODO can't overlap!
        StartCoroutine(ShowDamaged());
    }

    private IEnumerator ShowDamaged()
    {
        SetSpriteColour(new Color(1f, 0f, 0f, 0.5f));
        yield return new WaitForSeconds(0.2f);
        SetSpriteColour(Color.white);
    }

    private void SetSpriteColour(Color color)
    {
        foreach (var sr in Sprites.GetComponentsInChildren<SpriteRenderer>())
        {
            sr.color = color;
        }
    }

}

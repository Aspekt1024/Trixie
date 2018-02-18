using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventArea : MonoBehaviour {

    public bool FocusCamera;
    public bool HasExitEffects;
    public TriggerableObject[] triggerableObjects;
    
    public enum ExitEffects
    {
        EnemyDestroyed
    }
    public ExitEffects ExitEffect;

    public GameObject WatchedObject;

    private bool isTriggered;

	private void Start () {
		if (HasExitEffects)
        {
            if (ExitEffect == ExitEffects.EnemyDestroyed && WatchedObject != null)
            {
                BaseEnemy enemy = WatchedObject.GetComponent<BaseEnemy>();
                if (enemy != null)
                {
                    enemy.OnDeathCallback += OnExitEffectSuccess;
                }
            }
        }
	}
	
	private void Update () {
		
	}

    private void OnExitEffectSuccess()
    {
        float delayTime = 2f;
        Invoke("DestroyCompletionDelay", delayTime);
    }


    private void DestroyCompletionDelay()
    {
        foreach (var obj in triggerableObjects)
        {
            obj.Reset();
        }
    }


    private void Activate()
    {
        if (triggerableObjects != null)
        {
            foreach (var obj in triggerableObjects)
            {
                obj.Trigger();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isTriggered) return;

        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            isTriggered = true;
            Activate();
        }
    }
}

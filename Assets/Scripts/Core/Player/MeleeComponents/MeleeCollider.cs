using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeCollider : MonoBehaviour {

    private MeleeComponent meleeComponent;
    private PolygonCollider2D meleeCollider;

    private void Start()
    {
        meleeComponent = Player.Instance.GetComponent<MeleeComponent>();
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
            HitEnemy(collision.GetComponent<BaseEnemy>());
        }
    }

    private void HitEnemy(BaseEnemy enemy)
    {
        Vector2 direction = enemy.transform.position - transform.position;
        switch (meleeComponent.GetMeleeColour())
        {
            case EnergyTypes.Colours.Blue:
                enemy.DamageEnemy(direction, 1);
                break;
            case EnergyTypes.Colours.Red:
                enemy.DamageEnemy(direction, 1);
                break;
            case EnergyTypes.Colours.Green:
                enemy.Stun(direction, 1);
                break;
            default:
                break;
        }
    }
}

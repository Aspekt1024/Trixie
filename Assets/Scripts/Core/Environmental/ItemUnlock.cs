using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemUnlock : MonoBehaviour {

    public enum UnlockType
    {
        ShieldBlue, ShieldGreen, ShieldRed, ShieldShoot, ShieldMelee,
        Boosters,
    }
    public UnlockType Type;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            TriggerUnlock();
            gameObject.SetActive(false);
        }
    }

    private void TriggerUnlock()
    {
        switch (Type)
        {
            case UnlockType.ShieldBlue:
                Player.Instance.GetComponent<ShieldComponent>().ObtainedUnlock(Type);
                break;
            case UnlockType.ShieldGreen:
                Player.Instance.GetComponent<ShieldComponent>().ObtainedUnlock(Type);
                break;
            case UnlockType.ShieldRed:
                Player.Instance.GetComponent<ShieldComponent>().ObtainedUnlock(Type);
                break;
            case UnlockType.ShieldShoot:
                Player.Instance.GetComponent<ShieldComponent>().ObtainedUnlock(Type);
                break;
            case UnlockType.ShieldMelee:
                Player.Instance.GetComponent<ShieldComponent>().ObtainedUnlock(Type);
                break;
            case UnlockType.Boosters:
                break;
            default:
                break;
        }

        ShowItemText();
    }

    private void ShowItemText()
    {
        switch (Type)
        {
            case UnlockType.ShieldBlue:
                GameUIManager.ItemCollected("Blue Shield");
                break;
            case UnlockType.ShieldGreen:
                GameUIManager.ItemCollected("Green Shield");
                break;
            case UnlockType.ShieldRed:
                GameUIManager.ItemCollected("Red Shield");
                break;
            case UnlockType.ShieldShoot:
                GameUIManager.ItemCollected("Shield Shoot");
                break;
            case UnlockType.ShieldMelee:
                GameUIManager.ItemCollected("Shield Melee");
                break;
            case UnlockType.Boosters:
                GameUIManager.ItemCollected("Booster Boots");
                break;
            default:
                break;
        }
    }
}

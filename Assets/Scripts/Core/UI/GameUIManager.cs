using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUIManager : MonoBehaviour {

    public static GameUIManager Instance;

    public HealthUI HealthUI;
    public BoostUI BoostUI;
    public ShieldUI ShieldUI;

    public ItemCollectOverlay ItemCollectOverlay;
    public ConversationUI ConversationUI;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("Multiple GameUIManagers found in scene. There should only be one!");
            Destroy(gameObject);
            return;
        }
    }

    public static void UpdateHealth(int health) { Instance.HealthUI.UpdateHealth(health); }
    public static void UpdateBoostPercentage(float percentage) { Instance.BoostUI.UpdateBoostPercentage(percentage); }
    public static void SetShieldColour(EnergyTypes.Colours shieldColour) { Instance.ShieldUI.SetShieldColour(shieldColour); }
    public static void SetShieldPower(EnergyTypes.Colours colour, float powerRatio) { Instance.ShieldUI.SetShieldPower(colour, powerRatio); }

    //TODO hide health show health
    public static void HideBoostIndicator() { Instance.BoostUI.HideBoostIndicator(); }
    public static void ShowBoostIndicator() { Instance.BoostUI.ShowBoostIndicator(); }
    public static void HideShieldIndicator() { Instance.ShieldUI.HideShieldIndicator(); }
    public static void ShowShieldIndicator() { Instance.ShieldUI.ShowShieldIndicator(); }
    public static void ShowShieldsDisabled() { Instance.ShieldUI.ShowShieldsDisabled(); }
    public static void ShowShieldsEnabled() { Instance.ShieldUI.ShowShieldsEnabled(); }

    public static void ItemCollected(string itemName) { Instance.ItemCollectOverlay.ShowItemText(itemName); }

    public static void ShowConversation(string text) { Instance.ConversationUI.Show(text); }
    public static void HideConversation() { Instance.ConversationUI.Hide(); }
}

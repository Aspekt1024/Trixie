using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUIManager : MonoBehaviour {

    public static GameUIManager Instance;

    public HealthUI HealthUI;

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

    public static void UpdateHealth(int health)
    {
        Instance.HealthUI.UpdateHealth(health);
    }
}

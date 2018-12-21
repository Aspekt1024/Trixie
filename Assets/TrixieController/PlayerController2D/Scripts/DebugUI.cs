using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugUI : MonoBehaviour {

    public float TextDuration = 3f;
    public Text DebugTextBlock;
    public Image BoostIndicator;

    [HideInInspector] public static DebugUI Instance;

    private static DebugUI instance;

    private float textTimer;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            ClearText();
        }
        else
        {
            Destroy(this);
        }
    }

    private void Update()
    {
        if (textTimer > 0)
        {
            textTimer -= Time.deltaTime;
            if (textTimer <= 0)
            {
                ClearText();
            }
        }
    }

    public static void SetBoostIndicatorFill(float fillAmount)
    {
        instance.BoostIndicator.fillAmount = fillAmount;
    }

    public static void SetText(string text)
    {
        instance.textTimer = instance.TextDuration;
        instance.DebugTextBlock.text = text;
    }

    private void ClearText()
    {
        SetText("");
    }
}

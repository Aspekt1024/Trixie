using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShieldUIModule : MonoBehaviour {
    
    public Image ShieldImage;
    private Image backgroundImage;

    private float powerLevelPercent;
    private bool selected;

    private void Awake()
    {
        backgroundImage = GetComponent<Image>();
    }

    public void Hide()
    {
        backgroundImage.enabled = false;
        ShieldImage.enabled = false;
    }

    public void Show()
    {
        backgroundImage.enabled = true;
        ShieldImage.enabled = true;
    }

    public void SelectShield()
    {
        selected = true;
        SetShieldGraphic();
    }

    public void DeselectShield()
    {
        selected = false;
        SetShieldGraphic();
    }

    public void SetShieldPower(float powerRatio)
    {
        powerLevelPercent = powerRatio;
        SetShieldGraphic();
    }

    private void SetShieldGraphic()
    {
        ShieldImage.fillAmount = powerLevelPercent;
    }
}

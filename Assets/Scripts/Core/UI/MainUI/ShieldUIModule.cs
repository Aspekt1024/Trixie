using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShieldUIModule : MonoBehaviour {

    public Sprite SelectedOutline;
    public Sprite UnselectedOutline;
    
    public Image ShieldFillImage;
    public Image OutlineImage;
    public Image Background;

    private float powerLevelPercent;
    private bool selected;

    private float colorModifier = 1f;
    
    public void Hide()
    {
        OutlineImage.enabled = false;
        ShieldFillImage.enabled = false;
        Background.enabled = false;
    }

    public void Show()
    {
        OutlineImage.enabled = true;
        ShieldFillImage.enabled = true;
        Background.enabled = true;
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

    public void Disable()
    {
        colorModifier = 0.3f;
    }

    public void Enable()
    {
        colorModifier = 1f;
    }

    private void SetShieldGraphic()
    {
        if (selected)
        {
            OutlineImage.sprite = SelectedOutline;
            OutlineImage.color = Color.white * colorModifier;
            ShieldFillImage.color = Color.white * colorModifier;
            Background.color = Color.white * colorModifier * 0.5f;
        }
        else
        {
            OutlineImage.sprite = SelectedOutline;
            OutlineImage.color = Color.white * 0.5f * colorModifier;
            ShieldFillImage.color = Color.white * 0.7f * colorModifier;
            Background.color = Color.white * 0.5f * colorModifier;
        }
        
        ShieldFillImage.fillAmount = powerLevelPercent;
    }
}

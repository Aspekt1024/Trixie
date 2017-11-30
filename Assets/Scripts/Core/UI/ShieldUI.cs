using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShieldUI : MonoBehaviour {

    public Image ShieldIndicatorImage;
    
	private void Start ()
    {
        SetShieldColour(EnergyTypes.Colours.Blue);
	}

    public void SetShieldColour(EnergyTypes.Colours shieldColour)
    {
        switch (shieldColour)
        {
            case EnergyTypes.Colours.Blue:
                ShieldIndicatorImage.color = Color.blue;
                break;
            case EnergyTypes.Colours.Pink:
                ShieldIndicatorImage.color = Color.red;
                break;
            case EnergyTypes.Colours.Yellow:
                ShieldIndicatorImage.color = Color.yellow;
                break;
            default:
                break;
        }
    }

    public void HideShieldIndicator()
    {
        foreach (RectTransform rt in GetComponentsInChildren<RectTransform>())
        {
            rt.gameObject.SetActive(false);
        }
    }

    public void ShowShieldIndicator()
    {
        foreach (RectTransform rt in GetComponentsInChildren<RectTransform>())
        {
            rt.gameObject.SetActive(true);
        }
    }
    
}

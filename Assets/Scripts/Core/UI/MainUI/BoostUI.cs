using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoostUI : MonoBehaviour {

    public Image BoostMeterImage;
    
	private void Start () {
        BoostMeterImage.fillAmount = 0f;
	}

    public void UpdateBoostPercentage(float percentage)
    {
        percentage = Mathf.Clamp01(percentage);
        BoostMeterImage.fillAmount = percentage;
    }
    
    public void HideBoostIndicator()
    {
        foreach (RectTransform rt in GetComponentsInChildren<RectTransform>())
        {
            rt.gameObject.SetActive(false);
        }
    }

    public void ShowBoostIndicator()
    {
        foreach (RectTransform rt in GetComponentsInChildren<RectTransform>())
        {
            rt.gameObject.SetActive(true);
        }
    }
}

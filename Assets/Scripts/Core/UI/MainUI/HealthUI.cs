using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour {

    public Image Health1;
    public Image Health2;
    public Image Health3;
    public Image Health4;
    public Image Health5;

    private Color enabledColor;
    private Color disabledColor;

    private void Start()
    {
        enabledColor = Health1.color;
        disabledColor = new Color(enabledColor.r, enabledColor.g, enabledColor.b, 0.4f);

    }

    public void UpdateHealth(int health)
    {
        Health1.color = health >= 1 ? enabledColor : disabledColor;
        Health2.color = health >= 2 ? enabledColor : disabledColor;
        Health3.color = health >= 3 ? enabledColor : disabledColor;
        Health4.color = health >= 4 ? enabledColor : disabledColor;
        Health5.color = health >= 5 ? enabledColor : disabledColor;
    }
}

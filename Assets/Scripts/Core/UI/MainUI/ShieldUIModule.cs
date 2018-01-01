using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShieldUIModule : MonoBehaviour {

    public Sprite ShieldUnselectedEmpty;
    public Sprite ShieldUnselected1;
    public Sprite ShieldUnselected2;
    public Sprite ShieldUnselected3;
    public Sprite ShieldUnselected4;
    public Sprite ShieldUnselectedFull;

    public Sprite ShieldSelectedEmpty;
    public Sprite ShieldSelected1;
    public Sprite ShieldSelected2;
    public Sprite ShieldSelected3;
    public Sprite ShieldSelected4;
    public Sprite ShieldSelectedFull;

    private Image shieldImage;

    private int powerLevel;
    private bool selected;
    
	private void Awake ()
    {
        shieldImage = GetComponent<Image>();
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

    public void SetShieldPower(int power)
    {
        powerLevel = power;
        SetShieldGraphic();
    }

    private void SetShieldGraphic()
    {
        // Note: we're only using 3 levels (4 including empty) of charge
        switch (powerLevel)
        {
            case 0:
                shieldImage.sprite = selected ? ShieldSelectedEmpty : ShieldUnselectedEmpty;
                break;
            case 1:
                shieldImage.sprite = selected ? ShieldSelected1 : ShieldUnselected1;
                break;
            case 2:
                shieldImage.sprite = selected ? ShieldSelected3 : ShieldUnselected2;
                break;
            case 3:
                shieldImage.sprite = selected ? ShieldSelectedFull : ShieldUnselectedFull;
                break;
            case 4:
                shieldImage.sprite = selected ? ShieldSelectedFull : ShieldUnselectedFull;
                break;
            case 5:
                shieldImage.sprite = selected ? ShieldSelectedFull : ShieldUnselectedFull;
                break;
            default:
                break;
        }
    }
}

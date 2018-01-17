using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColourIndicator : MonoBehaviour {

    public Transform BlueIndicator;
    public Transform RedIndicator;
    public Transform GreenIndicator;
    public ParticleSystem IndicatorEffect;

    public AttackAction AttackActionRef;

    private void Start()
    {
        IndicatorEffect.Stop();
    }

    public void OnEnable()
    {
        AttackActionRef.OnShootPreparation += Show;
    }

    public void OnDisable()
    {
        AttackActionRef.OnShootPreparation -= Show;
    }

    public void Show(EnergyTypes.Colours colour)
    {
        switch (colour)
        {
            case EnergyTypes.Colours.Blue:
                IndicatorEffect.transform.position = BlueIndicator.position;
                break;
            case EnergyTypes.Colours.Red:
                IndicatorEffect.transform.position = RedIndicator.position;
                break;
            case EnergyTypes.Colours.Green:
                IndicatorEffect.transform.position = GreenIndicator.position;
                break;
            default:
                break;
        }
        IndicatorEffect.Play();
    }
}

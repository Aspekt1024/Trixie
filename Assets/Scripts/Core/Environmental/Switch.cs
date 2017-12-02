using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour {

    public enum SwitchTypes
    {
        Toggle, OnOnly
    }
    public SwitchTypes SwitchType;

    public GravityField TargetObject;
    public float OnValue;
    public float OffValue;
    public SpriteRenderer SwitchIndicator;

    private enum States
    {
        On, Off
    }
    private States state;

    private void Start()
    {
        SwitchOff();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" || collision.tag == "Projectile")
        {
            SwitchHit();
        }
    }

    private void SwitchHit()
    {
        if (SwitchType == SwitchTypes.OnOnly && state == States.Off)
        {
            SwitchOn();
        }
        else
        {
            if (state == States.Off)
            {
                SwitchOn();
            }
            else
            {
                SwitchOff();
            }
        }
    }

    private void SwitchOn()
    {
        state = States.On;
        TargetObject.Strength = OnValue;
        SwitchIndicator.color = new Color(.1f, .8f, .8f);
    }

    private void SwitchOff()
    {
        state = States.Off;
        TargetObject.Strength = OffValue;
        SwitchIndicator.color = new Color(.1f, .2f, .45f);
    }

}

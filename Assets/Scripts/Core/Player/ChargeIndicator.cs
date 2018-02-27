using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeIndicator : MonoBehaviour {
    
    private SpriteRenderer spriteRenderer;
    private Animator anim;

    private States state;

    public enum States
    {
        None, Charging, StageOne, StageTwo
    }

    public enum Positions
    {
        Forward, Back, Up, Down
    }

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    public void SetChargeState(States newState)
    {
        if (state == newState) return;
        state = newState;

        switch (newState)
        {
            case States.None:
                anim.enabled = false;
                spriteRenderer.sprite = null;
                break;
            case States.Charging:
                anim.enabled = true;
                anim.Play("Charging", 0, 0f);
                break;
            case States.StageOne:
                anim.enabled = true;
                anim.Play("StageOne", 0, 0f);
                break;
            case States.StageTwo:
                anim.enabled = true;
                anim.Play("StageTwo", 0, 0f);
                break;
            default:
                break;
        }
    }

    public void SetPosition(Positions pos)
    {
        switch (pos)
        {
            case Positions.Forward:
                transform.localPosition = new Vector2(1.6f, 1f);
                break;
            case Positions.Back:
                transform.localPosition = new Vector2(-1.6f, 1f);
                break;
            case Positions.Up:
                transform.localPosition = new Vector2(.3f, 3.5f);
                break;
            case Positions.Down:
                transform.localPosition = new Vector2(0f, -2.4f);
                break;
            default:
                break;
        }
    }


}

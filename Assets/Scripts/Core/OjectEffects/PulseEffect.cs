using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulseEffect : MonoBehaviour {

    public float PulseSize;
    public float PulseFrequency;

    private Vector2 originalScale;
    private float pulseDuration;

    private float timer;

    private enum States
    {
        None, Growing, Shrinking
    }
    private States state;

    private void Start () {
        pulseDuration = 0.5f / PulseFrequency;
        originalScale = transform.localScale;
        state = States.Growing;
	}

    private void Update () {
        timer += Time.deltaTime;
        if (timer >= pulseDuration)
        {
            timer = 0f;
            SwitchState();
        }

        switch (state)
        {
            case States.None:
                break;
            case States.Growing:
                transform.localScale = Mathf.Lerp(1, PulseSize, timer / pulseDuration) * originalScale;
                break;
            case States.Shrinking:
                transform.localScale = Mathf.Lerp(PulseSize, 1, timer / pulseDuration) * originalScale;
                break;
        }


    }

    private void SwitchState()
    {
        if (state == States.Growing)
        {
            state = States.Shrinking;
        }
        else
        {
            state = States.Growing;
        }
    }
}

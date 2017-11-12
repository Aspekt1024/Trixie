using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverEffect : MonoBehaviour {

    public float HoverDistance;
    public float HoverFrequency;

    private Vector3 originalPosition;
    private float pulseDuration;

    private float timer;

    private enum States
    {
        None, Rising, Falling
    }
    private States state;
    
    private void Start()
    {
        pulseDuration = 0.5f / HoverFrequency;
        originalPosition = transform.position;
        state = States.Rising;
    }
    
    private void Update()
    {
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
            case States.Rising:
                transform.position = originalPosition + Mathf.Lerp(0, HoverDistance, timer / pulseDuration) * Vector3.up;
                break;
            case States.Falling:
                transform.position = originalPosition + Mathf.Lerp(HoverDistance, 0, timer / pulseDuration) * Vector3.up;
                break;
        }
    }

    private void SwitchState()
    {
        if (state == States.Rising)
        {
            state = States.Falling;
        }
        else
        {
            state = States.Rising;
        }
    }
}

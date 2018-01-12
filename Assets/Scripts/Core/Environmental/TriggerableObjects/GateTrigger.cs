using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateTrigger : TriggerableObject {

    public Transform Gate;
    public Transform EndPosition;

    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
        anim.Play("RaiseGate", 0, 0f);
    }

    public override void Trigger()
    {
        anim.Play("LowerGate", 0, 0f);
    }

    public override void Reset()
    {
        anim.Play("RaiseGate", 0, 0f);
    }
}

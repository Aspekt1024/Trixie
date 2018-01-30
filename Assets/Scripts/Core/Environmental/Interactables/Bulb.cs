using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bulb : MonoBehaviour {

    private Animator anim;

    private enum Animations
    {
        Bounce, Idle
    }
    
	private void Start ()
    {
        anim = GetComponent<Animator>();
	}

    public void Bounce()
    {
        anim.Play(Animations.Bounce.ToString(), 0, 0f);
    }
}

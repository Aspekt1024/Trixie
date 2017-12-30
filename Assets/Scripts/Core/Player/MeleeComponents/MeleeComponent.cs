using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeComponent : MonoBehaviour {

    public float MeleeCooldown = 0.4f;
    public float MeleeDuration = 0.1f;
    public MeleeCollider meleeCollider;

    private bool isActive;
    
    private enum States
    {
        None, Attacking, Disabled
    }
    private States state;

    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public bool Activate()
    {
        if (state == States.None)
        {
            StartCoroutine(Melee());
            return true;
        }
        else
        {
            return false;
        }
    }

    public void Disable()
    {
        state = States.Disabled;
    }

    private IEnumerator Melee()
    {
        isActive = true;
        meleeCollider.EnableCollider();
        state = States.Attacking;
        float meleeTimer = 0f;
        anim.Play("Melee", 0, 0f);
        while (meleeTimer < MeleeCooldown)
        {
            if (meleeTimer > MeleeDuration)
            {
                isActive = false;
                meleeCollider.DisableCollider();
            }
            meleeTimer += Time.deltaTime;
            yield return null;
        }
        state = States.None;
    }

    public bool MeleeIsActive()
    {
        return isActive;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAction : GoapAction {

    public float CooldownDuration = 1f;

    private float cooldownTimer;

    private enum States
    {
        None, Shooting, HasShot
    }
    private States state;

    public AttackAction()
    {
        cooldownTimer = CooldownDuration;

        addPrecondition(GoapLabels.CanSeePlayer, true);
        addEffect(GoapLabels.KillPlayer, true);
    }

    private void Update()
    {
        if (cooldownTimer < CooldownDuration)
        {
            cooldownTimer += Time.deltaTime;
        }
    }
    
    public override bool CheckProceduralPrecondition(GameObject agent)
    {
        return cooldownTimer >= CooldownDuration;
    }

    public override bool IsDone()
    {
        return state == States.HasShot;
    }

    public override bool Perform(GameObject agent)
    {
        if (state == States.Shooting) return true;
        
        state = States.Shooting;
        
        StartCoroutine(ShootAnimation());
        return true;
    }

    private IEnumerator ShootAnimation()
    {
        float shootTimer = 0f;
        while (shootTimer < 0.5f)
        {
            shootTimer += Time.deltaTime;
            yield return null;
        }
        Debug.Log("shot!");

        cooldownTimer = 0f;
        state = States.HasShot;
    }

    public override bool RequiresInRange()
    {
        return false;
    }

    public override void ResetAction()
    {
        state = States.None;
    }
}

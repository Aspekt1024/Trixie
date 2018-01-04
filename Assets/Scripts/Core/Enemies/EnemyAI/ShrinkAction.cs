using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShrinkAction : GoapAction {

    private bool isShrunk;
    private Vector3 originalScale;

    public bool IsShrunk
    {
        get { return isShrunk; }
    }
    
    public ShrinkAction()
    {
        addPrecondition(GoapLabels.CanSeePlayer, true);
    }

    private void Start()
    {
        originalScale = transform.localScale;
    }

    public override void ResetAction()
    {
        isShrunk = false;
        transform.localScale = originalScale;
    }

    public override bool IsDone()
    {
        return isShrunk;
    }

    public override bool RequiresInRange()
    {
        return false;
    }

    public override bool CheckProceduralPrecondition(GameObject agent)
    {
        target = Player.Instance.gameObject;
        return true;
    }

    public override bool Perform(GameObject agent)
    {
        GetComponent<SauceRobot>().SetShrunkState();
        transform.localScale = originalScale * 0.5f;
        isShrunk = true;
        return true;
    }
}

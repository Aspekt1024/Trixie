using ReGoap.Unity;
using UnityEngine;
using TrixieCore.Goap;

public class GoapTestMem : ReGoapMemoryAdvanced<GoapLabels, object>
{
    protected override void Awake()
    {
        base.Awake();
        GetWorldState().Set(GoapLabels.HasCorrectProjectileColour, true);
    }

    public Vector2 GetLastKnownPlayerPosition()
    {
        if (GetWorldState().HasKey(GoapLabels.LastKnownPlayerPosition))
        {
            return (Vector2)GetWorldState().Get(GoapLabels.LastKnownPlayerPosition);
        }
        else
        {
            return Vector2.zero;
        }
    }

    public bool CheckCondition(GoapLabels conditionLabel)
    {
        if (GetWorldState().HasKey(conditionLabel))
        {
            return (bool)GetWorldState().Get(conditionLabel);
        }
        else
        {
            return false;
        }
    }

}

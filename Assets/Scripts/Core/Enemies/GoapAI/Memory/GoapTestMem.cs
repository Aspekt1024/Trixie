using ReGoap.Unity;
using UnityEngine;

using GoapLabels = GoapConditions.Labels;

public class GoapTestMem : ReGoapMemoryAdvanced<GoapLabels, object>
{
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

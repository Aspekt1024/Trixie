using ReGoap.Unity;
using TrixieCore.Goap;

public class EnemyGoapAgent : ReGoapAgent<GoapLabels, object>
{
    private BaseEnemy parent;

    public BaseEnemy Parent
    {
        get { return parent; }
    }

    protected override void Awake()
    {
        base.Awake();
        parent = GetComponentInParent<BaseEnemy>();
    }
}

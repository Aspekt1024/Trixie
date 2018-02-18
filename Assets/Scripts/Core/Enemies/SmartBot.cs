using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SmartBot : BaseEnemy {

    public Text ActionText;
    public Text GoalText;

    private EnemyPathfinder pathFinder;
    private VisionComponent vision;

    private void Start()
    {
        pathFinder = GetComponent<EnemyPathfinder>();
        vision = GetComponent<VisionComponent>();
        vision.Activate();
    }

    protected override void Update()
    {
        base.Update();
        if (ActionText.GetComponentInParent<Canvas>() == null) return;
        Transform canvasTf = ActionText.GetComponentInParent<Canvas>().transform;
        if (DirectionFlipped)
        {
            canvasTf.localScale = new Vector3(-Mathf.Abs(canvasTf.localScale.x), canvasTf.localScale.y, canvasTf.localScale.z);
        }
        else
        {
            canvasTf.localScale = new Vector3(Mathf.Abs(canvasTf.localScale.x), canvasTf.localScale.y, canvasTf.localScale.z);
        }
    }

    public void SetActionText(string text)
    {
        ActionText.text = text;
    }
    public void SetGoalText(string text)
    {
        GoalText.text = text;
    }
}

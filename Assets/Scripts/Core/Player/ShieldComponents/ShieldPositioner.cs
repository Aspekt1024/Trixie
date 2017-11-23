using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldPositioner : MonoBehaviour {

    public Transform CenterPoint;

    private Animator playerAnim;
    private bool directionReversed;

    private const float distFromCenter = 1.2f;

    private void Start()
    {
        playerAnim = Player.Instance.GetComponent<Animator>();
        directionReversed = false;
    }

    public void SetShieldPosition()
    {
        Vector2 aimDirection = GameManager.GetAimDirection();
        Vector2 distVector = (aimDirection - (Vector2)CenterPoint.position).normalized * distFromCenter;
        transform.position = CenterPoint.position + (Vector3)distVector;
        float angle = Mathf.Rad2Deg * Mathf.Atan2(distVector.y, distVector.x);
        transform.localEulerAngles = new Vector3(0f, 0f, angle);

        if (Player.Instance.IsLookingRight())
        {
            playerAnim.SetFloat("lookAngle", angle / 360f);
        }
        else
        {
            playerAnim.SetFloat("lookAngle", Mathf.Sign(angle) * (0.5f - Mathf.Abs(angle) / 360f));
        }
    }
}

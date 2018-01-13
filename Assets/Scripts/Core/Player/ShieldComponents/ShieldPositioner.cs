using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldPositioner : MonoBehaviour {

    private Transform centerPoint;

    private Animator playerAnim;

    private const float distFromCenter = 1.2f;

    public void Setup(Transform centerPointReference)
    {
        centerPoint = centerPointReference;
        playerAnim = Player.Instance.GetComponent<Animator>();
    }

    public void SetShieldPosition()
    {
        Vector2 aimDirection = GameManager.GetAimDirection();
        if (aimDirection == Vector2.zero) return;

        Vector2 distVector = (aimDirection - (Vector2)centerPoint.position).normalized * distFromCenter;
        transform.position = centerPoint.position + (Vector3)distVector;
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

using Aspekt.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrixieCore
{
    public class ShieldPositioner : MonoBehaviour
    {
        public float DistFromCenter = 1.5f;
        public float ShieldAngle = -10f;
        public float OffsetY = -.5f;

        private Transform centerPoint;
        private Animator playerAnim;

        public Vector2 shieldDirection;

        public void Setup(Transform centerPointReference)
        {
            centerPoint = centerPointReference;
            playerAnim = Trixie.Instance.GetComponent<Animator>();
        }

        public void SetShieldPosition()
        {
            Vector2 aimDirection = Trixie.Instance.GetComponent<PlayerController>().GetDynamicDirection(Trixie.Instance.transform.position);
            if (aimDirection == Vector2.zero) return;

            Vector2 distVector = (aimDirection - (Vector2)centerPoint.position).normalized * DistFromCenter;
            transform.position = centerPoint.position + (Vector3)distVector;
            float angle = Mathf.Rad2Deg * Mathf.Atan2(distVector.y, distVector.x);

            if (Trixie.Instance.IsFacingRight())
            {
                playerAnim.SetFloat("lookAngle", angle / 360f);
            }
            else
            {
                playerAnim.SetFloat("lookAngle", Mathf.Sign(angle) * (0.5f - Mathf.Abs(angle) / 360f));
            }

            if (Mathf.Abs(angle) > 90f)
            {
                transform.localEulerAngles = new Vector3(0f, 0f, angle + 180f);
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, 1f);
            }
            else
            {
                transform.localEulerAngles = new Vector3(0f, 0f, angle);
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, 1f);
            }

        }

        public void SetShieldPositionFixed()
        {
            Vector2 direction = Trixie.Instance.GetComponent<PlayerController>().GetDynamicDirection(Trixie.Instance.transform.position) - (Vector2)centerPoint.position; ;
            if (direction.y > Mathf.Abs(direction.x))
            {
                shieldDirection = Vector3.up;
                if (Trixie.Instance.IsFacingRight())
                {
                    playerAnim.SetFloat("lookAngle", 90f / 360f);
                    transform.localPosition = new Vector2(-OffsetY, DistFromCenter);
                    transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, 1f);
                    transform.localEulerAngles = new Vector3(0f, 0f, 90f + ShieldAngle);
                }
                else
                {
                    playerAnim.SetFloat("lookAngle", 90f / 360f);
                    transform.localPosition = new Vector2(OffsetY, DistFromCenter);
                    transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, 1f);
                    transform.localEulerAngles = new Vector3(0f, 0f, -90f - ShieldAngle);
                }
            }
            else if (direction.y < -Mathf.Abs(direction.x))
            {
                shieldDirection = Vector3.down;
                if (Trixie.Instance.IsFacingRight())
                {
                    playerAnim.SetFloat("lookAngle", -90f / 360f);
                    transform.localPosition = new Vector2(-OffsetY, -DistFromCenter);
                    transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, 1f);
                    transform.localEulerAngles = new Vector3(0f, 0f, 90f - ShieldAngle);
                }
                else
                {
                    playerAnim.SetFloat("lookAngle", -90f / 360f);
                    transform.localPosition = new Vector2(OffsetY, -DistFromCenter);
                    transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, 1f);
                    transform.localEulerAngles = new Vector3(0f, 0f, -90f + ShieldAngle);
                }
            }
            else
            {
                if (Trixie.Instance.IsFacingRight())
                {
                    shieldDirection = Vector3.right;
                    playerAnim.SetFloat("lookAngle", 0 / 360f);
                    transform.localPosition = new Vector2(DistFromCenter, OffsetY);
                    transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, 1f);
                    transform.localEulerAngles = new Vector3(0f, 0f, ShieldAngle);
                }
                else
                {
                    shieldDirection = Vector3.left;
                    playerAnim.SetFloat("lookAngle", 0 / 360f);
                    transform.localPosition = new Vector2(-DistFromCenter, OffsetY);
                    transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, 1f);
                    transform.localEulerAngles = new Vector3(0f, 0f, -ShieldAngle);
                }
            }
        }
    }
}

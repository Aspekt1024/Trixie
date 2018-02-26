﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldPositioner : MonoBehaviour {

    public float DistFromCenter = 1.5f;
    public float ShieldAngle = -10f;
    public float OffsetY = -.5f;

    private Transform centerPoint;
    private Animator playerAnim;

    public Vector2 shieldDirection;

    public void Setup(Transform centerPointReference)
    {
        centerPoint = centerPointReference;
        playerAnim = Player.Instance.GetComponent<Animator>();
    }

    public void SetShieldPosition()
    {
        Vector2 aimDirection = GameManager.GetAimDirection();
        if (aimDirection == Vector2.zero) return;

        Vector2 distVector = (aimDirection - (Vector2)centerPoint.position).normalized * DistFromCenter;
        transform.position = centerPoint.position + (Vector3)distVector;
        float angle = Mathf.Rad2Deg * Mathf.Atan2(distVector.y, distVector.x);
        
        if (Player.Instance.IsLookingRight())
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
        Vector2 direction = GameManager.GetAimDirection() - (Vector2)centerPoint.position;
        if (direction.y > Mathf.Abs(direction.x))
        {
            shieldDirection = Vector3.up;
            if (Player.Instance.IsLookingRight())
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
            if (Player.Instance.IsLookingRight())
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
            if (Player.Instance.IsLookingRight())
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

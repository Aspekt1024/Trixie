using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionComponent : MonoBehaviour {

    public float StartingAngle = 0f;
    public float Arc = 75f;
    public float Radius = 10f;
    public float CheckFrequency = 5f;

    private bool canSeePlayer;
    private LayerMask layers;

    private enum States
    {
        None, Active
    }
    private States state;

    private void Start()
    {
        layers = 1 << LayerMask.NameToLayer("Terrain") | 1 << LayerMask.NameToLayer("Player");
    }

    public void Activate()
    {
        if (state == States.Active) return;
        
        state = States.Active;
        InvokeRepeating("CheckForPlayer", 0.5f, 1f / CheckFrequency);
    }

    public void Deactivate()
    {
        state = States.None;
        CancelInvoke();
    }

    public bool CanSeePlayer() { return canSeePlayer; }
    
    private void CheckForPlayer()
    {
        Vector2 distVector = (Player.Instance.transform.position - transform.position);
        if (!IsWithinDistance(distVector) || !IsWithinArc(distVector))
        {
            canSeePlayer = false;
            return;
        }

        RaycastHit2D hit = Physics2D.Raycast(transform.position, distVector, Radius, layers);
        if (hit.collider == null)
        {
            canSeePlayer = false;
        }
        else
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Terrain"))
            {
                canSeePlayer = false;
            }
            else
            {
                canSeePlayer = true;
            }
        }
    }

    private bool IsWithinDistance(Vector2 distVector)
    {
        return distVector.magnitude < Radius;
    }

    private bool IsWithinArc(Vector2 distVector)
    {
        float angle = Mathf.Atan2(distVector.y, distVector.x) * Mathf.Rad2Deg;
        float minAngle = StartingAngle;
        float maxAngle = StartingAngle + Arc;

        if (maxAngle > 180f)
        {
            maxAngle -= 360f;
            return (angle > minAngle && angle < 180f) || (angle > -180f && angle < maxAngle);
        }
        else
        {
            return (angle > minAngle && angle < maxAngle);
        }
    }
}

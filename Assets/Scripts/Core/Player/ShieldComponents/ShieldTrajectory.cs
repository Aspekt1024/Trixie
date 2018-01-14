using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrixieCore;

public class ShieldTrajectory : MonoBehaviour {

    private LineRenderer lineRenderer;
    private LayerMask collisionLayers;
    private float targetLength;
    private float timeSinceEnabled;
    private const float LINE_GROW_TIME = 0.2f;

    private enum States
    {
        Disabled, Enabled
    }
    private States state;

	private void Start ()
    {
        collisionLayers = 1 << TrixieLayers.GetMask(Layers.Terrain) | 1 << TrixieLayers.GetMask(Layers.Enemy);
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
	}

    private void Update()
    {
        if (state == States.Disabled) return;
        
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, 30f, collisionLayers);
        if (hit.collider == null)
        {
            targetLength = 30f;
        }
        else
        {
            targetLength = hit.distance;
        }

        float lineLength = targetLength;
        if (timeSinceEnabled < LINE_GROW_TIME)
        {
            timeSinceEnabled += Time.deltaTime;
            lineLength = Mathf.Lerp(0f, targetLength, timeSinceEnabled / LINE_GROW_TIME);
        }

        lineRenderer.SetPosition(1, Vector3.right * lineLength / transform.lossyScale.x);
    }

    public void Enable()
    {
        timeSinceEnabled = 0f;
        lineRenderer.enabled = true;
        state = States.Enabled;
    }

    public void Disable()
    {
        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }
        lineRenderer.enabled = false;
        state = States.Disabled;
    }
}

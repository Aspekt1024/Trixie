using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(VisionComponent))]
public class VisionComponentInspector : Editor {

    private VisionComponent current;

    private void OnSceneGUI()
    {
        current = (VisionComponent)target;
        
        // Ensure the angles are within an appropriate range
        if (current.StartingAngle > 180f) current.StartingAngle -= 360f;
        if (current.StartingAngle < -180f) current.StartingAngle += 360f;
        current.Arc = Mathf.Clamp(current.Arc, 0, 360f);

        // Check if the vision has been flipped (i.e. by movement) and calculate the correct angles
        float startingAngle = current.StartingAngle;

        if (current.DirectionFlipped)
        {
            startingAngle = Mathf.Sign(startingAngle) * (180f - Mathf.Abs(startingAngle)) - current.Arc;
        }

        Handles.color = new Color(1f, 0.3f, 0.3f, 0.3f);
        Handles.DrawSolidArc(current.transform.position, Vector3.forward, Quaternion.AngleAxis(startingAngle, Vector3.forward) * Vector3.right, current.Arc, current.Radius);
        Handles.color = Color.white;
    }
}

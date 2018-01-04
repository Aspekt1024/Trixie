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

        if (current.StartingAngle > 180f) current.StartingAngle -= 360f;
        if (current.StartingAngle < -180f) current.StartingAngle += 360f;
        current.Arc = Mathf.Clamp(current.Arc, 0, 360f);

        Handles.color = new Color(1f, 0.3f, 0.3f, 0.3f);
        Handles.DrawSolidArc(current.transform.position, Vector3.forward, Quaternion.AngleAxis(current.StartingAngle, Vector3.forward) * Vector3.right, current.Arc, current.Radius);
        Handles.color = Color.white;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ObjectPooler))]
public class ObjectPoolInspector : Editor {

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        UpdateProjectileNames();
    }

    private void UpdateProjectileNames()
    {

    }
}

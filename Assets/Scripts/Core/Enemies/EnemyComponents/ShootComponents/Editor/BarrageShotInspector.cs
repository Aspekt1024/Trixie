using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BarrageShot))]
public class BarrageShotInspector : Editor {

    private BarrageShot current;

    public override void OnInspectorGUI()
    {
        current = (BarrageShot)target;
        base.OnInspectorGUI();
    }
}

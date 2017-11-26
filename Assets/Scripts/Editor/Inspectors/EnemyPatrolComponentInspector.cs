using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EnemyPatrolComponent))]
public class EnemyPatrolComponentInspector : Editor {

    private EnemyPatrolComponent patrolComponent;
    
    private void OnSceneGUI()
    {
        patrolComponent = (EnemyPatrolComponent)target;

        if (Event.current.type == EventType.Repaint)
        {

            for (int i = 0; i < patrolComponent.PatrolPoints.Length; i++)
            {
                Handles.color = Color.red;
                Handles.CircleHandleCap(i, patrolComponent.PatrolPoints[i].position, Quaternion.identity, 1f, EventType.Repaint);
                Handles.color = Color.white;
                Handles.SphereHandleCap(i, patrolComponent.PatrolPoints[i].position, Quaternion.identity, 1f, EventType.Repaint);
            }
        }
    }

}

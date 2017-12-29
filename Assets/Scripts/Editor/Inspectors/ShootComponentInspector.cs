using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ShootComponent))]
public class ShootComponentInspector : Editor {

    private ShootComponent current;

    public override void OnInspectorGUI()
    {
        current = (ShootComponent)target;

        ShowCommonUI();

        switch (current.ShootType)
        {
            case ShootComponent.ShootTypes.OneShot:
                ShowOneShotUI();
                break;
            case ShootComponent.ShootTypes.Radial:
                ShowRadialUI();
                break;
            default:
                break;
        }
    }

    private void ShowCommonUI()
    {
        current.ShootType = (ShootComponent.ShootTypes)EditorGUILayout.EnumPopup("Shoot Type", current.ShootType);
        current.ProjectilePrefab = (GameObject)EditorGUILayout.ObjectField("Projectile Prefab", current.ProjectilePrefab, typeof(GameObject), false);
        current.ProjectileSpeed = EditorGUILayout.FloatField("Projectile Speed", current.ProjectileSpeed);
        current.ProjectileCooldown = EditorGUILayout.FloatField("Projectile Cooldown", current.ProjectileCooldown);
        current.VisibleRange = EditorGUILayout.FloatField("Visible Range", current.VisibleRange);

        GameObject currentShootObject = current.ShootPoint == null ? null : current.ShootPoint.gameObject;
        GameObject newShootPoint = ((GameObject)EditorGUILayout.ObjectField("Shoot Point", currentShootObject, typeof(GameObject), true));
        if (newShootPoint != null)
        {
            current.ShootPoint = newShootPoint.transform;
        }
        current.Turrets = (GameObject)EditorGUILayout.ObjectField("Turrets", current.Turrets, typeof(GameObject), true);

        for (int i = 0; i < current.TargetLayers.Length; i++)
        {
            //current.TargetLayers[i] = EditorGUILayout.
        }

        current.TargetLayers = new LayerMask[1];
        current.TargetLayers[0] = 1 << LayerMask.NameToLayer("Player");

        //current.TargetLayers = EditorGUILayout.

    }

    private void ShowOneShotUI()
    {
        current.ShootTarget = (ShootComponent.ShootTargets)EditorGUILayout.EnumPopup("Shoot Target", current.ShootTarget);
        if (current.ShootTarget == ShootComponent.ShootTargets.CustomPoint)
        {
            current.CustomTarget = (Transform)EditorGUILayout.ObjectField("Custom Target", current.CustomTarget, typeof(Transform), true);
        }
    }

    private void ShowRadialUI()
    {
        EditorGUILayout.Space();
        current.RadialClockwise = EditorGUILayout.Toggle("Radial Clockwise", current.RadialClockwise);

        current.RadialStartDegree = EditorGUILayout.FloatField("Start Angle", current.RadialStartDegree);
        current.RadialArc = EditorGUILayout.FloatField("Radial Arc (degrees)", current.RadialArc);
        current.RadialDistanceBetweenShots = EditorGUILayout.FloatField("Radial Spread", current.RadialDistanceBetweenShots);
        current.DelayBetweenRadialShots = EditorGUILayout.FloatField("Delay between shots", current.DelayBetweenRadialShots);
    }
}

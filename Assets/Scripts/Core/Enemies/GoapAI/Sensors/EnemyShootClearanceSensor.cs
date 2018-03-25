using ReGoap.Unity;
using UnityEngine;
using TrixieCore.Goap;
using TrixieCore;
using System.Collections.Generic;
using TrixieCore.Units;

public class EnemyShootClearanceSensor : ReGoapSensor<GoapLabels, object>
{

    public bool DebugMode = false;
    public GameObject PointPrefab;
    private Transform debugTf;

    private Transform shootPoint;
    private  ShootComponent shootComponent;
    
    private LayerMask hitLayers;
    
    private void Start()
    {
        memory = GetComponentInParent<EnemyGoapAgent>().GetMemory();
        shootComponent = GetComponentInParent<ShootComponent>();
        shootPoint = shootComponent.transform;
        hitLayers = 1 << TrixieLayers.GetMask(Layers.Terrain) | 1 << TrixieLayers.GetMask(Layers.Player);
    }

    public override void UpdateSensor()
    {
        float angle = Maths.CalculateThrowingAngle(
            shootPoint.position,
            Player.Instance.transform.position,
            false,
            shootComponent.ProjectileSpeed,
            shootComponent.ProjectileSettings.GravityScale
        );

        Vector2 velocity = Maths.SpeedToVelocity(shootComponent.ProjectileSpeed, angle);

        List<Vector2> points = new List<Vector2>();

        bool hitPlayer = false;
        bool hitTerrain = false;
        float maxDistance = 30f;
        const float timeStep = 0.05f;

        points.Add(shootPoint.position);

        while (!hitPlayer && !hitTerrain && Vector2.Distance(points[points.Count - 1], shootPoint.position) < maxDistance)
        {
            RaycastHit2D hit = Physics2D.CircleCast(points[points.Count - 1], 0.6f, velocity, velocity.magnitude * timeStep, hitLayers);
            if (hit.collider != null)
            {
                if (hit.collider.gameObject.layer == TrixieLayers.GetMask(Layers.Terrain))
                {
                    hitTerrain = true;
                }
                else
                {
                    hitPlayer = true;
                }
            }

            points.Add(points[points.Count - 1] + velocity * timeStep);
            velocity.y += Physics2D.gravity.y * shootComponent.ProjectileSettings.GravityScale * timeStep;
        }
        memory.GetWorldState().Set(GoapLabels.CanHitPlayer, hitPlayer);
        // TODO if points.Count is 1 and hitTerrain is true, the enemy needs to move from the terrain

        VisualiseDebugPoints(points);
    }

    private void VisualiseDebugPoints(List<Vector2> points)
    {
        if (debugTf != null)
        {
            Destroy(debugTf.gameObject);
        }

        if (DebugMode)
        {
            debugTf = new GameObject("traj").transform;
            debugTf.SetParent(transform);

            foreach (var point in points)
            {
                GameObject go = Instantiate(PointPrefab, point, Quaternion.identity, debugTf);
                go.name = "Trajectory";
            }
        }
    }
}

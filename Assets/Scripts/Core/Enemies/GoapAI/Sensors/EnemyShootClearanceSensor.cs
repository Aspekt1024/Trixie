using ReGoap.Unity;
using UnityEngine;
using TrixieCore.Goap;
using TrixieCore;
using System.Collections.Generic;

public class EnemyShootClearanceSensor : ReGoapSensor<GoapLabels, object>
{
    private Transform shootPoint;
    private  ShootComponent shootComponent;
    List<GameObject> testObjects = new List<GameObject>();
    
    private LayerMask hitLayers;

    private void Start()
    {
        memory = GetComponentInParent<EnemyGoapAgent>().GetMemory();
        shootComponent = GetComponentInParent<ShootComponent>();
        shootPoint = shootComponent.ShootPoint.transform;
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
            points.Add(points[points.Count - 1] + velocity * timeStep);
            velocity.y += Physics2D.gravity.y * shootComponent.ProjectileSettings.GravityScale * timeStep;

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
        }
        Debug.Log("can hit player: " + hitPlayer);
        memory.GetWorldState().Set(GoapLabels.CanHitPlayer, hitPlayer);
    }
}

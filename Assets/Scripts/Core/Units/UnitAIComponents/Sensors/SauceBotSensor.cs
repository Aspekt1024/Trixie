using System.Collections.Generic;
using UnityEngine;
using Aspekt.AI;
using TestUnitLabels;
using TrixieCore;
using TrixieCore.Units;

public class SauceBotSensor : AISensor {

    private VisionComponent vision;
    private LayerMask hitLayers;

    private void Start()
    {
        vision = agent.BaseUnit.GetAbility<VisionComponent>();
        vision.Activate();
        hitLayers = 1 << TrixieLayers.GetMask(Layers.Terrain) | 1 << TrixieLayers.GetMask(Layers.Player);
    }

    private void Update()
    {
        agent.GetMemory().UpdateCondition(SauceLabels.HasCorrectProjectColour, true);
        agent.GetMemory().UpdateCondition(SauceLabels.CanSeeTarget, vision.CanSeePlayer());

        if (agent.GetMemory().ConditionMet(SauceLabels.CanSeeTarget, true))
        {
            agent.GetMemory().UpdateCondition(SauceLabels.CanShootTarget, CanHitTarget());
        }
        else
        {
            agent.GetMemory().UpdateCondition(SauceLabels.CanShootTarget, false);
        }
    }

    private bool CanHitTarget()
    {
        ShootComponent shootComponent = agent.BaseUnit.GetAbility<ShootComponent>();

        float angle = Maths.CalculateThrowingAngle(
            shootComponent.transform.position,
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

        points.Add(shootComponent.transform.position);

        while (!hitPlayer && !hitTerrain && Vector2.Distance(points[points.Count - 1], shootComponent.transform.position) < maxDistance)
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
        // TODO if points.Count is 1 and hitTerrain is true, the enemy needs to move from the terrain

        return hitPlayer;
    }
}

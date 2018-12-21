using System.Collections.Generic;
using UnityEngine;
using Aspekt.AI;
using TestUnitLabels;
using TrixieCore;
using TrixieCore.Units;

public class SauceBotSensor : AISensor {

    public float AggroDuration = 3f;

    private EnemyShield shield;
    private VisionComponent vision;
    private LayerMask hitLayers;
    private AIMemory memory;

    private float aggroTimer;

    private void Start()
    {
        memory = agent.GetMemory();

        shield = agent.BaseUnit.GetAbility<EnemyShield>();
        vision = agent.BaseUnit.GetAbility<VisionComponent>();
        vision.Activate();
        hitLayers = 1 << TrixieLayers.GetMask(Layers.Terrain) | 1 << TrixieLayers.GetMask(Layers.Player);
    }

    private void Update()
    {
        memory.Set(SauceLabels.IsAggravated, CheckAggro(Time.deltaTime));

        memory.Set(SauceLabels.HasCorrectProjectColour, true);
        memory.Set(SauceLabels.CanSeeTarget, vision.CanSeePlayer());

        memory.Set(SauceLabels.CanShield, shield.IsAvailable);
        memory.Set(SauceLabels.IsShielded, shield.IsActive);
        
        if (memory.ConditionMet(SauceLabels.CanSeeTarget, true))
        {
            memory.Set(SauceLabels.CanShootTarget, CanHitTarget());
        }
        else
        {
            memory.Set(SauceLabels.CanShootTarget, false);
        }
        
    }

    private bool CheckAggro(float deltaTime)
    {
        float distanceFromTarget = Vector2.Distance(agent.BaseUnit.transform.position, Trixie.Instance.transform.position);
        bool aggroTriggered = memory.ConditionMet(SauceLabels.CanSeeTarget, true) && distanceFromTarget <= agent.BaseUnit.AggroRadius;

        if (aggroTriggered)
        {
            aggroTimer = AggroDuration;
            return true;
        }
        else if (memory.ConditionMet(SauceLabels.IsAggravated, true))
        {
            aggroTimer -= deltaTime;
            if (aggroTimer > 0)
            {
                return true;
            }
            else
            {
                agent.FindNewGoal();
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    // TODO set as separate class
    private bool CanHitTarget()
    {
        ShootComponent shootComponent = agent.BaseUnit.GetAbility<ShootComponent>();

        float angle = Maths.CalculateThrowingAngle(
            shootComponent.transform.position,
            Trixie.Instance.transform.position,
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Aspekt.AI;

namespace TrixieCore.Units
{
    public class FlyingMovementPathfinding : UnitAbility, IAIMovementBehaviour
    {
        public float MaxSpeed = 10f;
        public float MoveForce = 55f;

        private UnitPathfinder pathFinder;
        private Transform target;
        private Rigidbody2D body;

        private Vector2 previousVelocity;
        private float breakingTimer;
        private const float breakingDuration = 0.4f;
        
        private enum States
        {
            Stopped, Active, TargetReached
        }
        private States state;

        private void Start()
        {
            pathFinder = GetComponent<UnitPathfinder>();
            body = GetComponentInParent<BaseUnit>().GetBody();
        }

        public void Run(Transform target)
        {
            this.target = target;
            state = States.Active;
            pathFinder.Activate(target);
        }

        public void Stop()
        {
            pathFinder.Stop();
            state = States.Stopped;
            breakingTimer = 0f;
            previousVelocity = body.velocity;
        }

        public void SetTargetReached()
        {
            state = States.TargetReached;
            breakingTimer = 0f;
            previousVelocity = body.velocity;
            pathFinder.Stop();
        }

        public bool TargetReached()
        {
            return state == States.TargetReached;
        }

        public void UpdateTarget(Transform target)
        {
            this.target = target;
            pathFinder.Activate(target);
        }

        public void Tick(float deltaTime)
        {
            if (pathFinder.HasFinishedPathing())
            {
                SetTargetReached();
            }
            
            if (breakingTimer < breakingDuration)
            {
                breakingTimer += deltaTime;
            }

            switch (state)
            {
                case States.Stopped:
                    body.velocity = Vector2.Lerp(previousVelocity, Vector2.zero, breakingTimer / breakingDuration);
                    break;
                case States.Active:
                    if (body.velocity.magnitude < MaxSpeed)
                    {
                        body.AddForce(pathFinder.GetDirection() * MoveForce, ForceMode2D.Force);
                    }
                    break;
                case States.TargetReached:
                    body.velocity = Vector2.Lerp(previousVelocity, Vector2.zero, breakingTimer / breakingDuration);
                    break;
                default:
                    break;
            }
        }
    }
}
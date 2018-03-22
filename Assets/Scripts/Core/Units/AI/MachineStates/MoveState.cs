using UnityEngine;

namespace Aspekt.AI
{
    class MoveState : AIMachineState
    {
        private const float speed = 8f;

        private IAIMovementBehaviour movement;

        public override void Enter()
        {
            movement.Start();
        }

        public override void Tick(float deltaTime)
        {
            movement.Tick(deltaTime);

            if (movement.TargetReached())
            {
                StateComplete();
            }
        }

        public void SetMovementBehaviour(IAIMovementBehaviour movementBehaviour)
        {
            movement = movementBehaviour;
        }

        public void SetTarget(Transform target)
        {
            movement.SetTarget(target);
        }
    }
}

using UnityEngine;

namespace Aspekt.AI
{
    class MoveState : AIMachineState
    {
        private const float speed = 8f;
        private Transform target;

        private IAIMovementBehaviour movement;

        public override void Enter()
        {
            movement.Run(target);
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
            this.target = target;
            movement.UpdateTarget(target);
        }

        public void SetTargetReached()
        {
            movement.SetTargetReached();
        }
    }
}

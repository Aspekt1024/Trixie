using System;
using Aspekt.AI;
using UnityEngine;
using TestUnitLabels;

namespace TrixieCore.Units
{
    public class KeepMeleeDistanceAction : AIAction
    {
        public float DistanceToKeep = 6f;

        private MoveState moveState;
        private Transform target;
        private Vector2 direction;
        private float distanceToMove;

        public override void Enter(AIStateMachine stateMachine, Action SuccessCallback, Action FailureCallback)
        {
            base.Enter(stateMachine, SuccessCallback, FailureCallback);

            direction = Player.Instance.transform.position - agent.BaseUnit.transform.position;
            distanceToMove = DistanceToKeep - direction.magnitude;

            if (distanceToMove < 0)
            {
                MoveComplete();
                return;
            }

            target = agent.GetMoveTransform();
            target.position = (Vector2)agent.BaseUnit.transform.position - direction.normalized * distanceToMove;

            moveState = stateMachine.AddState<MoveState>();
            moveState.SetMovementBehaviour(agent.BaseUnit.GetMovementBehaviour());
            moveState.SetTarget(target);

            stateMachine.OnComplete += MoveComplete;
        }
        
        public override bool CheckProceduralPrecondition()
        {
            direction = Player.Instance.transform.position - agent.BaseUnit.transform.position;
            distanceToMove = DistanceToKeep - direction.magnitude;
            return distanceToMove > 0 && agent.GetMemory().ConditionMet(SauceLabels.CanShoot, false);
        }

        private void MoveComplete()
        {
            Success();
        }

        public override void Exit()
        {
            base.Exit();
            stateMachine.OnComplete -= Success;
        }

        protected override void Run(float deltaTime)
        {
            agent.BaseUnit.LookAtPosition(Player.Instance.transform.position);
        }
        
        protected override void SetPreconditions()
        {
            AddPrecondition(SauceLabels.CanShoot, false);
        }

        protected override void SetEffects()
        {
            AddEffect(SauceLabels.AttackGoalComplete, true);
        }
    }
}


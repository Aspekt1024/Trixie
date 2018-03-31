using System;
using Aspekt.AI;
using TestUnitLabels;

namespace TrixieCore.Units
{
    public class MoveToTargetAction : AIAction
    {
        private MoveState moveState;

        public event Action<EnergyTypes.Colours> OnShootPreparation = delegate { };

        public override void Enter(AIStateMachine stateMachine, Action SuccessCallback, Action FailureCallback)
        {
            base.Enter(stateMachine, SuccessCallback, FailureCallback);
            moveState = stateMachine.AddState<MoveState>();
            moveState.SetMovementBehaviour(agent.BaseUnit.GetMovementBehaviour());
            moveState.SetTarget(Player.Instance.transform);

            stateMachine.OnComplete += MovementComplete;
        }

        private void MovementComplete()
        {
            if (agent.GetMemory().ConditionMet(SauceLabels.CanSeeTarget, true))
            {
                Success();
            }
            else
            {
                Failure();
            }
        }

        protected override void Exit()
        {
            stateMachine.OnComplete -= MovementComplete;
        }

        protected override void Run()
        {
            if (moveState != null)
            {
                moveState.SetTarget(Player.Instance.transform);

                agent.BaseUnit.LookAtPosition(Player.Instance.transform.position);

                if (CanSeeAndShootTarget())
                {
                    moveState.SetTargetReached();
                }
            }

        }

        protected override void SetPreconditions()
        {
        }

        protected override void SetEffects()
        {
            AddEffect(SauceLabels.CanShootTarget, true);
            AddEffect(SauceLabels.CanSeeTarget, true);
        }

        private bool CanSeeAndShootTarget()
        {
            return agent.GetMemory().ConditionMet(SauceLabels.CanShootTarget, true)
                    && agent.GetMemory().ConditionMet(SauceLabels.CanSeeTarget, true);
        }
    }
}

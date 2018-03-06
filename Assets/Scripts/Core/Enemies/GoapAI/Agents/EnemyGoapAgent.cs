using System.Collections.Generic;
using ReGoap.Unity;
using ReGoap.Core;
using ReGoap.Utilities;
using UnityEngine;

namespace TrixieCore.Goap
{
    public class EnemyGoapAgent : ReGoapAgent<GoapLabels, object>
    {
        public GameObject GoalsObject;
        public GameObject ActionsObject;
        public GameObject SensorsObject;

        private BaseEnemy parent;
        private GoapStateMachine stateMachine;

        public BaseEnemy Parent
        {
            get { return parent; }
        }

        protected override void Awake()
        {
            base.Awake();
            parent = GetComponentInParent<BaseEnemy>();
            stateMachine = GetComponent<GoapStateMachine>();
        }

        protected override void Update()
        {
            base.Update();
            if (parent.IsDead())
            {
                stateMachine.enabled = false;
            }
        }

        public A GetAction<A>() where A : ReGoapAction<GoapLabels, object> { return ActionsObject.GetComponent<A>(); }
        public S GetSensor<S>() where S : ReGoapSensor<GoapLabels, object> { return SensorsObject.GetComponent<S>(); }
        public G GetGoal<G>() where G : ReGoapGoal<GoapLabels, object> { return GoalsObject.GetComponent<G>(); }

        public IReGoapSensor<GoapLabels, object>[] GetSensors()
        {
            return SensorsObject.GetComponentsInChildren<IReGoapSensor<GoapLabels, object>>();
        }

        public override IReGoapMemory<GoapLabels, object> GetMemory()
        {
            if (memory == null)
            {
                memory = GetComponent<IReGoapMemory<GoapLabels, object>>();
            }
            return memory;
        }

        public override void RefreshActionsSet()
        {
            actions = new List<IReGoapAction<GoapLabels, object>>(ActionsObject.GetComponents<IReGoapAction<GoapLabels, object>>());
        }

        public override void RefreshGoalsSet()
        {
            goals = new List<IReGoapGoal<GoapLabels, object>>(GoalsObject.GetComponents<IReGoapGoal<GoapLabels, object>>());
            possibleGoalsDirty = true;
        }

        public void RecalculateGoal()
        {
            CalculateNewGoal(true);
        }

        #region Debug
        protected override void ShowAction(IReGoapAction<GoapLabels, object> action)
        {
            if (parent.GetType().Equals(typeof(SmartBot)))
            {
                ((SmartBot)parent).SetActionText(action.GetType().ToString());
            }
        }


        protected override void ShowGoal(IReGoapGoal<GoapLabels, object> goal)
        {
            if (parent.GetType().Equals(typeof(SmartBot)))
            {
                ((SmartBot)parent).SetGoalText(goal.GetType().ToString());
            }
            else if (parent.GetType().Equals(typeof(TrixieCore.Guy2)))
            {
                //Debug.Log(goal.GetType().ToString());
            }
        }
        #endregion
    }
}


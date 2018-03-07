using System;
using System.Collections;
using UnityEngine;
using ReGoap.Core;
using ReGoap.Unity;

namespace TrixieCore.Goap
{
    public class ReturnToStartPointAction : ReGoapAction<GoapLabels, object>
    {

        private EnemyGoapAgent agentAI;
        private GotoState gotoState;
        private Vector2 startPosition;

        protected override void Awake()
        {
            base.Awake();
            agentAI = GetComponentInParent<EnemyGoapAgent>();
            gotoState = agentAI.GetComponent<GotoState>();
            startPosition = transform.position;
        }

        private void Update()
        {
            if (GetComponentInParent<VisionComponent>().HasSeenPlayerRecenty())
            {
                gotoState.Exit();
            }
        }

        public override ReGoapState<GoapLabels, object> GetEffects(ReGoapState<GoapLabels, object> goalState, IReGoapAction<GoapLabels, object> next = null)
        {
            effects.Clear();
            effects.Set(GoapLabels.Idle, true);
            return effects;
        }

        public override ReGoapState<GoapLabels, object> GetPreconditions(ReGoapState<GoapLabels, object> goalState, IReGoapAction<GoapLabels, object> next = null)
        {
            preconditions.Clear();
            //preconditions.Set(GoapLabels.NotSeenPlayerRecently, true);
            return preconditions;
        }

        public override bool CheckProceduralCondition(IReGoapAgent<GoapLabels, object> goapAgent, ReGoapState<GoapLabels, object> goalState, IReGoapAction<GoapLabels, object> next = null)
        {
            bool check = base.CheckProceduralCondition(goapAgent, goalState, next) && ((GoapTestMem)agent.GetMemory()).CheckCondition(GoapLabels.IsStunned) == false;
            return check;
        }

        //public override bool CheckProceduralCondition(IReGoapAgent<GoapLabels, object> goapAgent, ReGoapState<GoapLabels, object> goalState, IReGoapAction<GoapLabels, object> next = null)
        //{
        //    if (GetComponentInParent<VisionComponent>().HasSeenPlayerRecenty())
        //    {
        //        return false;
        //    }
        //    else
        //    {
        //        return base.CheckProceduralCondition(goapAgent, goalState, next);
        //    }
        //}

        public override void Run(IReGoapAction<GoapLabels, object> previous, IReGoapAction<GoapLabels, object> next, IReGoapActionSettings<GoapLabels, object> settings, ReGoapState<GoapLabels, object> goalState, Action<IReGoapAction<GoapLabels, object>> done, Action<IReGoapAction<GoapLabels, object>> fail)
        {
            base.Run(previous, next, settings, goalState, done, fail);
            gotoState.GoTo(startPosition, OnDoneCallback, OnFailCallback);
        }

        private void OnDoneCallback()
        {
            failCallback(this);
        }

        private void OnFailCallback()
        {
            failCallback(this);
        }
    }
}



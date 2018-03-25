﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace Aspekt.AI
{
    public abstract class AIAction : MonoBehaviour
    {
        public float Cost = 1f;

        protected AIAgent agent;
        protected AIStateMachine stateMachine;

        private Dictionary<string, object> preconditions = new Dictionary<string, object>();
        private Dictionary<string, object> effects = new Dictionary<string, object>();

        private Action SuccessCallback;
        private Action FailureCallback;

        private void Awake()
        {
            SetPreconditions();
            SetEffects();
        }

        public void SetAgent(AIAgent agent)
        {
            this.agent = agent;
        }

        public override string ToString()
        {
            return GetType().ToString();
        }

        public virtual void Enter(AIStateMachine stateMachine, Action SuccessCallback, Action FailureCallback)
        {
            this.stateMachine = stateMachine;
            this.SuccessCallback = SuccessCallback;
            this.FailureCallback = FailureCallback;
        }

        protected virtual void Exit()
        {
        }

        protected abstract void Update();
        protected abstract void SetPreconditions();
        protected abstract void SetEffects();

        public void Tick(float deltaTime)
        {
            if (CheckProceduralPrecondition())
            {
                Update();
            }
            else
            {
                Failure();
            }
        }

        public virtual bool CheckProceduralPrecondition()
        {
            // TODO check all preconditions are still fulfilled
            return true;
        }
        
        public Dictionary<string, object> GetPreconditions()
        {
            return preconditions;
        }

        public Dictionary<string, object> GetEffects()
        {
            return effects;
        }

        protected virtual void AddPrecondition(object label, object value)
        {
            preconditions.Add(label.ToString(), value);
        }

        protected virtual void AddEffect(object label, object value)
        {
            effects.Add(label.ToString(), value);
        }

        protected void Failure()
        {
            Exit();
            FailureCallback.Invoke();
        }

        protected void Success()
        {
            Exit();
            SuccessCallback.Invoke();
        }
    }
}

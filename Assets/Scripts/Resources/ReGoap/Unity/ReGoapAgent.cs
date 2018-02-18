﻿using System;
using System.Collections.Generic;
using System.Linq;
using ReGoap.Core;
using ReGoap.Utilities;
using UnityEngine;
using ReGoap.Unity;

namespace ReGoap.Unity
{
    public class ReGoapAgent<T, W> : MonoBehaviour, IReGoapAgent<T, W>, IReGoapAgentHelper
    {
        public string Name;
        public float CalculationDelay = 0.1f;
        public bool BlackListGoalOnFailure;

        public bool CalculateNewGoalOnStart = true;

        protected float lastCalculationTime;

        protected List<IReGoapGoal<T, W>> goals;
        protected List<IReGoapAction<T, W>> actions;
        protected IReGoapMemory<T, W> memory;
        protected IReGoapGoal<T, W> currentGoal;

        protected ReGoapActionState<T, W> currentActionState;

        protected Dictionary<IReGoapGoal<T, W>, float> goalBlacklist;
        protected List<IReGoapGoal<T, W>> possibleGoals;
        protected bool possibleGoalsDirty;
        protected Queue<ReGoapActionState<T, W>> startingPlan;
        protected Dictionary<T, W> planValues;
        protected bool interruptOnNextTransition;

        protected bool startedPlanning;
        protected ReGoapPlanWork<T, W> currentReGoapPlanWorker;
        public bool IsPlanning
        {
            get { return startedPlanning && currentReGoapPlanWorker.NewGoal == null; }
        }

        #region UnityFunctions
        protected virtual void Awake()
        {
            lastCalculationTime = -100;
            goalBlacklist = new Dictionary<IReGoapGoal<T, W>, float>();

            RefreshGoalsSet();
            RefreshActionsSet();
            RefreshMemory();
        }

        protected virtual void Start()
        {
            if (CalculateNewGoalOnStart)
            {
                CalculateNewGoal(true);
            }
        }

        private void Update()
        {
            if (currentActionState != null)
            {
                ShowAction(currentActionState.Action);
            }
            if (currentGoal !=  null)
            {
                ShowGoal(currentGoal);
            }
        }

        protected virtual void OnEnable()
        {

        }

        protected virtual void OnDisable()
        {
            if (currentActionState != null)
            {
                currentActionState.Action.Exit(null);
                currentActionState = null;
                currentGoal = null;
            }
        }
        #endregion

        protected virtual void UpdatePossibleGoals()
        {
            possibleGoalsDirty = false;
            if (goalBlacklist.Count > 0)
            {
                possibleGoals = new List<IReGoapGoal<T, W>>(goals.Count);
                foreach (var goal in goals)
                    if (!goalBlacklist.ContainsKey(goal))
                    {
                        possibleGoals.Add(goal);
                    }
                    else if (goalBlacklist[goal] < Time.time)
                    {
                        goalBlacklist.Remove(goal);
                        possibleGoals.Add(goal);
                    }
            }
            else
            {
                possibleGoals = goals;
            }
        }

        protected virtual void TryWarnActionFailure(IReGoapAction<T, W> action)
        {
            if (action.IsInterruptable())
                WarnActionFailure(action);
            else
                action.AskForInterruption();
        }

        protected virtual bool CalculateNewGoal(bool forceStart = false)
        {
            if (IsPlanning)
                return false;
            if (!forceStart && (Time.time - lastCalculationTime <= CalculationDelay))
                return false;
            lastCalculationTime = Time.time;

            interruptOnNextTransition = false;
            UpdatePossibleGoals();
            //var watch = System.Diagnostics.Stopwatch.StartNew();
            startedPlanning = true;
            currentReGoapPlanWorker = ReGoapPlannerManager<T, W>.Instance.Plan(this, BlackListGoalOnFailure ? currentGoal : null,
                currentGoal != null ? currentGoal.GetPlan() : null, OnDonePlanning);

            return true;
        }

        protected virtual void OnDonePlanning(IReGoapGoal<T, W> newGoal)
        {
            startedPlanning = false;
            currentReGoapPlanWorker = default(ReGoapPlanWork<T, W>);
            if (newGoal == null) { 
                if (currentGoal == null)
                {
                    ReGoapLogger.LogWarning("GoapAgent " + this + " could not find a plan.");
                }
                return;
            }

            if (currentActionState != null)
                currentActionState.Action.Exit(null);
            currentActionState = null;
            currentGoal = newGoal;
            startingPlan = currentGoal.GetPlan();
            ClearPlanValues();
            foreach (var actionState in startingPlan)
            {
                actionState.Action.PostPlanCalculations(this);
            }
            currentGoal.Run(WarnGoalEnd);
            PushAction();
        }

        public static string PlanToString(IEnumerable<IReGoapAction<T, W>> plan)
        {
            var result = "GoapPlan(";
            var reGoapActions = plan as IReGoapAction<T, W>[] ?? plan.ToArray();
            for (var index = 0; index < reGoapActions.Length; index++)
            {
                var action = reGoapActions[index];
                result += string.Format("'{0}'{1}", action, index + 1 < reGoapActions.Length ? ", " : "");
            }
            result += ")";
            return result;
        }

        public virtual void WarnActionEnd(IReGoapAction<T, W> thisAction)
        {
            if (currentActionState == null || currentActionState.Action == null)
            {
                if (currentActionState != null)
                {
                    Debug.LogError("WarnActionEnd: " + thisAction + " : " + currentActionState);
                }
                else
                {
                    Debug.LogError("WarnActionEnd: " + thisAction);
                }
                return;
            }
            if (thisAction != currentActionState.Action)
                return;
            PushAction();
        }

        protected virtual void PushAction()
        {
            if (interruptOnNextTransition)
            {
                CalculateNewGoal();
                return;
            }
            var plan = currentGoal.GetPlan();
            if (plan.Count == 0)
            {
                if (currentActionState != null)
                {
                    currentActionState.Action.Exit(currentActionState.Action);
                    currentActionState = null;
                }
                CalculateNewGoal();
            }
            else
            {
                var previous = currentActionState;
                currentActionState = plan.Dequeue();
                IReGoapAction<T, W> next = null;
                if (plan.Count > 0)
                    next = plan.Peek().Action;
                if (previous != null)
                    previous.Action.Exit(currentActionState.Action);
                currentActionState.Action.Run(previous != null ? previous.Action : null, next, currentActionState.Settings, currentGoal.GetGoalState(), WarnActionEnd, WarnActionFailure);
            }
        }

        protected virtual void ShowAction(IReGoapAction<T, W> action) { }
        protected virtual void ShowGoal(IReGoapGoal<T, W> goal) { }

        public virtual void WarnActionFailure(IReGoapAction<T, W> thisAction)
        {
            if (currentActionState != null && thisAction != currentActionState.Action)
            {
                ReGoapLogger.LogWarning(string.Format("[GoapAgent] Action {0} warned for failure but is not current action.", thisAction));
                return;
            }
            if (BlackListGoalOnFailure)
                goalBlacklist[currentGoal] = Time.time + currentGoal.GetErrorDelay();
            CalculateNewGoal(true);
        }

        public virtual void WarnGoalEnd(IReGoapGoal<T, W> goal)
        {
            if (goal != currentGoal)
            {
                ReGoapLogger.LogWarning(string.Format("[GoapAgent] Goal {0} warned for end but is not current goal.", goal));
                return;
            }
            CalculateNewGoal();
        }

        public virtual void WarnPossibleGoal(IReGoapGoal<T, W> goal)
        {
            if ((currentGoal != null) && (goal.GetPriority() <= currentGoal.GetPriority()))
                return;
            if (currentActionState != null && !currentActionState.Action.IsInterruptable())
            {
                interruptOnNextTransition = true;
                currentActionState.Action.AskForInterruption();
            }
            else
                CalculateNewGoal();
        }

        public virtual bool IsActive()
        {
            return enabled;
        }

        public virtual Queue<ReGoapActionState<T, W>> GetStartingPlan()
        {
            return startingPlan;
        }

        protected virtual void ClearPlanValues()
        {
            if (planValues == null)
                planValues = new Dictionary<T, W>();
            else
            {
                planValues.Clear();
            }
        }

        public virtual W GetPlanValue(T key)
        {
            return planValues[key];
        }

        public virtual bool HasPlanValue(T key)
        {
            return planValues.ContainsKey(key);
        }

        public virtual void SetPlanValue(T key, W value)
        {
            planValues[key] = value;
        }

        public virtual void RefreshMemory()
        {
            memory = GetComponent<IReGoapMemory<T, W>>();
        }

        public virtual void RefreshGoalsSet()
        {
            goals = new List<IReGoapGoal<T, W>>(GetComponents<IReGoapGoal<T, W>>());
            possibleGoalsDirty = true;
        }

        public virtual void RefreshActionsSet()
        {
            actions = new List<IReGoapAction<T, W>>(GetComponents<IReGoapAction<T, W>>());
        }

        public virtual List<IReGoapGoal<T, W>> GetGoalsSet()
        {
            if (possibleGoalsDirty)
                UpdatePossibleGoals();
            return possibleGoals;
        }

        public virtual List<IReGoapAction<T, W>> GetActionsSet()
        {
            return actions;
        }

        public virtual IReGoapMemory<T, W> GetMemory()
        {
            return memory;
        }

        public virtual IReGoapGoal<T, W> GetCurrentGoal()
        {
            return currentGoal;
        }

        public virtual ReGoapState<T, W> InstantiateNewState()
        {
            return ReGoapState<T, W>.Instantiate();
        }

        public override string ToString()
        {
            return string.Format("GoapAgent('{0}')", Name);
        }

        // this only works if the ReGoapAgent has been inherited. For "special cases" you have to override this
        public virtual Type[] GetGenericArguments()
        {
            return GetType().BaseType.GetGenericArguments();
        }
    }
}
using System.Collections.Generic;
using UnityEngine;
using Aspekt.AI.Planning;

namespace Aspekt.AI
{
    // TODO pause/unpause states when awaiting and receiving plan
    public class AIAgent : MonoBehaviour
    {
        public bool LoggingEnabled = true;
        public GameObject Owner;
        public GameObject GoalsObject;
        public GameObject ActionsObject;

        private AIGoal[] goals;
        private AIAction[] actions;

        private AIMemory memory;
        private AIPlanner planner;
        private AIExecutor executor;
        
        private enum States
        {
            Stopped, Active, Paused, FindNewGoal
        }
        private States state;

        private void Awake()
        {
            Owner = GetComponentInParent<TestUnit>().gameObject;
            memory = new AIMemory();
            planner = new AIPlanner(this);
            executor = new AIExecutor(this);

            goals = GoalsObject.GetComponents<AIGoal>();
            actions = ActionsObject.GetComponents<AIAction>();

            executor.OnFinishedPlan += FindNewGoal;
            planner.OnActionPlanFound += PlanFound;
        }

        private void OnDestroy()
        {
            executor.OnFinishedPlan -= FindNewGoal;
            planner.OnActionPlanFound -= PlanFound;
        }

        private void Update()
        {
            switch (state)
            {
                case States.Active:
                    executor.Tick(Time.deltaTime);
                    break;
                case States.Paused:
                    break;
                case States.Stopped:
                    break;
                case States.FindNewGoal:
                    planner.CalculateNewGoal();
                    break;
                default:
                    break;
            }
        }

        public void Activate()
        {
            if (state == States.Active) return;
            state = States.FindNewGoal;
        }

        public void Unpause()
        {
            if (state == States.Paused)
            {
                executor.Unpause();
                state = States.Active;
            }
        }

        public void Stop()
        {
            if (state == States.Stopped) return;
            executor.Stop();
            state = States.Stopped;
        }

        public void Pause()
        {
            if (state != States.Active) return;
            executor.Pause();
            state = States.Paused;
        }

        public AIAction[] GetActions()
        {
            return actions;
        }

        public AIGoal[] GetGoals()
        {
            return goals;
        }

        public AIMemory GetMemory()
        {
            return memory;
        }
        
        private void FindNewGoal()
        {
            state = States.FindNewGoal;
        }

        private void PlanFound()
        {
            if (state != States.FindNewGoal) return;
            state = States.Active;
            executor.ExecutePlan(planner.GetActionPlan(), planner.GetGoal());
        }
    }
}

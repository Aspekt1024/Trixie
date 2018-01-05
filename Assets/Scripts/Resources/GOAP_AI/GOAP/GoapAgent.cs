using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

using GoapLabels = GoapAction.GoapLabels;

public sealed class GoapAgent : MonoBehaviour {

    public bool DebugMode;

	private FSM stateMachine;

	private FSM.FSMState idleState; // finds something to do
	private FSM.FSMState moveToState; // moves to a target
	private FSM.FSMState performActionState; // performs an action
	
	private HashSet<GoapAction> availableActions;
	private Queue<GoapAction> currentActions;

	private IGoap dataProvider; // this is the implementing class that provides our world data and listens to feedback on planning

	private GoapPlanner planner;


	private void Start()
    {
		stateMachine = new FSM();
		availableActions = new HashSet<GoapAction>();
		currentActions = new Queue<GoapAction>();
		planner = new GoapPlanner();

		FindDataProvider();
		CreateIdleState();
		CreateMoveToState();
		CreatePerformActionState();

		stateMachine.pushState(idleState);
		LoadActions();
	}
	
	private void Update()
    {
		stateMachine.Update (gameObject);
	}
    
	public void AddAction(GoapAction a) {
		availableActions.Add (a);
	}

	public GoapAction GetAction(Type action) {
		foreach (GoapAction g in availableActions) {
			if (g.GetType().Equals(action) )
			    return g;
		}
		return null;
	}

	public void RemoveAction(GoapAction action) {
		availableActions.Remove (action);
	}

	private bool HasActionPlan() {
		return currentActions.Count > 0;
	}

	private void CreateIdleState()
    {
		idleState = (fsm, gameObj) => {

            // Get the world state and the goal we want to plan for
            Dictionary<GoapLabels, object> worldState = dataProvider.GetWorldState();
            Dictionary<GoapLabels, object> goals = dataProvider.CreateGoalState();

            if (goals.Count == 0) return;

			Queue<GoapAction> plan = planner.Plan(gameObject, availableActions, worldState, goals);
			if (plan != null)
            {
				// Plan Found
				currentActions = plan;
				dataProvider.PlanFound(goals, plan);

				fsm.popState(); // move to PerformAction state
				fsm.pushState(performActionState);

			}
            else
            {
				// No plan found, the agent will idle
                if (DebugMode)
                {
                    Debug.Log("<color=yellow>Failed Plan:</color> " + PrettyPrint(goals));
                }
				dataProvider.PlanFailed(goals);
				fsm.popState (); // move back to IdleAction state
				fsm.pushState (idleState);
			}

		};
	}
	
	private void CreateMoveToState() {
		moveToState = (fsm, gameObj) => {
			// move the game object

			GoapAction action = currentActions.Peek();
			if (action.RequiresInRange() && action.target == null) {
                if (DebugMode)
                {
                    Debug.Log("<color=red>Fatal error:</color> Action requires a target but has none. Planning failed. You did not assign the target in your Action.checkProceduralPrecondition()");
                }
				fsm.popState(); // move
				fsm.popState(); // perform
				fsm.pushState(idleState);
				return;
			}
            
			if ( dataProvider.MoveAgent(action) ) {
				fsm.popState();
			}
		};
	}
	
	private void CreatePerformActionState() {

		performActionState = (fsm, gameObj) => {
			// perform the action

			if (!HasActionPlan()) {
				// no actions to perform
                if (DebugMode)
                {
                    Debug.Log("<color=red>Done actions</color>");
                }
				fsm.popState();
				fsm.pushState(idleState);
				dataProvider.ActionsFinished();
				return;
			}

			GoapAction action = currentActions.Peek();
			if ( action.IsDone() ) {
				// the action is done. Remove it so we can perform the next one
				currentActions.Dequeue();
			}

			if (HasActionPlan()) {
				// perform the next action
				action = currentActions.Peek();
				bool inRange = action.RequiresInRange() ? action.IsInRange() : true;

				if ( inRange ) {
					// we are in range, so perform the action
					bool success = action.Perform(gameObj);

					if (!success) {
						// action failed, we need to plan again
						fsm.popState();
						fsm.pushState(idleState);
						dataProvider.PlanAborted(action);
					}
				} else {
					// we need to move there first
					// push moveTo state
					fsm.pushState(moveToState);
				}

			} else {
				// no actions left, move to Plan state
				fsm.popState();
				fsm.pushState(idleState);
				dataProvider.ActionsFinished();
			}

		};
	}

	private void FindDataProvider() {
		foreach (Component comp in gameObject.GetComponents(typeof(Component)))
        {
			if (typeof(IGoap).IsAssignableFrom(comp.GetType()))
            {
				dataProvider = (IGoap)comp;
				return;
			}
		}
	}

	private void LoadActions()
	{
		GoapAction[] actions = gameObject.GetComponents<GoapAction>();
		foreach (GoapAction a in actions) {
			availableActions.Add (a);
		}
        if (DebugMode)
        {
            Debug.Log("Found actions: " + PrettyPrint(actions));
        }
    }

	public static string PrettyPrint(Dictionary<GoapLabels, object> state) {
		String s = "";
		foreach (KeyValuePair<GoapLabels, object> kvp in state) {
			s += kvp.Key + ":" + kvp.Value.ToString();
			s += ", ";
		}
		return s;
	}

	public static string PrettyPrint(Queue<GoapAction> actions) {
		String s = "";
		foreach (GoapAction a in actions) {
			s += a.GetType().Name;
			s += "-> ";
		}
		s += "GOAL";
		return s;
	}

	public static string PrettyPrint(GoapAction[] actions) {
		String s = "";
		foreach (GoapAction a in actions) {
			s += a.GetType().Name;
			s += ", ";
		}
		return s;
	}

	public static string PrettyPrint(GoapAction action) {
		String s = ""+action.GetType().Name;
		return s;
	}
}

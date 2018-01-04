
using UnityEngine;
using System.Collections.Generic;

public abstract class GoapAction : MonoBehaviour {

    public enum GoapLabels
    {
        KillPlayer, Survive, RunAway, Wander, Patrol, Idle,
        CanSeePlayer, IsDying, CanAttack
    }

	private HashSet<KeyValuePair<GoapLabels,object>> preconditions;
	private HashSet<KeyValuePair<GoapLabels, object>> effects;

	private bool inRange = false;

	/* The cost of performing the action. 
	 * Figure out a weight that suits the action. 
	 * Changing it will affect what actions are chosen during planning.*/
	public float cost = 1f;

	/**
	 * An action often has to perform on an object. This is that object. Can be null. */
	public GameObject target;

	public GoapAction() {
		preconditions = new HashSet<KeyValuePair<GoapLabels, object>> ();
		effects = new HashSet<KeyValuePair<GoapLabels, object>> ();
	}

	public void doReset() {
		inRange = false;
		target = null;
		ResetAction ();
	}

	/**
	 * Reset any variables that need to be reset before planning happens again.
	 */
	public abstract void ResetAction();

	/**
	 * Is the action done?
	 */
	public abstract bool IsDone();

	/**
	 * Procedurally check if this action can run. Not all actions
	 * will need this, but some might.
	 */
	public abstract bool CheckProceduralPrecondition(GameObject agent);

	/**
	 * Run the action.
	 * Returns True if the action performed successfully or false
	 * if something happened and it can no longer perform. In this case
	 * the action queue should clear out and the goal cannot be reached.
	 */
	public abstract bool Perform(GameObject agent);

	/**
	 * Does this action need to be within range of a target game object?
	 * If not then the moveTo state will not need to run for this action.
	 */
	public abstract bool RequiresInRange ();
	

	/**
	 * Are we in range of the target?
	 * The MoveTo state will set this and it gets reset each time this action is performed.
	 */
	public bool isInRange () {
		return inRange;
	}
	
	public void setInRange(bool inRange) {
		this.inRange = inRange;
	}


	public void addPrecondition(GoapLabels key, object value) {
		preconditions.Add (new KeyValuePair<GoapLabels, object>(key, value) );
	}


	public void removePrecondition(GoapLabels key) {
		KeyValuePair<GoapLabels, object> remove = default(KeyValuePair<GoapLabels, object>);
		foreach (KeyValuePair<GoapLabels, object> kvp in preconditions) {
			if (kvp.Key.Equals (key)) 
				remove = kvp;
		}
		if ( !default(KeyValuePair<GoapLabels, object>).Equals(remove) )
			preconditions.Remove (remove);
	}


	public void addEffect(GoapLabels key, object value) {
		effects.Add (new KeyValuePair<GoapLabels, object>(key, value) );
	}


	public void removeEffect(GoapLabels key) {
		KeyValuePair<GoapLabels, object> remove = default(KeyValuePair<GoapLabels, object>);
		foreach (KeyValuePair<GoapLabels, object> kvp in effects) {
			if (kvp.Key.Equals (key)) 
				remove = kvp;
		}
		if ( !default(KeyValuePair<GoapLabels, object>).Equals(remove) )
			effects.Remove (remove);
	}

	
	public HashSet<KeyValuePair<GoapLabels, object>> Preconditions {
		get {
			return preconditions;
		}
	}

	public HashSet<KeyValuePair<GoapLabels, object>> Effects {
		get {
			return effects;
		}
	}
}
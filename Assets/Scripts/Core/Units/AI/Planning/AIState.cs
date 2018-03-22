using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aspekt.AI.Planning
{
    public class AIState
    {
        private Dictionary<string, object> state = new Dictionary<string, object>();
        private Dictionary<string, object> effects = new Dictionary<string, object>();
        private Dictionary<string, object> preconditions = new Dictionary<string, object>();

        public AIState()
        {
        }

        public AIState(AIGoal goal, Dictionary<string, object> newState)
        {
            preconditions = new Dictionary<string, object>(goal.GetConditions());
            state = newState;
        }

        public AIState(AIState oldState)
        {
            state = oldState.GetState();
            preconditions = new Dictionary<string, object>(oldState.GetPreconditions());
            effects = oldState.GetEffects();
        }

        public void AddEffect(KeyValuePair<string, object> effect)
        {
            AddEffect(effect.Key, effect.Value);
        }

        public void AddEffect(string label, object value)
        {
            if (!effects.ContainsKey(label))
            {
                effects.Add(label, value);
            }
        }

        public void ClearMetPreconditions(Dictionary<string, object> effects)
        {
            foreach (var effect in effects)
            {
                if (preconditions.ContainsKey(effect.Key) && preconditions[effect.Key].Equals(effect.Value))
                {
                    preconditions.Remove(effect.Key);
                }
            }
        }

        public void AddUnmetPreconditions(Dictionary<string, object> preconditionSet)
        {
            // Add preconditions not met by the world state
            foreach (var precondition in preconditionSet)
            {
                if (state.ContainsKey(precondition.Key) && state[precondition.Key].Equals(precondition.Value)) continue;
                AddPrecondition(precondition);
            }
        }

        public void AddPrecondition(KeyValuePair<string, object> precondition)
        {
            AddPrecondition(precondition.Key, precondition.Value);
        }

        public void AddPrecondition(string label, object value)
        {
            if (!preconditions.ContainsKey(label))
            {
                preconditions.Add(label, value);
            }
        }

        public Dictionary<string, object> GetState()
        {
            return state;
        }

        public Dictionary<string, object> GetPreconditions()
        {
            return preconditions;
        }

        public Dictionary<string, object> GetEffects()
        {
            return effects;
        }

        public AIState Clone()
        {
            return new AIState(this);
        }
    }
}

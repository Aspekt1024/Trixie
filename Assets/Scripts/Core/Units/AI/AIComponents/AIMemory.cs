using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aspekt.AI
{
    public class AIMemory
    {
        private Dictionary<string, object> state = new Dictionary<string, object>();

        public bool ConditionMet(object label, object value)
        {
            string key = label.ToString();
            if (state.ContainsKey(key))
            {
                return state[key].Equals(value);
            }
            else if (value.GetType().Equals(typeof(bool)))
            {
                return value.Equals(false);
            }
            else
            {
                return false;
            }
        }

        public void Set(object label, object newValue)
        {
            string key = label.ToString();
            if (state.ContainsKey(key))
            {
                state[key] = newValue;
            }
            else
            {
                state.Add(key, newValue);
            }
        }

        public Dictionary<string, object> GetState()
        {
            return state;
        }

        public Dictionary<string, object> CloneState()
        {
            return new Dictionary<string, object>(state);
        }
    }
}

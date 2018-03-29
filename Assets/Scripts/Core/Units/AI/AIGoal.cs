using System.Collections.Generic;
using UnityEngine;

namespace Aspekt.AI
{
    public abstract class AIGoal : MonoBehaviour
    {
        public float Priority = 1f;

        private Dictionary<string, object> conditions = new Dictionary<string, object>();
        
        private void Awake()
        {
            SetConditions();
        }

        /// <summary>
        /// Called once the goal is complete. Use for (re)setting states before finding a new goal.
        /// </summary>
        public virtual void ExitGoal(AIAgent agent) { }
        
        public Dictionary<string, object> GetConditions()
        {
            return conditions;
        }

        public override string ToString()
        {
            return GetType().ToString();
        }

        protected virtual void SetConditions() { }

        protected void AddCondition(object label, object value)
        {
            conditions.Add(label.ToString(), value);
        }

    }
}

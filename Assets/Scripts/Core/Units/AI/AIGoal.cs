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
        
        public Dictionary<string, object> GetConditions()
        {
            return conditions;
        }

        public override string ToString()
        {
            return GetType().ToString();
        }

        protected virtual void SetConditions() { }

        protected void AddCondition(string label, object value)
        {
            conditions.Add(label, value);
        }

    }
}

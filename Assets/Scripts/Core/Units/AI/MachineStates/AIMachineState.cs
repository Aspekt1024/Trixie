using System;

namespace Aspekt.AI
{
    public class AIMachineState : IAIMachineState
    {
        protected AIAgent agent;

        public void SetParentAgent(AIAgent agent)
        {
            this.agent = agent;
        }

        public virtual void Tick(float deltaTime)
        {

        }

        public event Action OnComplete = delegate { };
        
        public virtual void Enter() { }
        public virtual void Pause() { }
        public virtual void Stop() { }

        public override string ToString()
        {
            return GetType().ToString();
        }
        
        protected void StateComplete()
        {
            if (OnComplete != null)
            {
                OnComplete();
            }
        }
    }
}


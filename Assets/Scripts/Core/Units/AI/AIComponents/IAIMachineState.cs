using System;

namespace Aspekt.AI
{
    interface IAIMachineState
    {
        void Enter();
        void Pause();
        void Stop();
        event Action OnComplete;
    }
}

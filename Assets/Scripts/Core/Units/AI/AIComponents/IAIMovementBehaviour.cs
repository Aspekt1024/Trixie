using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aspekt.AI
{
    public interface IAIMovementBehaviour
    {
        void Run(Transform target);
        void Stop();
        void Tick(float deltaTime);
        bool TargetReached();
        void UpdateTarget(Transform target);
        void SetTargetReached();
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aspekt.AI
{
    public interface IAIMovementBehaviour
    {
        void Start();
        void Stop();
        void Tick(float deltaTime);
        void SetTarget(Transform target);
        bool TargetReached();
    }
}


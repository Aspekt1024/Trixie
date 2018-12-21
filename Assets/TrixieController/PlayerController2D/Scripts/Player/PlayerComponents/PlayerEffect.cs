using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aspekt.PlayerController
{
    public abstract class PlayerEffect : MonoBehaviour
    {
        public abstract void Play();
        public abstract void Stop();
    }
}

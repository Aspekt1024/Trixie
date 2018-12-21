using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Aspekt.PlayerController
{
    public enum AnimationFloats
    {
        MoveSpeed,
        LookAngle,
    }

    public enum AnimationBools
    {
        IsRunning,
        Falling,
        Grounded,
        Jetpacking,
        ShieldEnabled,
    }
    
    public enum AnimationNames
    {
        Jump,
    }

    public class AnimationHandler
    {
        Animator animator;

        public AnimationHandler(Animator animator)
        {
            this.animator = animator;
        }

        internal void SetFloat(AnimationFloats name, float value)
        {
            animator?.SetFloat(name.ToString(), value);
        }

        internal void SetBool(AnimationBools name, bool value)
        {
            animator?.SetBool(name.ToString(), value);
        }

        internal void Play(AnimationNames name)
        {
            animator?.Play(name.ToString(), 0, 0f);
        }
    }
}

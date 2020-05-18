using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spine.Unity;
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
        SkeletonAnimation skeletonAnimation;

        public AnimationHandler(SkeletonAnimation animator)
        {
            this.skeletonAnimation = animator;
        }

        public void SetAnimation (string animation, int track=0)
        {
            if (skeletonAnimation.AnimationName == animation) return;
            skeletonAnimation.AnimationState.SetAnimation(track, animation, false);
        }

        public void PlayAnimationOneShot(string animation, int track=0)
        {
            skeletonAnimation.AnimationState.SetAnimation(track, animation, true);
        }

        public void SetFacing(float direction)
        {
            skeletonAnimation.Skeleton.ScaleX = direction;
        }
    }
}

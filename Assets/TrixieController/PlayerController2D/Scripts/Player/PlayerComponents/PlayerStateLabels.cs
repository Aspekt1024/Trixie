using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aspekt.PlayerController
{
    public enum StateLabels
    {
        IsKnockedBack,

        IsGrounded,
        IsTouchingCeiling,
        IsAgainstWall,
        IsAtEdge,
        IsAttachedToWall,
        IsOnSlope,
        IsJumping,
        IsStomping,
        IsBoosting,
        IsOnMovingPlatform,

        IsInGravityField,
        IsStunned,

        SlopeGradient,
        FieldStrength,
        TerrainBounciness,
        PlatformVelocity,

        IsAlive,
    }
}

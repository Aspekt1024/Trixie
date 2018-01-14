using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrixieCore
{ 
    public static class Maths
    {
        public static float CalculateThrowingAngle(Vector3 startPos, Vector3 targetPos, bool upperPath, float s, float gravityScale)
        {
            // Source: https://en.wikipedia.org/wiki/Trajectory_of_a_projectile#Angle_required_to_hit_coordinate_.28x.2Cy.29
            float g = -Physics2D.gravity.y * gravityScale;
            float x = startPos.x - targetPos.x;
            float y = targetPos.y - startPos.y;

            bool backwards = x < 0;
            if (backwards)
            {
                x = -x;
            }

            float angle;
            if (upperPath)
                angle = Mathf.Atan((s * s + Mathf.Sqrt(Mathf.Pow(s, 4) - g * (g * x * x + 2 * y * s * s))) / (g * x));
            else
                angle = Mathf.Atan((s * s - Mathf.Sqrt(Mathf.Pow(s, 4) - g * (g * x * x + 2 * y * s * s))) / (g * x));

            if (float.IsNaN(angle)) angle = 0;

            angle *= Mathf.Rad2Deg;

            return backwards ? angle : 180f - angle;
        }

        public static Vector2 SpeedToVelocity(float speed, float angle)
        {
            Vector2 velocity = new Vector2()
            {
                x = speed * Mathf.Cos(angle * Mathf.Deg2Rad),
                y = speed * Mathf.Sin(angle * Mathf.Deg2Rad)
            };
            return velocity;
        }
    }
}

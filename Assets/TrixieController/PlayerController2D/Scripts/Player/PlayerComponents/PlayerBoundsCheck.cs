using UnityEngine;

namespace Aspekt.PlayerController
{
    public class PlayerBoundsCheck
    {
        private Transform playerTransform;
        private Rigidbody2D playerBody;
        private Collider2D playerCollider;

        public PlayerBoundsCheck(Transform playerTransform, Rigidbody2D playerBody, Collider2D playerCollider)
        {
            this.playerTransform = playerTransform;
            this.playerBody = playerBody;
            this.playerCollider = playerCollider;
        }

        public void CheckBounds(float deltaTime)
        {
            RaycastHit2D hit = Physics2D.Raycast(
                playerTransform.position,
                playerBody.velocity,
                playerBody.velocity.magnitude * Time.fixedDeltaTime * 2f,
                1 << LayerMask.NameToLayer("Terrain"));

            if (hit.collider == null) return;

            bool positionCorrected = false;
            float xVelocity = playerBody.velocity.x;
            float yVelocity = playerBody.velocity.y;
            float xPos = playerTransform.position.x;
            float yPos = playerTransform.position.y;

            if (Mathf.Abs(hit.point.y - playerTransform.position.y) > playerCollider.bounds.extents.y)
            {
                yPos = hit.point.y - Mathf.Sign(yVelocity) * playerCollider.bounds.extents.y;
                positionCorrected = true;
                yVelocity = 0f;
            }

            if (Mathf.Abs(hit.point.x - playerTransform.position.x) > playerCollider.bounds.extents.x)
            {
                xPos = hit.point.x - Mathf.Sign(xVelocity) * playerCollider.bounds.extents.x;
                positionCorrected = true;
                xVelocity = 0f;
            }

            if (positionCorrected)
            {
                playerTransform.position = new Vector2(xPos, yPos);
                playerBody.velocity = new Vector2(xVelocity, yVelocity);
            }
        }
    }
}

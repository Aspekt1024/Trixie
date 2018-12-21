using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrixieCore
{
    public class CameraFollow : MonoBehaviour
    {
        public enum FollowTypes
        {
            None, PlayerOnly, PlayerAndAggro
        }
        public FollowTypes FollowType = FollowTypes.PlayerAndAggro;
        public float FollowSpeed = 5f;
        public float TargetRange = 20f;
        public float Strength = 1f;

        private float xMin = -10000f;
        private float xMax = 10000f;
        private float yMin = 0f;
        private float yMax = 10000f;

        private float xOffset = 0f;
        private float yOffset = 0f;

        private bool focusOverrideMode;

        private Transform playerTf;
        private Rigidbody2D playerBody;
        private HashSet<Transform> objectsToFollow;
        private Vector2 focusPoint;

        private void Start()
        {
            if (Trixie.Instance == null) return;

            playerTf = Trixie.Instance.transform;
            playerBody = playerTf.GetComponent<Rigidbody2D>();
            objectsToFollow = new HashSet<Transform>();
        }

        private void LateUpdate()
        {
            if (FollowType == FollowTypes.None) return;

            if (focusOverrideMode)
            {
                GotoFocusPoint();
            }
            else if (FollowType == FollowTypes.PlayerOnly || objectsToFollow == null || objectsToFollow.Count == 0)
            {
                FollowPlayer();
            }
            else
            {
                FocusAllBodies();
            }
        }

        public void SetCameraFocus(Transform focusTf)
        {
            focusOverrideMode = true;
            focusPoint = focusTf.position;
        }

        public void SetCameraFollow(Transform newObjectToFollow = null)
        {
            if (newObjectToFollow)
            {
                playerTf = newObjectToFollow;
                playerBody = playerTf.GetComponent<Rigidbody2D>();
            }
            focusOverrideMode = false;
        }

        public void AddObjectToFollow(Transform newObject)
        {
            objectsToFollow.Add(newObject);
        }

        public void StopFollowingObject(Transform objectToRemove)
        {
            objectsToFollow.Remove(objectToRemove);
        }

        private void GotoFocusPoint()
        {
            Vector2 newPos = Vector2.Lerp(transform.position, focusPoint, 2 * Time.deltaTime);
            transform.position = new Vector3(newPos.x, newPos.y, transform.position.z);
        }

        private void FollowPlayer()
        {
            if (playerBody != null)
            {
                xOffset = Mathf.Lerp(xOffset, playerBody.velocity.x, Time.deltaTime * FollowSpeed);
                yOffset = Mathf.Lerp(yOffset, playerBody.velocity.y * 0.75f, Time.deltaTime * FollowSpeed);
            }

            Vector2 targetPos = new Vector2()
            {
                x = Mathf.Clamp(playerTf.position.x + xOffset, xMin, xMax),
                y = Mathf.Clamp(playerTf.position.y + yOffset, yMin, yMax)
            };

            LerpCameraPosition(targetPos);
        }

        private void FocusAllBodies()
        {
            Vector2 centerPoint = GetCenterPoint();
            Vector3 newPosition = centerPoint + new Vector2(xOffset, yOffset);

            LerpCameraPosition(centerPoint);
        }

        private Vector2 GetCenterPoint()
        {
            var bounds = new Bounds(playerTf.position, Vector3.zero);

            foreach (Transform tf in objectsToFollow)
            {
                if (Vector2.Distance(tf.position, playerTf.position) <= TargetRange)
                {
                    bounds.Encapsulate(tf.position);
                }
            }

            return Vector2.Lerp(playerTf.position, bounds.center, Strength);
        }

        private void LerpCameraPosition(Vector2 targetPos)
        {
            float cameraSpeed = 2f;

            Vector2 screenSize = GetScreenSizeInUnits();

            Vector2 maxDistFromCenter = new Vector2(screenSize.x * 0.4f, screenSize.y * 0.4f);
            Vector2 triggerDistance = new Vector2(screenSize.x * 0.35f, screenSize.y * 0.3f);
            Vector2 playerDistFromCenter = playerTf.position - Camera.main.ScreenToWorldPoint(new Vector2(Screen.width / 2f, Screen.height / 2f));
            playerDistFromCenter = new Vector2(Mathf.Abs(playerDistFromCenter.x), Mathf.Abs(playerDistFromCenter.y));

            // Favour the player
            if (playerDistFromCenter.x > triggerDistance.x)
            {
                targetPos.x = Mathf.Lerp(targetPos.x, Trixie.Instance.transform.position.x, (playerDistFromCenter.x - triggerDistance.x) / (maxDistFromCenter.x - triggerDistance.x));
            }

            if (playerDistFromCenter.y > triggerDistance.y)
            {
                targetPos.y = Mathf.Lerp(targetPos.y, Trixie.Instance.transform.position.y, (playerDistFromCenter.y - triggerDistance.y) / (maxDistFromCenter.y - triggerDistance.y));
            }

            Vector2 newPos = Vector2.Lerp(transform.position, targetPos, cameraSpeed * Time.deltaTime);
            transform.position = new Vector3(newPos.x, newPos.y, transform.position.z);
        }

        private Vector2 GetScreenSizeInUnits()
        {
            return Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height)) - Camera.main.ScreenToWorldPoint(Vector2.zero);
        }
    }
}

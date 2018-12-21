using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrixieCore.Units
{
    public class VisionComponent : UnitAbility
    {

        public float StartingAngle = 0f;
        public float Arc = 75f;
        public float Radius = 10f;
        public float CheckFrequency = 5f;
        public float VisionMemory = 2f;

        private bool canSeePlayer;
        private LayerMask visibleLayers;
        private bool hasSeenPlayer;
        private float timeLastSeenPlayer;
        private Vector3 lastKnownPlayerPosition;

        private BaseUnit unit;

        private enum States
        {
            None, Active
        }
        private States state;

        private void Start()
        {
            unit = GetComponentInParent<BaseUnit>();
            visibleLayers = 1 << LayerMask.NameToLayer("Terrain") | 1 << LayerMask.NameToLayer("Player");
        }

        public void Activate()
        {
            if (state == States.Active) return;

            state = States.Active;
            InvokeRepeating("CheckForPlayer", 0.5f, 1f / CheckFrequency);
        }

        public void Deactivate()
        {
            state = States.None;
            CancelInvoke();
        }

        public bool HasSeenPlayerRecenty()
        {
            bool seenRecently = false;
            if (canSeePlayer)
            {
                seenRecently = true;
            }
            else if (!hasSeenPlayer)
            {
                seenRecently = false;
            }
            else if (timeLastSeenPlayer + VisionMemory > Time.time)
            {
                seenRecently = true;
            }
            else
            {
                seenRecently = false;
            }

            return seenRecently;
        }

        public Vector2 GetLastKnownPlayerPosition() { return lastKnownPlayerPosition; }
        public bool CanSeePlayer() { return canSeePlayer; }

        private void CheckForPlayer()
        {
            Vector2 distVector = (Trixie.Instance.transform.position - transform.position);
            if (!IsWithinDistance(distVector) || !IsWithinArc(distVector))
            {
                SetPlayerUnseen();
                canSeePlayer = false;
                return;
            }

            RaycastHit2D hit = Physics2D.Raycast(transform.position, distVector, Radius, visibleLayers);
            if (hit.collider == null)
            {
                canSeePlayer = false;
            }
            else
            {
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Terrain"))
                {
                    canSeePlayer = false;
                }
                else
                {
                    timeLastSeenPlayer = Time.time;
                    hasSeenPlayer = true;
                    canSeePlayer = true;
                    lastKnownPlayerPosition = Trixie.Instance.transform.position;
                }
            }
        }

        private void SetPlayerUnseen()
        {
            if (canSeePlayer)
            {
                timeLastSeenPlayer = Time.time;
            }
            canSeePlayer = false;
        }

        private bool IsWithinDistance(Vector2 distVector)
        {
            return distVector.magnitude < Radius;
        }

        private bool IsWithinArc(Vector2 distVector)
        {
            // TODO unit will never be null when we remove BaseEnemy
            if (unit != null)
            {
                if (unit.DirectionFlipped)
                {
                    distVector.x *= -1;
                }
            }

            float angle = Mathf.Atan2(distVector.y, distVector.x) * Mathf.Rad2Deg;
            float minAngle = StartingAngle;
            float maxAngle = StartingAngle + Arc;


            if (maxAngle > 180f)
            {
                maxAngle -= 360f;
                return (angle > minAngle && angle < 180f) || (angle > -180f && angle < maxAngle);
            }
            else
            {
                return (angle > minAngle && angle < maxAngle);
            }
        }
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

namespace TrixieCore.Units
{
    [RequireComponent(typeof(Seeker))]
    public class UnitPathfinder : MonoBehaviour
    {
        public float UpdateFrequency = 2f;
        public float nextWaypointDistance = 2f;

        [HideInInspector] public Path Path;

        private Seeker seeker;
        private int currentWaypoint = 0;
        private Transform target;
        private bool directionChangedSinceLastCheck;

        private bool currentlySeeking;
        
        public void Activate(Transform tf)
        {
            Activate(tf.position);
        }

        public void Activate(Vector3 pos)
        {
            target.position = pos;
            if (currentlySeeking) return;

            currentlySeeking = true;
            InvokeRepeating(((System.Action)UpdatePath).Method.Name, 0f, 1f / UpdateFrequency);
        }

        public Vector3 GetTargetPosition()
        {
            return target.position;
        }

        public bool HasFinishedPathing()
        {
            return Path == null;
        }

        public void Stop()
        {
            CancelPath();
        }

        public void CancelPath()
        {
            Path = null;
            currentlySeeking = false;
            CancelInvoke();
        }

        public Vector3 GetDirection()
        {
            if (Path == null || Path.vectorPath.Count - 1 < currentWaypoint) return Vector3.zero;
            return (Path.vectorPath[currentWaypoint] - transform.position).normalized;
        }

        public bool DirectionChanged()
        {
            if (directionChangedSinceLastCheck)
            {
                directionChangedSinceLastCheck = false;
                return true;
            }
            return false;
        }

        private void Start()
        {
            seeker = GetComponent<Seeker>();
            target = new GameObject("MovementTf-" + name).transform;
            target.SetParent(PathfinderHandler.GetPathfinderWaypointsTransform());
        }

        private void FixedUpdate()
        {
            if (Path == null) return;
            
            if (TargetReached() || currentWaypoint == Path.vectorPath.Count)
            {
                CancelPath();
                return;
            }

            if (Vector3.Distance(transform.position, Path.vectorPath[currentWaypoint]) < nextWaypointDistance)
            {
                currentWaypoint++;
                directionChangedSinceLastCheck = true;
                return;
            }
        }

        private bool TargetReached()
        {
            float dist = Vector2.Distance(target.position, transform.position);
            return dist < nextWaypointDistance;
        }

        private void UpdatePath()
        {
            seeker.StartPath(transform.position, target.position, OnPathComplete);
        }

        private void OnPathComplete(Path p)
        {
            if (!p.error)
            {
                Path = p;
                currentWaypoint = 0;
            }
        }
    }
}

﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using TrixieCore.Units;

namespace TrixieCore
{
    [RequireComponent(typeof(VisionComponent))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Seeker))]
    public class EnemyPathfinder : MonoBehaviour
    {

        public float UpdateFrequency = 2f;
        public float Speed = 300f;
        public ForceMode2D ForceMode;

        public float nextWaypointDistance = 3f;
        public float MinDistToTarget = 5f;
        public float MaxDistToTarget = 18f;

        [HideInInspector] public Path Path;

        private Seeker seeker;
        private Rigidbody2D body;
        private VisionComponent vision;
        private int currentWaypoint = 0;
        private Transform target;

        private bool currentlySeeking;

        private enum States
        {
            Idle, Moving
        }
        private States state;

        private void Start()
        {
            seeker = GetComponent<Seeker>();
            body = GetComponent<Rigidbody2D>();
            vision = GetComponent<VisionComponent>();

            target = new GameObject("MovementTf-" + name).transform;
        }

        public void Activate(Transform tf)
        {
            Activate(tf.position);
        }

        public void Activate(Vector3 pos)
        {
            state = States.Moving;

            target.position = pos;
            if (currentlySeeking) return;

            currentlySeeking = true;
            InvokeRepeating(GetMethodName(UpdatePath), 0f, 1f / UpdateFrequency);
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
            state = States.Idle;
            Path = null;
            currentlySeeking = false;
            CancelInvoke();
        }

        private void FixedUpdate()
        {
            if (state == States.Idle)
            {
                body.velocity = Vector2.Lerp(body.velocity, Vector2.zero, Time.fixedDeltaTime);
                return;
            }

            if (Path == null) return;

            if (InSweetSpot())
            {
                // TODO: Randomise path
                CancelPath();
                return;
            }

            if (currentWaypoint == Path.vectorPath.Count)
            {
                CancelPath();
                return;
            }

            Vector3 dir = (Path.vectorPath[currentWaypoint] - transform.position).normalized;
            dir *= Speed * Time.fixedDeltaTime;

            body.AddForce(dir, ForceMode);

            if (Vector3.Distance(transform.position, Path.vectorPath[currentWaypoint]) < nextWaypointDistance)
            {
                currentWaypoint++;
                return;
            }
        }

        private bool InSweetSpot()
        {
            if (!vision.CanSeePlayer()) return false;

            float dist = Vector2.Distance(target.position, transform.position);
            return dist > MinDistToTarget && dist < MaxDistToTarget;
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

        private string GetMethodName(Action method)
        {
            return method.Method.Name;
        }
    }
}



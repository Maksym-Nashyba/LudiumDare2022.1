﻿using System.Collections;
using Interactables;
using Misc;
using NPCs.Navigation;
using Player;
using UnityEngine;

namespace NPCs
{
    public abstract class LivingNPC : NPC, IInteractable
    {
        [SerializeField] public Transform ParasiteTargetPoint;
        protected SuspiciousObjectsDetector Detector;

        protected override void Awake()
        {
            base.Awake();
            Detector = GetComponentInChildren<SuspiciousObjectsDetector>();
        }

        private void Start()
        {
            ChangeState(new IdleState());
        }

        public virtual void WalkToPosition(Vector3 target)
        {
            NavMeshAgent.SetDestination(target);
        }

        public virtual void RunToPosition(Vector3 target)
        {
            NavMeshAgent.SetDestination(target);
        }

        public virtual void Die(DeathCauses deathCause)
        {
            Destroy(gameObject);
        }

        public void GetInfected()
        {
            StopAllCoroutines();
        }

        public enum DeathCauses
        {
            Default,
            Shot,
            ParasiteLeaving
        }

        protected override Context GetContext()
        {
            return new Context(WaypointsContainer, NavMeshAgent, Transform, this);
        }

        protected class IdleState : State
        {
            private Context _context;
            private Waypoint _idlingWaypoint;
            private Vector3 _idlingPosition;

            public override void Start(Context context)
            {
                _context = context;
                _idlingWaypoint = context.WaypointsContainer.GetClosestWaypoint(context.Transform.position);
            }

            public override IEnumerator Act()
            {
                LivingNPC npc = (LivingNPC)_context.NPC;
                while (true)
                {
                    _idlingPosition =
                        Utils.GetPointInRadiusFlat(_idlingWaypoint.Transform.position, _idlingWaypoint.Radius);
                    npc.WalkToPosition(_idlingPosition);
                    yield return new WaitForSeconds(Random.Range(2f, 8f));
                }
            }

            public override void End()
            {
            }

            public override bool IsRelaxed()
            {
                return true;
            }
        }

        protected class RunForLifeState : State
        {
            private Transform _threatTransform;
            private Waypoint _target;
            private Context _context;
            private float _targetDistanceToTarget;
            private LivingNPC _npc;

            public RunForLifeState(Transform threatTransform)
            {
                _threatTransform = threatTransform;
            }

            public override void Start(Context context)
            {
                _context = context;
                _target = ServiceLocator.WaypointsContainer
                    .GetBestWaypointForEscape(context.Transform.position, _threatTransform.position);
                _targetDistanceToTarget = _target.Radius / 2f;
                _npc = (LivingNPC)context.NPC;
                _npc.RunToPosition(_target.Transform.position);
            }

            public override IEnumerator Act()
            {
                bool tooFar = true;
                while (tooFar)
                {
                    float distanceSqr = (_target.Transform.position - _context.Transform.position).sqrMagnitude;
                    tooFar = distanceSqr > _targetDistanceToTarget * _targetDistanceToTarget;
                    yield return null;
                }
                _npc.ChangeState(new IdleState());
            }

            public override void End()
            {
            }

            public override bool IsRelaxed()
            {
                return false;
            }
        }

        public void AcceptVisitor(IVisitor visitor)
        {
            visitor.Interact(this);
        }
    }
}
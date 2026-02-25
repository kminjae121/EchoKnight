using System;
using System.Collections;
using Code.Core.Interfaces;
using Code.UnitSystem;
using UnityEngine;
using UnityEngine.AI;
using Unit = Code.UnitSystem.Unit;

namespace EnemySystem
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class EnemyGridMovingSystem : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float _tileSize = 3.18f; 

        [Header("References")]
        [SerializeField] private UnitRotation _rotationCompo;
        
        private NavMeshAgent _agent;
        private Transform _rootTransform;
        private Animator _animator;
        
        public void Initialize(Unit owner)
        {
            _rootTransform = owner.transform;
            _agent = GetComponent<NavMeshAgent>();
            _animator = owner.GetComponentInChildren<Animator>();
            
            if (_rotationCompo == null) 
                _rotationCompo = owner.GetComponent<UnitRotation>();

            if (_agent != null)
            {
                _agent.updateRotation = false;
                _agent.updateUpAxis = false;
            }
        }

        public void MoveTo(Vector3 targetPos, int maxSteps, Action onComplete)
        {
            float maxDistance = maxSteps * _tileSize;
            StartCoroutine(NavMeshMoveRoutine(targetPos, maxDistance, false, onComplete));
        }

        public void RetreatFromTarget(Vector3 targetPos, int steps, Action onComplete)
        {
            float retreatDistance = steps * _tileSize;
            Vector3 dirAwayFromTarget = (_rootTransform.position - targetPos).normalized;
            Vector3 idealRetreatPos = _rootTransform.position + (dirAwayFromTarget * retreatDistance);

            if (NavMesh.SamplePosition(idealRetreatPos, out NavMeshHit hit, _tileSize * 2f, NavMesh.AllAreas))
            {
                StartCoroutine(NavMeshMoveRoutine(hit.position, retreatDistance, true, onComplete));
            }
            else
            {
                Debug.Log("[후퇴 중단] 맵 끝이거나 막혀있습니다.");
                onComplete?.Invoke();
            }
        }

        private IEnumerator NavMeshMoveRoutine(Vector3 destination, float maxDistance, bool isRetreating, Action onComplete)
        {
            if (_agent == null || !_agent.isOnNavMesh)
            {
                onComplete?.Invoke();
                yield break;
            }

            Vector3 constrainedDest = GetConstrainedDestination(_rootTransform.position, destination, maxDistance);

            _agent.SetDestination(constrainedDest);

            if (_animator != null && isRetreating) _animator.SetBool("RETREAT", true);

            while (_agent.pathPending || _agent.remainingDistance > 0.1f)
            {
                if (!isRetreating && _rotationCompo != null && _agent.velocity.sqrMagnitude > 0.1f)
                {
                    _rotationCompo.SetDir(_rootTransform.position + _agent.velocity);
                }
                yield return null;
            }

            if (_animator != null && isRetreating) _animator.SetBool("RETREAT", false);
            
            onComplete?.Invoke();
        }

        private Vector3 GetConstrainedDestination(Vector3 start, Vector3 target, float maxDistance)
        {
            NavMeshPath path = new NavMeshPath();
            NavMesh.CalculatePath(start, target, NavMesh.AllAreas, path);
            
            if (path.corners.Length == 0) return start;

            float traveledDist = 0;
            Vector3 currentPos = path.corners[0];

            for (int i = 1; i < path.corners.Length; i++)
            {
                float distToNextCorner = Vector3.Distance(currentPos, path.corners[i]);
                if (traveledDist + distToNextCorner > maxDistance)
                {
                    float remainingAllowableDist = maxDistance - traveledDist;
                    return currentPos + (path.corners[i] - currentPos).normalized * remainingAllowableDist;
                }
                traveledDist += distToNextCorner;
                currentPos = path.corners[i];
            }
            return currentPos;
        }
    }
}
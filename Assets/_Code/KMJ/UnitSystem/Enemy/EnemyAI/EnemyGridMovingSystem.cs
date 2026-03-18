using System;
using System.Collections;
using Code.Core.Interfaces;
using Code.Map;
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
        [SerializeField] private float _defaultSpeed = 5f;
        [SerializeField] private LayerMask _whatIsGround; 

        [Header("References")]
        [SerializeField] private UnitRotation _rotationCompo;
        
        private NavMeshAgent _agent;
        //private NavMeshObstacle _obstacle; 
        private Transform _rootTransform;
        private Animator _animator;
        private GameObject _currentTileObj; 
        
        private void Awake()
        {
            if (_rootTransform == null) _rootTransform = transform;
            _agent = GetComponent<NavMeshAgent>();
            
            //_obstacle = GetComponent<NavMeshObstacle>();
            //if (_obstacle == null)
            //{
            //    _obstacle = gameObject.AddComponent<NavMeshObstacle>();
            //}
            //_obstacle.carving = true; 
            //_obstacle.shape = NavMeshObstacleShape.Box;
            //_obstacle.size = new Vector3(_tileSize * 0.8f, 2f, _tileSize * 0.8f);
        }

        public void Initialize(Unit owner)
        {
            _rootTransform = owner.transform;
            _animator = owner.GetComponentInChildren<Animator>();
            
            if (_rotationCompo == null) 
                _rotationCompo = owner.GetComponent<UnitRotation>();

            if (_agent != null)
            {
                _agent.updateRotation = false; 
                _agent.updateUpAxis = false;
                _agent.speed = _defaultSpeed;
            }

            SetMovementState(false);
            SnapToTileCenterAndRegister();
        }

        private void SetMovementState(bool isMoving)
        {
            if (isMoving)
            {
                //if (_obstacle != null) _obstacle.enabled = false;
                if (_agent != null) _agent.enabled = true;
            }
            else
            {
                if (_agent != null) _agent.enabled = false;
               // if (_obstacle != null) _obstacle.enabled = true;
            }
        }

        public Vector3 GetExactTileCenter(Vector3 pos)
        {
            if (GridMap.Instance != null)
            {
                Vector2Int gridPos = GridMap.Instance.WorldToGridPosition(pos);
                if (GridMap.Instance.IsValidPosition(gridPos))
                {
                    Vector3 exactPos = GridMap.Instance.GridToWorldPosition(gridPos.x, gridPos.y);
                    return new Vector3(exactPos.x, pos.y, exactPos.z);
                }
            }
            
            if (Physics.Raycast(pos + Vector3.up * 5f, Vector3.down, out RaycastHit hit, 10f, _whatIsGround))
            {
                if (hit.transform.TryGetComponent(out IMapTile tile))
                {
                    return new Vector3(hit.transform.position.x, pos.y, hit.transform.position.z);
                }
            }
            return pos;
        }

        private void SnapToTileCenterAndRegister()
        {
            Vector3 centerPos = GetExactTileCenter(_rootTransform.position);
            _rootTransform.position = centerPos;
            UpdateCurrentTile(centerPos);
        }

        #region [Move & Retreat]

        public void MoveTo(Vector3 targetPos, int maxSteps, Action onComplete)
        {
            float maxDistance = maxSteps * _tileSize;
            if (_agent != null) _agent.speed = _defaultSpeed;
            
            StartCoroutine(NavMeshMoveRoutine(targetPos, maxDistance, false, onComplete));
        }

        public void RetreatFromTarget(Vector3 targetPos, int steps, Action onComplete)
        {
            float retreatDistance = steps * _tileSize;
            Vector3 dirAway = (_rootTransform.position - targetPos).normalized;
            Vector3 idealPos = _rootTransform.position + (dirAway * retreatDistance);

            if (NavMesh.SamplePosition(idealPos, out NavMeshHit hit, _tileSize * 2f, NavMesh.AllAreas))
            {
                if (_agent != null) _agent.speed = _defaultSpeed; 
                
                StartCoroutine(NavMeshMoveRoutine(hit.position, retreatDistance, true, onComplete));
            }
            else
            {
                onComplete?.Invoke();
            }
        }

        #endregion

        #region [Core Logic]

        private IEnumerator NavMeshMoveRoutine(Vector3 targetDest, float maxDistance, bool isRetreating, Action onComplete)
        {
            SetMovementState(true);

            yield return null; 
            yield return null; 

            if (_agent == null || !_agent.isOnNavMesh)
            {
                Debug.LogWarning($"[{gameObject.name}] NavMeshAgent가 맵 위에 없습니다! 베이크 상태를 확인하세요.");
                SetMovementState(false);
                onComplete?.Invoke();
                yield break;
            }

            Vector3 finalDest = GetConstrainedDestination(_rootTransform.position, targetDest, maxDistance);
            
            if (Vector3.Distance(_rootTransform.position, finalDest) < 0.1f)
            {
                SetMovementState(false);
                onComplete?.Invoke();
                yield break;
            }

            _agent.SetDestination(finalDest);
            _agent.isStopped = false;

            float timeout = 2f;
            while (_agent.pathPending && timeout > 0f)
            {
                timeout -= Time.deltaTime;
                yield return null;
            }

            if (_agent.pathStatus == NavMeshPathStatus.PathInvalid)
            {
                SetMovementState(false);
                onComplete?.Invoke();
                yield break;
            }

            while (!_agent.pathPending && _agent.remainingDistance > 0.1f)
            {
                if (!isRetreating && _rotationCompo != null && _agent.velocity.sqrMagnitude > 0.1f)
                {
                    Vector3 lookPos = _rootTransform.position + _agent.velocity;
                    _rotationCompo.SetDir(lookPos);
                }
                yield return null;
            }

            _agent.isStopped = true;
            SetMovementState(false);
            
            SnapToTileCenterAndRegister();
            
            onComplete?.Invoke();
        }

        private Vector3 GetConstrainedDestination(Vector3 startPos, Vector3 targetPos, float maxDistance)
        {
            NavMeshPath path = new NavMeshPath();
            NavMesh.CalculatePath(startPos, targetPos, NavMesh.AllAreas, path);

            if (path.status != NavMeshPathStatus.PathComplete && path.status != NavMeshPathStatus.PathPartial)
                return startPos;

            float currentDist = 0f;
            Vector3 prevCorner = startPos;

            for (int i = 1; i < path.corners.Length; i++)
            {
                Vector3 currentCorner = path.corners[i];
                float distSeg = Vector3.Distance(prevCorner, currentCorner);

                if (currentDist + distSeg > maxDistance)
                {
                    float remain = maxDistance - currentDist;
                    Vector3 dir = (currentCorner - prevCorner).normalized;
                    return prevCorner + (dir * remain);
                }
                currentDist += distSeg;
                prevCorner = currentCorner;
            }
            return prevCorner; 
        }

        private void UpdateCurrentTile(Vector3 pos)
        {
            if (_currentTileObj != null)
                _currentTileObj.GetComponent<IMapTile>().SetState(TileState.Obstacle, false);

            if (Physics.Raycast(pos + Vector3.up * 5f, Vector3.down, out RaycastHit hit, Mathf.Infinity, _whatIsGround))
            {
                if (hit.transform.TryGetComponent(out IMapTile tile))
                {
                    tile.SetState(TileState.Enemy | TileState.Obstacle, true);
                    _currentTileObj = hit.transform.gameObject;
                }
            }
        }

        #endregion
    }
}
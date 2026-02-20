using System;
using System.Collections;
using Code.Core.Interfaces;
using Code.UnitSystem;
using UnityEngine;
using Unit = Code.UnitSystem.Unit;

namespace EnemySystem
{
    public class EnemyGridMovingSystem : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private LayerMask _whatIsGround;
        [SerializeField] private float _moveSpeed = 2f;
        [SerializeField] private float _tileSize = 1f; 

        [Header("References")]
        [SerializeField] private UnitRotation _rotationCompo;
        
        private Transform _rootTransform;
        private GameObject _currentTileObj;
        
        public void Initialize(Unit owner)
        {
            _rootTransform = owner.transform;
            if (_rotationCompo == null) 
                _rotationCompo = owner.GetComponent<UnitRotation>();
        }

        private void Awake()
        {
            if (_rootTransform == null) _rootTransform = transform;
        }

        #region [Move]
        
        public void MoveTo(Vector3 targetPos, int maxSteps, Action onComplete)
        {
            StartCoroutine(MoveSequence(targetPos, maxSteps, onComplete));
        }
        
        private IEnumerator MoveSequence(Vector3 targetPos, int maxSteps, Action onComplete)
        {
            for (int i = 0; i < maxSteps; i++)
            {
                Vector3 currentPos2D = new Vector3(_rootTransform.position.x, 0, _rootTransform.position.z);
                Vector3 targetPos2D = new Vector3(targetPos.x, 0, targetPos.z);

                if (Vector3.Distance(currentPos2D, targetPos2D) <= (_tileSize * 1.1f)) 
                {
                    break;
                }

                Vector3 dir = (targetPos2D - currentPos2D).normalized;
                Vector3 step = CalculateStep(dir);
                Vector3 destination = _rootTransform.position + step;

                if (!CheckDestination(destination)) 
                {
                    break;
                }
                
                yield return StartCoroutine(MoveRoutine(destination, null, checkObstacle: false));
            }
            
            onComplete?.Invoke();
        }

        #endregion

        #region [Retreat]

        public void RetreatFromTarget(Vector3 targetPos, int steps, Action onComplete)
        {
            StartCoroutine(RetreatSequence(targetPos, steps, onComplete));
        }

        private IEnumerator RetreatSequence(Vector3 targetPos, int steps, Action onComplete)
        {
            for (int i = 0; i < steps; i++)
            {
                Vector3 currentPos2D = new Vector3(_rootTransform.position.x, 0, _rootTransform.position.z);
                Vector3 targetPos2D = new Vector3(targetPos.x, 0, targetPos.z);

                Vector3 dir = (currentPos2D - targetPos2D).normalized;
                Vector3 step = CalculateStep(dir);
                Vector3 destination = _rootTransform.position + step;

                bool moveSuccess = false;

                yield return StartCoroutine(MoveRoutine(destination, () => moveSuccess = true, checkObstacle: true));
                
                if (!CheckDestination(destination)) break; 
            }

            onComplete?.Invoke();
        }

        #endregion

        #region [Internal Logic]

        private Vector3 CalculateStep(Vector3 dir)
        {
            if (Mathf.Abs(dir.x) > Mathf.Abs(dir.z))
                return new Vector3(Mathf.Sign(dir.x) * _tileSize, 0, 0);
            else
                return new Vector3(0, 0, Mathf.Sign(dir.z) * _tileSize);
        }

        private IEnumerator MoveRoutine(Vector3 targetPos, Action onComplete, bool checkObstacle = true)
        {
            if (checkObstacle && !CheckDestination(targetPos))
            {
                onComplete?.Invoke();
                yield break;
            }

            if (_currentTileObj != null)
                _currentTileObj.GetComponent<IMapTile>().SetObstacle(false);

            if (_rotationCompo != null)
                _rotationCompo.SetDir(targetPos);

            while ((_rootTransform.position - targetPos).sqrMagnitude > 0.0001f)
            {
                _rootTransform.position = Vector3.MoveTowards(
                    _rootTransform.position, 
                    targetPos, 
                    _moveSpeed * Time.deltaTime
                );
                yield return null;
            }
            _rootTransform.position = targetPos;
            
            UpdateCurrentTile(targetPos);

            onComplete?.Invoke();
        }

        private bool CheckDestination(Vector3 pos)
        {
            Vector3 origin = pos + Vector3.up * 2;
            Vector3 dir = Vector3.down;
            float checkRadius = _tileSize * 0.4f;

            if (Physics.SphereCast(origin, checkRadius, dir, out RaycastHit groundHit, Mathf.Infinity, _whatIsGround))
            {
                if (groundHit.transform.TryGetComponent(out IMapTile tile))
                {
                    if (tile.HasObstacle) 
                    {
                        Debug.LogWarning($"[이동 불가] {pos} 타일({groundHit.transform.name}) 위에 이미 다른 유닛/장애물이 있습니다.");
                        return false;
                    }
                    return true;
                }
            }
            else
            {
                Debug.LogWarning($"[이동 불가] {pos} 위치에서 바닥을 찾을 수 없습니다. (타일 틈새 문제는 해결됨. 실제 맵 가장자리 밖이거나 타일이 없는 곳입니다.)");
            }
            
            return false;
        }

        private void UpdateCurrentTile(Vector3 pos)
        {
            float checkRadius = _tileSize * 0.4f;
            if (Physics.SphereCast(pos + Vector3.up * 2, checkRadius, Vector3.down, out RaycastHit hit, Mathf.Infinity, _whatIsGround))
            {
                if (hit.transform.TryGetComponent(out IMapTile tile))
                {
                    tile.SetObstacle(true);
                    _currentTileObj = hit.transform.gameObject;
                }
            }
        }

        #endregion
    }
}
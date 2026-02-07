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

        public void MoveTo(Vector3 targetPos, Action onComplete)
        {
            Vector3 dir = (targetPos - _rootTransform.position).normalized;
            Vector3 step = CalculateStep(dir);
            Vector3 destination = _rootTransform.position + step;

            StartCoroutine(MoveRoutine(destination, onComplete));
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
                Vector3 dir = (_rootTransform.position - targetPos).normalized;
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
                return new Vector3(Mathf.Sign(dir.x), 0, 0);
            else
                return new Vector3(0, 0, Mathf.Sign(dir.z));
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
            if (Physics.Raycast(pos + Vector3.up * 2, Vector3.down, out RaycastHit hit, Mathf.Infinity, _whatIsGround))
            {
                if (hit.transform.TryGetComponent(out IMapTile tile))
                {
                    return !tile.HasObstacle;
                }
            }
            return false;
        }

        private void UpdateCurrentTile(Vector3 pos)
        {
            if (Physics.Raycast(pos + Vector3.up * 2, Vector3.down, out RaycastHit hit, Mathf.Infinity, _whatIsGround))
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
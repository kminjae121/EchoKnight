using System;
using System.Collections;
using Code.Core.Interfaces;
using Code.UnitSystem;
using UnitSystem;
using UnityEngine;
using UnityEngine.Events;
using Unit = Code.UnitSystem.Unit;
using Random = UnityEngine.Random;

namespace EnemySystem
{
    public class EnemyGridMovingSystem : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private UnitRotation rotationCompo;
        [SerializeField] private UnitAnimation animationCompo;
        [SerializeField] private Transform owner;

        [Header("Settings")]
        [SerializeField] private LayerMask whatIsGround;

        public UnityEvent OnMoveEndEvent = new UnityEvent();

        private GameObject _ownTrm;

        private void Awake()
        {
            if (OnMoveEndEvent == null)
                OnMoveEndEvent = new UnityEvent();
        }

        public void Initialize(Unit owner)
        {
        }
        
        private void SetMoveAnim(bool isMoving)
        {
            if (animationCompo != null)
            {
                if (isMoving)
                {
                    animationCompo.PlaySelectAnimation("MOVE");
                }
                else
                {
                    animationCompo.PlaySelectAnimation("IDLE");
                }
            }
        }

        public void Move()
        {
            MoveToRandomGrid();
        }

        private void MoveToRandomGrid()
        {
            int randomDir = Random.Range(1, 5);
            Moveing(randomDir);
        }
        
        public void MoveTowardsTarget(Vector3 targetPos)
        {
            Vector3 dir = (targetPos - transform.position).normalized;
            Vector3 step = CalculateStep(dir);
            
            SetMoveAnim(true);
            StartCoroutine(MoveRoutine(step, true, true, (success) => SetMoveAnim(false)));
        }

        public void RetreatFromTarget(Vector3 targetPos, int steps)
        {
            StartCoroutine(RetreatSequence(targetPos, steps));
        }

        private IEnumerator RetreatSequence(Vector3 targetPos, int steps)
        {
            SetMoveAnim(true); 

            for (int i = 0; i < steps; i++)
            {
                Vector3 dir = (transform.position - targetPos).normalized;
                Vector3 step = CalculateStep(dir);

                bool moveSuccess = false;

                yield return StartCoroutine(MoveRoutine(step, false, false, (success) => moveSuccess = success));

                if (!moveSuccess) break;
            }

            SetMoveAnim(false);
            
            OnMoveEndEvent?.Invoke();
        }

        private Vector3 CalculateStep(Vector3 dir)
        {
            if (Mathf.Abs(dir.x) > Mathf.Abs(dir.z))
                return new Vector3(Mathf.Sign(dir.x), 0, 0);
            else
                return new Vector3(0, 0, Mathf.Sign(dir.z));
        }

        private void Moveing(int dir)
        {
            MoveTo(dir);
        }

        private void MoveTo(int dirInt)
        {
            Vector3 step = dirInt switch
            {
                1 => new Vector3(-1f, 0f, 0f), 
                2 => new Vector3( 1f, 0f, 0f), 
                3 => new Vector3( 0f, 0f, 1f), 
                4 => new Vector3( 0f, 0f,-1f), 
                _ => Vector3.zero
            };

            if (step == Vector3.zero) return;

            SetMoveAnim(true);
            StartCoroutine(MoveRoutine(step, true, true, (success) => SetMoveAnim(false)));
        }

        private IEnumerator MoveRoutine(Vector3 step, bool fireEvent, bool shouldRotate, Action<bool> onComplete = null)
        {
            Vector3 target = owner.position + step;
            bool isSuccess = false;
            
            if(Physics.Raycast(target, Vector3.down, out RaycastHit hit, Mathf.Infinity, whatIsGround))
            {
                if (hit.transform.TryGetComponent(out IMapTile checkTile))
                {
                    if (checkTile.HasObstacle)
                    {
                        if (fireEvent) OnMoveEndEvent?.Invoke();
                        onComplete?.Invoke(false);
                        yield break; 
                    }
                }
                
                if (_ownTrm != null)
                {
                    _ownTrm.GetComponent<IMapTile>().SetObstacle(false);
                }
            
                if (shouldRotate && rotationCompo != null)
                    rotationCompo.SetDir(target);

                while ((owner.position - target).sqrMagnitude > 0.0001f)
                {
                    owner.position = Vector3.MoveTowards(owner.position, target, 2 * Time.deltaTime);
                    yield return null;
                }
                owner.position = target;
        
                if (hit.transform.TryGetComponent(out IMapTile tile))
                {
                    tile.SetObstacle(true);
                    _ownTrm = hit.transform.gameObject;
                }
    
                isSuccess = true;
            }
            
            if (fireEvent) OnMoveEndEvent?.Invoke();
            onComplete?.Invoke(isSuccess);
        }
    }
}
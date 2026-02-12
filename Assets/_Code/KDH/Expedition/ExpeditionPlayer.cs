using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Expedition.Components
{
    public class ExpeditionPlayer : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float rotationSpeed = 10f;
        [SerializeField] private Animator animator;

        private static readonly int WalkHash = Animator.StringToHash("WALK");
        private static readonly int RunHash = Animator.StringToHash("RUN");

        public void Initialize(Vector3 startPosition)
        {
            transform.position = startPosition;
            SetIdle();
        }

        public void MoveAlongPath(List<Vector3> pathPoints, Action onComplete)
        {
            StopAllCoroutines();
            StartCoroutine(MoveRoutine(pathPoints, onComplete));
        }

        private IEnumerator MoveRoutine(List<Vector3> pathPoints, Action onComplete)
        {
            SetWalk(true);

            for (int i = 0; i < pathPoints.Count; i++)
            {
                Vector3 targetPos = pathPoints[i];
                while (Vector3.Distance(transform.position, targetPos) > 0.1f)
                {
                    Vector3 direction = (targetPos - transform.position).normalized;
                    
                    if (direction != Vector3.zero)
                    {
                        Quaternion targetRotation = Quaternion.LookRotation(direction);
                        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                    }

                    transform.position += direction * moveSpeed * Time.deltaTime;
                    yield return null;
                }
            }

            transform.position = pathPoints[pathPoints.Count - 1];
            SetWalk(false);
            
            onComplete?.Invoke();
        }

        private void SetWalk(bool isWalking)
        {
            if (animator != null)
            {
                animator.SetBool(WalkHash, isWalking);
                animator.SetBool(RunHash, false); 
            }
        }

        private void SetIdle()
        {
            if (animator != null)
            {
                animator.SetBool(WalkHash, false);
                animator.SetBool(RunHash, false);
            }
        }
    }
}
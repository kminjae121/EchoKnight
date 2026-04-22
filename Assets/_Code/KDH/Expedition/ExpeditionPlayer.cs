using System;
using System.Collections;
using UnityEngine;

namespace Code.Expedition.Components
{
    [RequireComponent(typeof(RectTransform))]
    public class ExpeditionPlayer : MonoBehaviour
    {
        [Header("UI Movement Settings")]
        [SerializeField] private float moveSpeed = 500f;
        private RectTransform _rectTransform;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        public void Initialize(Vector2 startPosition)
        {
            if (_rectTransform == null)
                _rectTransform = GetComponent<RectTransform>();

            _rectTransform.anchoredPosition = startPosition;
        }

        public void MoveTo(Vector2 targetPos, Action onComplete)
        {
            StopAllCoroutines();
            StartCoroutine(MoveRoutine(targetPos, onComplete));
        }

        private IEnumerator MoveRoutine(Vector2 targetPos, Action onComplete)
        {
            while (Vector2.Distance(_rectTransform.anchoredPosition, targetPos) > 1f)
            {
                _rectTransform.anchoredPosition = Vector2.MoveTowards(
                    _rectTransform.anchoredPosition, 
                    targetPos, 
                    moveSpeed * Time.deltaTime
                );
                yield return null;
            }

            _rectTransform.anchoredPosition = targetPos;
            
            onComplete?.Invoke();
        }
    }
}
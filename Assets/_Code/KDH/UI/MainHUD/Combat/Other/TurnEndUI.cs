using Code.Core.Events.Bus;
using DG.Tweening;
using UnityEngine;

namespace Code.UI
{
    public class TurnEndUI : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private RectTransform panelRect;

        [Header("Animation Settings")]
        [SerializeField] private Vector2 visiblePosition;
        [SerializeField] private Vector2 hiddenPosition;
        [SerializeField] private float slideDuration = 0.3f;
        [SerializeField] private Ease slideEase = Ease.OutBack;

        private Tween _slideTween;
        private bool _isCurrentlyVisible = false;

        private void Awake()
        {
            if (panelRect == null)
            {
                panelRect = GetComponent<RectTransform>();
            }
            
            panelRect.anchoredPosition = hiddenPosition;

            Bus<TurnEndUIEvent>.Subscribe(HandleTurnEndUI);
            Bus<CombatSkillCancelEvent>.Subscribe(HandleSkillCanceled);
            Bus<UnitTurnEndEvent>.Subscribe(HandleUnitTurnEnd);
        }

        private void OnDestroy()
        {
            Bus<TurnEndUIEvent>.Unsubscribe(HandleTurnEndUI);
            Bus<CombatSkillCancelEvent>.Unsubscribe(HandleSkillCanceled);
            Bus<UnitTurnEndEvent>.Unsubscribe(HandleUnitTurnEnd);
            
            _slideTween?.Kill();
        }

        private void HandleTurnEndUI(TurnEndUIEvent evt)
        {
            if (evt.isActive)
            {
                ShowUI();
            }
            else
            {
                HideUI();
            }
        }

        private void HandleSkillCanceled(CombatSkillCancelEvent evt)
        {
            ShowUI();
        }

        private void HandleUnitTurnEnd(UnitTurnEndEvent evt)
        {
            HideUI();
        }

        private void ShowUI()
        {
            if (_isCurrentlyVisible) return;
            
            _isCurrentlyVisible = true;
            _slideTween?.Kill();
            _slideTween = panelRect.DOAnchorPos(visiblePosition, slideDuration).SetEase(slideEase);
        }

        private void HideUI()
        {
            if (!_isCurrentlyVisible) return;
            
            _isCurrentlyVisible = false;
            _slideTween?.Kill();
            _slideTween = panelRect.DOAnchorPos(hiddenPosition, slideDuration).SetEase(Ease.InBack);
        }
    }
}
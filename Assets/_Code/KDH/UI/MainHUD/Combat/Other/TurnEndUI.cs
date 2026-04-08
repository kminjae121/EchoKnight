using System.Reflection;
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

            Bus<SkillUIEvent>.Subscribe(HandleSkillUI);
            Bus<UsingSkillEvent>.Subscribe(HandleUsingSkill);
            Bus<UnitMoveControlEvent>.Subscribe(HandleMoveControl);
            Bus<UnitTurnEndEvent>.Subscribe(HandleUnitTurnEnd);
        }

        private void OnDestroy()
        {
            Bus<SkillUIEvent>.Unsubscribe(HandleSkillUI);
            Bus<UsingSkillEvent>.Unsubscribe(HandleUsingSkill);
            Bus<UnitMoveControlEvent>.Unsubscribe(HandleMoveControl);
            Bus<UnitTurnEndEvent>.Unsubscribe(HandleUnitTurnEnd);
            
            _slideTween?.Kill();
        }

        private void HandleSkillUI(SkillUIEvent evt)
        {
            if (evt.SkillCompo != null) ShowUI();
            else HideUI();
        }

        private void HandleUsingSkill(UsingSkillEvent evt)
        {
            if (evt.isUsingSkill) HideUI();
            else ShowUI();
        }

        private void HandleMoveControl(UnitMoveControlEvent evt)
        {
            bool isControl = true; 
            
            var field = typeof(UnitMoveControlEvent).GetField("isControl") ?? typeof(UnitMoveControlEvent).GetField("isMoveControl");
            if (field != null) isControl = (bool)field.GetValue(evt);
            else
            {
                var prop = typeof(UnitMoveControlEvent).GetProperty("isControl") ?? typeof(UnitMoveControlEvent).GetProperty("isMoveControl");
                if (prop != null) isControl = (bool)prop.GetValue(evt);
            }

            if (isControl) HideUI();
            else ShowUI();
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
using System.Reflection;
using Code.Core.Events.Bus;
using DG.Tweening;
using UnityEngine;

namespace Code.UI
{
    public class CombatStatusAreaUI : MonoBehaviour
    {
        [Header("Status Areas")]
        [SerializeField] private RectTransform buffArea;
        [SerializeField] private RectTransform debuffArea;

        [Header("Animation Settings")]
        [SerializeField] private Vector2 buffVisiblePosition;
        [SerializeField] private Vector2 buffHiddenPosition;
        [SerializeField] private Vector2 debuffVisiblePosition;
        [SerializeField] private Vector2 debuffHiddenPosition;
        [SerializeField] private float slideDuration = 0.3f;
        [SerializeField] private Ease slideEase = Ease.OutBack;

        private Tween _buffTween;
        private Tween _debuffTween;
        private bool _isCurrentlyVisible = false;

        private void Awake()
        {
            if (buffArea != null) buffArea.anchoredPosition = buffHiddenPosition;
            if (debuffArea != null) debuffArea.anchoredPosition = debuffHiddenPosition;

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
            
            _buffTween?.Kill();
            _debuffTween?.Kill();
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
            
            if (buffArea != null)
            {
                _buffTween?.Kill();
                _buffTween = buffArea.DOAnchorPos(buffVisiblePosition, slideDuration).SetEase(slideEase);
            }

            if (debuffArea != null)
            {
                _debuffTween?.Kill();
                _debuffTween = debuffArea.DOAnchorPos(debuffVisiblePosition, slideDuration).SetEase(slideEase);
            }
        }

        private void HideUI()
        {
            if (!_isCurrentlyVisible) return;
            
            _isCurrentlyVisible = false;
            
            if (buffArea != null)
            {
                _buffTween?.Kill();
                _buffTween = buffArea.DOAnchorPos(buffHiddenPosition, slideDuration).SetEase(Ease.InBack);
            }

            if (debuffArea != null)
            {
                _debuffTween?.Kill();
                _debuffTween = debuffArea.DOAnchorPos(debuffHiddenPosition, slideDuration).SetEase(Ease.InBack);
            }
        }
    }
}
using Code.Core.Events.Bus;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    public class TurnEndUI : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private RectTransform panelRect;
        [SerializeField] private Button turnEndButton;

        [Header("Animation Settings")]
        [SerializeField] private Vector2 visiblePosition;
        [SerializeField] private Vector2 hiddenPosition;
        [SerializeField] private float slideDuration = 0.3f;
        [SerializeField] private Ease slideEase = Ease.OutBack;

        private Tween _slideTween;
        private bool _isCurrentlyVisible = false;
        private bool _isActionPlaying = false;

        private void Awake()
        {
            if (panelRect == null)
            {
                panelRect = GetComponent<RectTransform>();
            }
            
            panelRect.anchoredPosition = hiddenPosition;

            if (turnEndButton != null)
            {
                turnEndButton.onClick.AddListener(OnTurnEndButtonClicked);
            }

            Bus<SkillUIEvent>.Subscribe(HandleSkillUI);
            Bus<SetAtkUIEvent>.Subscribe(HandleAtkUI);
            Bus<UnitSkilStartEvent>.Subscribe(HandleSkillStart);
            Bus<UnitMoveControlEvent>.Subscribe(HandleMoveControl);
            Bus<UnitTurnEndEvent>.Subscribe(HandleUnitTurnEnd);
            Bus<CombatSkillCancelEvent>.Subscribe(HandleSkillCancel);
        }

        private void OnDestroy()
        {
            if (turnEndButton != null)
            {
                turnEndButton.onClick.RemoveListener(OnTurnEndButtonClicked);
            }

            Bus<SkillUIEvent>.Unsubscribe(HandleSkillUI);
            Bus<SetAtkUIEvent>.Unsubscribe(HandleAtkUI);
            Bus<UnitSkilStartEvent>.Unsubscribe(HandleSkillStart);
            Bus<UnitMoveControlEvent>.Unsubscribe(HandleMoveControl);
            Bus<UnitTurnEndEvent>.Unsubscribe(HandleUnitTurnEnd);
            Bus<CombatSkillCancelEvent>.Unsubscribe(HandleSkillCancel);
            
            _slideTween?.Kill();
        }

        private void OnTurnEndButtonClicked()
        {
            Bus<UnitTurnEndEvent>.Raise(new UnitTurnEndEvent());
        }

        private void HandleSkillUI(SkillUIEvent evt)
        {
            HideUI();
            
            if (evt.SkillCompo != null) 
            {
                DOVirtual.DelayedCall(0.5f, () => 
                {
                    if (this == null) return;
                    if (!_isActionPlaying) ShowUI();
                });
            }
        }

        private void HandleAtkUI(SetAtkUIEvent evt)
        {
            _isActionPlaying = !evt.IsActive;
            
            if (evt.IsActive) ShowUI();
            else HideUI();
        }

        private void HandleSkillStart(UnitSkilStartEvent evt)
        {
            _isActionPlaying = evt.isStart;
            
            if (evt.isStart) HideUI();
            else ShowUI();
        }

        private void HandleMoveControl(UnitMoveControlEvent evt)
        {
            _isActionPlaying = !evt.isMoving; 
            
            if (evt.isMoving) ShowUI();
            else HideUI();
        }

        private void HandleUnitTurnEnd(UnitTurnEndEvent evt)
        {
            _isActionPlaying = false;
            HideUI();
        }

        private void HandleSkillCancel(CombatSkillCancelEvent evt)
        {
            if (!_isActionPlaying) 
            {
                ShowUI();
            }
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
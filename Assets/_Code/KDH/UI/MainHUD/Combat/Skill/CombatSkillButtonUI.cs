using Code.Core.Events.Bus;
using Code.SkillSystem;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Code.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class CombatSkillButtonUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField] private Image skillIcon;
        [SerializeField] private TextMeshProUGUI damageText;
        [SerializeField] private TextMeshProUGUI costText;
        [SerializeField] private Image dimOverlay;
        
        [SerializeField] private float hoverYOffset = 15f;
        [SerializeField] private float selectYOffset = 30f;
        [SerializeField] private float animDuration = 0.2f;
        [SerializeField] private Ease animEase = Ease.OutCubic;

        private RectTransform _rectTransform;
        private Vector2 _originalPosition;
        private Tween _moveTween;
        private SkillSO _currentSkill;
        private bool _isSelected;
        private bool _isInteractable;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _originalPosition = _rectTransform.anchoredPosition;
            
            Bus<CombatSkillCancelEvent>.Subscribe(HandleSkillCancel);
            Bus<CombatSkillSelectEvent>.Subscribe(HandleOtherSkillSelected);
        }

        private void OnDestroy()
        {
            Bus<CombatSkillCancelEvent>.Unsubscribe(HandleSkillCancel);
            Bus<CombatSkillSelectEvent>.Unsubscribe(HandleOtherSkillSelected);
            _moveTween?.Kill();
        }

        public void SetupSkill(SkillSO skill, int currentTurnCost)
        {
            _currentSkill = skill;
            _isSelected = false;
            
            skillIcon.sprite = skill.skillUIImage;
            damageText.text = skill.SkillDamage.ToString();
            costText.text = skill.SkillCost.ToString();

            _isInteractable = currentTurnCost >= skill.SkillCost;

            if (!_isInteractable)
            {
                transform.SetAsFirstSibling();
                if (dimOverlay != null) dimOverlay.gameObject.SetActive(true);
            }
            else
            {
                if (dimOverlay != null) dimOverlay.gameObject.SetActive(false);
            }

            ResetPosition();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!_isInteractable || _isSelected) return;

            _moveTween?.Kill();
            _moveTween = _rectTransform.DOAnchorPosY(_originalPosition.y + hoverYOffset, animDuration).SetEase(animEase);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!_isInteractable || _isSelected) return;

            _moveTween?.Kill();
            _moveTween = _rectTransform.DOAnchorPosY(_originalPosition.y, animDuration).SetEase(animEase);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!_isInteractable)
            {
                Bus<ShowMessageUIEvent>.Raise(new ShowMessageUIEvent("코스트가 부족하여 스킬을 사용할 수 없습니다."));
                return;
            }

            if (eventData.button == PointerEventData.InputButton.Left)
            {
                SelectThisSkill();
            }
        }

        private void SelectThisSkill()
        {
            _isSelected = true;
            _moveTween?.Kill();
            _moveTween = _rectTransform.DOAnchorPosY(_originalPosition.y + selectYOffset, animDuration).SetEase(animEase);
            
            Bus<CombatSkillSelectEvent>.Raise(new CombatSkillSelectEvent(_currentSkill));
        }

        private void HandleOtherSkillSelected(CombatSkillSelectEvent evt)
        {
            if (evt.SelectedSkill != _currentSkill && _isSelected)
            {
                _isSelected = false;
                ResetPosition();
            }
        }

        private void HandleSkillCancel(CombatSkillCancelEvent evt)
        {
            if (_isSelected)
            {
                _isSelected = false;
                ResetPosition();
            }
        }

        private void ResetPosition()
        {
            _moveTween?.Kill();
            _moveTween = _rectTransform.DOAnchorPosY(_originalPosition.y, animDuration).SetEase(animEase);
        }
    }
}
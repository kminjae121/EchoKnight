using System.Collections.Generic;
using Code.Core.Events.Bus;
using Code.Core.Managers;
using Code.Managers;
using Code.SkillSystem;
using Code.UnitSystem;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class CombatSkillUIManager : MonoBehaviour
    {
        [SerializeField] private List<CombatSkillButtonUI> skillSlots;
        [SerializeField] private Button nextPageButton;
        [SerializeField] private Button prevPageButton;
        [SerializeField] private Button backgroundCancelButton;

        [SerializeField] private float slideDuration = 0.3f;
        [SerializeField] private Ease slideEase = Ease.OutBack;
        [SerializeField] private Vector2 hiddenPosition;
        [SerializeField] private Vector2 visiblePosition;

        private RectTransform _rectTransform;
        private Tween _slideTween;
        private UnitSO _currentUnit;
        private SkillSO[] _equippedSkills;
        private int _currentPage = 0;
        private const int MaxSkillsPerPage = 3;
        private bool _isSkillSelected = false;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _rectTransform.anchoredPosition = hiddenPosition;

            if (nextPageButton != null) nextPageButton.onClick.AddListener(GoToNextPage);
            if (prevPageButton != null) prevPageButton.onClick.AddListener(GoToPrevPage);
            if (backgroundCancelButton != null) backgroundCancelButton.onClick.AddListener(CancelSkillSelection);

            Bus<CharacterInfoEvent>.Subscribe(HandleTurnStart);
            Bus<CombatSkillSelectEvent>.Subscribe(HandleSkillSelected);
            Bus<UnitTurnEndEvent>.Subscribe(HandleTurnEnd);
            
            if (backgroundCancelButton != null) backgroundCancelButton.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            if (nextPageButton != null) nextPageButton.onClick.RemoveListener(GoToNextPage);
            if (prevPageButton != null) prevPageButton.onClick.RemoveListener(GoToPrevPage);
            if (backgroundCancelButton != null) backgroundCancelButton.onClick.RemoveListener(CancelSkillSelection);

            Bus<CharacterInfoEvent>.Unsubscribe(HandleTurnStart);
            Bus<CombatSkillSelectEvent>.Unsubscribe(HandleSkillSelected);
            Bus<UnitTurnEndEvent>.Unsubscribe(HandleTurnEnd);

            _slideTween?.Kill();
        }

        private void Update()
        {
            if (_isSkillSelected && UnityEngine.Input.GetMouseButtonDown(1))
            {
                CancelSkillSelection();
            }
        }

        private void HandleTurnStart(CharacterInfoEvent evt)
        {
            if (evt.Unit == null)
            {
                HideUI();
                return;
            }

            _currentUnit = evt.Unit.Data;
            _equippedSkills = SkillSendManager.Instance.GetEquipSkills(_currentUnit.UnitType);
            _currentPage = 0;
            _isSkillSelected = false;

            if (backgroundCancelButton != null) backgroundCancelButton.gameObject.SetActive(false);

            RefreshSkillSlots();
            ShowUI();
        }

        private void HandleTurnEnd(UnitTurnEndEvent evt)
        {
            HideUI();
        }

        private void RefreshSkillSlots()
        {
            if (_equippedSkills == null || _equippedSkills.Length == 0)
            {
                foreach (var slot in skillSlots) slot.gameObject.SetActive(false);
                if (nextPageButton != null) nextPageButton.gameObject.SetActive(false);
                if (prevPageButton != null) prevPageButton.gameObject.SetActive(false);
                return;
            }

            int startIndex = _currentPage * MaxSkillsPerPage;
            int currentTurnCost = GetCurrentUnitCost();

            for (int i = 0; i < MaxSkillsPerPage; i++)
            {
                int skillIndex = startIndex + i;
                if (skillIndex < _equippedSkills.Length && _equippedSkills[skillIndex] != null)
                {
                    skillSlots[i].gameObject.SetActive(true);
                    skillSlots[i].SetupSkill(_equippedSkills[skillIndex], currentTurnCost);
                }
                else
                {
                    skillSlots[i].gameObject.SetActive(false);
                }
            }

            UpdatePaginationButtons();
        }

        private void UpdatePaginationButtons()
        {
            int totalPages = Mathf.CeilToInt((float)_equippedSkills.Length / MaxSkillsPerPage);
            
            if (nextPageButton != null) 
                nextPageButton.gameObject.SetActive(totalPages > 1 && _currentPage < totalPages - 1);
                
            if (prevPageButton != null) 
                prevPageButton.gameObject.SetActive(totalPages > 1 && _currentPage > 0);
        }

        private void GoToNextPage()
        {
            _currentPage++;
            RefreshSkillSlots();
        }

        private void GoToPrevPage()
        {
            _currentPage--;
            RefreshSkillSlots();
        }

        private void HandleSkillSelected(CombatSkillSelectEvent evt)
        {
            _isSkillSelected = true;
            if (backgroundCancelButton != null) backgroundCancelButton.gameObject.SetActive(true);
        }

        private void CancelSkillSelection()
        {
            if (!_isSkillSelected) return;

            _isSkillSelected = false;
            Bus<CombatSkillCancelEvent>.Raise(new CombatSkillCancelEvent());
            
            if (backgroundCancelButton != null) backgroundCancelButton.gameObject.SetActive(false);
        }

        private int GetCurrentUnitCost()
        {
            var costManager = UnityEngine.Object.FindFirstObjectByType<TurnCostGaugeManager>();
            
            if (costManager != null && costManager.currentGaugeValue != null)
            {
                return costManager.currentGaugeValue.Value; 
            }
            
            return 99; 
        }

        private void ShowUI()
        {
            _slideTween?.Kill();
            _slideTween = _rectTransform.DOAnchorPos(visiblePosition, slideDuration).SetEase(slideEase);
        }

        private void HideUI()
        {
            CancelSkillSelection();
            _slideTween?.Kill();
            _slideTween = _rectTransform.DOAnchorPos(hiddenPosition, slideDuration).SetEase(Ease.InBack);
        }
    }
}
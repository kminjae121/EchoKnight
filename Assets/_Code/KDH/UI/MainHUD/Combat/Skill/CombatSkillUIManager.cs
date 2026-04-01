using System.Collections.Generic;
using Code.Core.Events.Bus;
using Code.Core.Managers;
using Code.Managers;
using Code.UnitSystem;
using Code.SkillSystem;
using DG.Tweening;
using GondrLib.ObjectPool.Runtime;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    public class CombatSkillUIManager : MonoBehaviour
    {
        [Header("UI Area & Pooling")]
        [SerializeField] private RectTransform skillArea;
        [SerializeField] private PoolingItemSO skillButtonPoolingSO;

        [Header("Slots & Buttons")]
        [SerializeField] private List<RectTransform> skillSlotPositions;
        [SerializeField] private Button nextPageButton;
        [SerializeField] private Button prevPageButton;
        [SerializeField] private Button backgroundCancelButton;

        [Header("Slide Animation")]
        [SerializeField] private float slideDuration = 0.3f;
        [SerializeField] private Ease slideEase = Ease.OutBack;
        [SerializeField] private Vector2 hiddenPosition;
        [SerializeField] private Vector2 visiblePosition;

        private Tween _slideTween;
        private UnitSO _currentUnit;
        private SkillSO[] _equippedSkills;
        private int _currentPage = 0;
        private const int MaxSkillsPerPage = 3;
        private bool _isSkillSelected = false;
        
        private PoolManagerMono _poolManager;
        private List<CombatSkillButtonUI> _activeSkillButtons = new List<CombatSkillButtonUI>();

        private void Awake()
        {
            _poolManager = UnityEngine.Object.FindFirstObjectByType<PoolManagerMono>();

            if (skillArea != null)
            {
                skillArea.anchoredPosition = hiddenPosition;
            }

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

            if (_equippedSkills != null && _equippedSkills.Length > MaxSkillsPerPage)
            {
                if (nextPageButton != null) nextPageButton.gameObject.SetActive(true);
                if (prevPageButton != null) prevPageButton.gameObject.SetActive(false);
            }
            else
            {
                if (nextPageButton != null) nextPageButton.gameObject.SetActive(false);
                if (prevPageButton != null) prevPageButton.gameObject.SetActive(false);
            }

            RefreshSkillSlots();
            ShowUI();
        }

        private void HandleTurnEnd(UnitTurnEndEvent evt)
        {
            HideUI();
        }

        private void RefreshSkillSlots()
        {
            foreach (var btn in _activeSkillButtons)
            {
                btn.ReturnToPool();
            }
            _activeSkillButtons.Clear();

            if (_equippedSkills == null || _equippedSkills.Length == 0) return;

            int startIndex = _currentPage * MaxSkillsPerPage;
            int currentTurnCost = GetCurrentUnitCost();

            for (int i = 0; i < MaxSkillsPerPage; i++)
            {
                int skillIndex = startIndex + i;
                if (skillIndex < _equippedSkills.Length && _equippedSkills[skillIndex] != null)
                {
                    if (i < skillSlotPositions.Count && skillSlotPositions[i] != null)
                    {
                        var btn = _poolManager.Pop<CombatSkillButtonUI>(skillButtonPoolingSO);
                        btn.transform.SetParent(skillSlotPositions[i]);
                        btn.transform.localPosition = Vector3.zero;
                        btn.transform.localScale = Vector3.one;
                        
                        btn.SetupSkill(_equippedSkills[skillIndex], currentTurnCost);
                        _activeSkillButtons.Add(btn);
                    }
                }
            }
        }

        private void GoToNextPage()
        {
            _currentPage = 1;
            RefreshSkillSlots();

            if (nextPageButton != null) nextPageButton.gameObject.SetActive(false);
            if (prevPageButton != null) prevPageButton.gameObject.SetActive(true);
        }

        private void GoToPrevPage()
        {
            _currentPage = 0;
            RefreshSkillSlots();

            if (nextPageButton != null) nextPageButton.gameObject.SetActive(true);
            if (prevPageButton != null) prevPageButton.gameObject.SetActive(false);
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
            if (skillArea != null)
            {
                _slideTween = skillArea.DOAnchorPos(visiblePosition, slideDuration).SetEase(slideEase);
            }
        }

        private void HideUI()
        {
            CancelSkillSelection();
            _slideTween?.Kill();
            if (skillArea != null)
            {
                _slideTween = skillArea.DOAnchorPos(hiddenPosition, slideDuration).SetEase(Ease.InBack);
            }
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using Code.Core.Events.Bus;
using Code.Core.Managers;
using Code.Managers; 
using Code.SkillSystem;
using Code.UnitSystem;
using Code.UnitSystem.Combat;
using Code.Items;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using GondrLib.ObjectPool.Runtime;

namespace Code.UI
{
    public class CombatInfoUI : MonoBehaviour
    {
        [Header("UI Panel & Animation")]
        [SerializeField] private RectTransform panelRect;
        [SerializeField] private GameObject backgroundPanel;
        [SerializeField] private Vector2 hiddenPosition = Vector2.zero;
        [SerializeField] private Vector2 visiblePosition = Vector2.zero;
        [SerializeField] private float slideDuration = 0.3f;
        [SerializeField] private Ease slideEase = Ease.OutQuart;
        [SerializeField] private Button openButton;
        [SerializeField] private Button closeButton;

        [Header("Basic Info")]
        [SerializeField] private Image unitImage;
        [SerializeField] private TextMeshProUGUI unitNameText;
        [SerializeField] private TextMeshProUGUI unitClassText;
        
        [Header("Health Info")]
        [SerializeField] private TextMeshProUGUI hpText;
        [SerializeField] private Image hpFillImage;

        [Header("Stats")]
        [SerializeField] private TextMeshProUGUI turnSpeedText;
        [SerializeField] private TextMeshProUGUI moveSpeedText;
        [SerializeField] private TextMeshProUGUI maxHealthText;
        [SerializeField] private TextMeshProUGUI attackDamageText;
        [SerializeField] private TextMeshProUGUI defensivePowerText;
        [SerializeField] private TextMeshProUGUI criticalProbabilityText;
        [SerializeField] private TextMeshProUGUI criticalDamageIncreaseText;
        [SerializeField] private TextMeshProUGUI maxSkillCostText;
        [SerializeField] private TextMeshProUGUI recoverySkillCostText;

        [Header("Skills & Artifacts")]
        [SerializeField] private RectTransform skillPanelGroup;
        [SerializeField] private PoolingItemSO characterSkillButtonPoolingSO;
        [SerializeField] private List<Image> artifactIcons;
        [SerializeField] private List<Image> artifactRarityImages;
        [SerializeField] private List<Sprite> raritySprites;
        [SerializeField] private Sprite emptyArtifactSprite;

        private Tween _slideTween;
        private UnitState _currentUnitState; 
        private Unit _currentInGameUnit;     
        
        private bool _isVisible;
        private bool _isManualTargeting; 
        
        private PoolManagerMono _poolManager;
        private TurnManager _turnManager; 
        private List<CharacterSkillButton> _activeSkillButtons = new List<CharacterSkillButton>();

        private void Awake()
        {
            _poolManager = UnityEngine.Object.FindFirstObjectByType<PoolManagerMono>();
            _turnManager = UnityEngine.Object.FindFirstObjectByType<TurnManager>();

            if (_poolManager == null) Debug.LogError("[CombatInfoUI] 풀 매니저를 찾을 수 없습니다.");

            if (panelRect == null) panelRect = GetComponent<RectTransform>();
            panelRect.anchoredPosition = hiddenPosition;
            
            if (backgroundPanel != null) backgroundPanel.SetActive(false);
            if (openButton != null) openButton.onClick.AddListener(ShowUI);
            if (closeButton != null) closeButton.onClick.AddListener(HideUI);

            Bus<ShowCombatInfoEvent>.Subscribe(HandleShowCombatInfo);
            Bus<TurnOrderUpdateEvent>.Subscribe(HandleTurnUpdate); 
            
            SetupArtifactTriggers();
        }

        private void OnDestroy()
        {
            if (openButton != null) openButton.onClick.RemoveListener(ShowUI);
            if (closeButton != null) closeButton.onClick.RemoveListener(HideUI);
            
            Bus<ShowCombatInfoEvent>.Unsubscribe(HandleShowCombatInfo);
            Bus<TurnOrderUpdateEvent>.Unsubscribe(HandleTurnUpdate);
            
            UnsubscribeHealth();
            _slideTween?.Kill();
        }

        private void HandleTurnUpdate(TurnOrderUpdateEvent evt)
        {
            if (_isVisible && !_isManualTargeting)
            {
                UpdateToCurrentTurnUnit();
            }
        }

        private void HandleShowCombatInfo(ShowCombatInfoEvent evt)
        {
            UnsubscribeHealth();

            if (evt.IsShow && evt.TargetUnit != null)
            {
                _isManualTargeting = true; 
                _currentUnitState = evt.TargetUnit;
                
                if (_currentUnitState.CurrentHp != null)
                {
                    _currentUnitState.CurrentHp.OnValueChanged += OnHealthChangedState;
                }
                
                ShowUI();
            }
            else
            {
                HideUI();
            }
        }

        private void UpdateToCurrentTurnUnit()
        {
            if (_turnManager == null) _turnManager = UnityEngine.Object.FindFirstObjectByType<TurnManager>();
            
            if (_turnManager != null)
            {
                var units = _turnManager.GetTimelineUnits(1);
                
                if (units != null && units.Count > 0 && units[0] is Unit newTurnUnit)
                {
                    if (_currentInGameUnit != newTurnUnit)
                    {
                        UnsubscribeHealth();
                        _currentInGameUnit = newTurnUnit;
                        
                        var healthCompo = _currentInGameUnit.GetUnitCompo<UnitHealth>();
                        if (healthCompo != null)
                        {
                            healthCompo.OnHealthChangedEvent += OnHealthChangedInGame;
                        }
                        
                        if (_isVisible) RefreshAllUI();
                    }
                }
            }
        }

        private void UnsubscribeHealth()
        {
            if (_currentUnitState != null && _currentUnitState.CurrentHp != null)
            {
                _currentUnitState.CurrentHp.OnValueChanged -= OnHealthChangedState;
            }
            
            if (_currentInGameUnit != null)
            {
                var healthCompo = _currentInGameUnit.GetUnitCompo<UnitHealth>();
                if (healthCompo != null)
                {
                    healthCompo.OnHealthChangedEvent -= OnHealthChangedInGame;
                }
            }
        }

        private void OnHealthChangedState(float prevHp, float nextHp) => RefreshHealthUI();

        private void OnHealthChangedInGame(float current, float max) => RefreshHealthUI();

        public void ShowUI()
        {
            if (_isVisible) return;
            
            if (_currentUnitState == null && !_isManualTargeting)
            {
                UpdateToCurrentTurnUnit();
            }

            _isVisible = true;
            if (backgroundPanel != null) backgroundPanel.SetActive(true);

            RefreshAllUI();

            _slideTween?.Kill();
            _slideTween = panelRect.DOAnchorPos(visiblePosition, slideDuration).SetEase(slideEase);
        }

        public void HideUI()
        {
            if (!_isVisible) return;
            
            _isVisible = false;
            _isManualTargeting = false; 
            
            _slideTween?.Kill();
            _slideTween = panelRect.DOAnchorPos(hiddenPosition, slideDuration).SetEase(Ease.InBack).OnComplete(() => 
            {
                if (backgroundPanel != null) backgroundPanel.SetActive(false);
            });
            
            Bus<SkillUIHoverEvent>.Raise(new SkillUIHoverEvent(null, null));
            Bus<CombatArtifactHoverEvent>.Raise(new CombatArtifactHoverEvent(null, false));

            UnsubscribeHealth();
            _currentUnitState = null; 
            _currentInGameUnit = null;
        }
        
        private UnitSO GetCurrentUnitData()
        {
            if (_isManualTargeting && _currentUnitState != null)
                return _currentUnitState.Data;
            if (!_isManualTargeting && _currentInGameUnit != null)
                return _currentInGameUnit.unitSO;
                
            return null;
        }

        private void RefreshAllUI()
        {
            UnitSO data = GetCurrentUnitData();
            if (data == null) return;

            if (unitImage != null) unitImage.sprite = data.UnitImage;
            if (unitNameText != null) unitNameText.text = data.UnitName;
            if (unitClassText != null) unitClassText.text = data.UnitClass;
            
            RefreshHealthUI();

            if (turnSpeedText != null) turnSpeedText.text = data.turnSpeed.ToString("F1");
            if (moveSpeedText != null) moveSpeedText.text = data.MoveRange.ToString("F1");
            if (maxHealthText != null) maxHealthText.text = data.Maxhealth.ToString("F1");
            if (attackDamageText != null) attackDamageText.text = data.AttackDamage.ToString("F1");
            if (defensivePowerText != null) defensivePowerText.text = data.DefensivePower.ToString("F1");
            if (criticalProbabilityText != null) criticalProbabilityText.text = $"{data.CriticalProbability:F1}%";
            if (criticalDamageIncreaseText != null) criticalDamageIncreaseText.text = data.CriticalDamageIncrease.ToString("F1");
            if (maxSkillCostText != null) maxSkillCostText.text = data.MaxSkillCost.ToString();
            if (recoverySkillCostText != null) recoverySkillCostText.text = data.RecoverySkillCost.ToString();

            RefreshSkills();
            RefreshArtifacts();
        }

        private void RefreshHealthUI()
        {
            float currentHp = 0f;
            float maxHp = 0f;

            if (_isManualTargeting && _currentUnitState != null && _currentUnitState.Data != null)
            {
                currentHp = _currentUnitState.CurrentHp.Value;
                maxHp = _currentUnitState.Data.Maxhealth;
            }
            else if (!_isManualTargeting && _currentInGameUnit != null)
            {
                var healthCompo = _currentInGameUnit.GetUnitCompo<UnitHealth>();
                if (healthCompo != null)
                {
                    currentHp = healthCompo.CurrentHealth;
                    maxHp = healthCompo.MaxHealth;
                }
            }

            if (hpText != null) hpText.text = $"{Mathf.CeilToInt(currentHp)} / {Mathf.CeilToInt(maxHp)}";
            if (hpFillImage != null) hpFillImage.fillAmount = maxHp > 0 ? (currentHp / maxHp) : 0f;
        }

        private void RefreshSkills()
        {
            List<SkillSO> equippedSkills = new List<SkillSO>();
            UnitSO data = GetCurrentUnitData();

            if (!_isManualTargeting && _currentInGameUnit != null)
            {
                Code.SkillSystem.SkillComponent skillCompo = null;
                
                if (_currentInGameUnit is CharacterUnit charUnit)
                    skillCompo = charUnit.SkillCompo;
                else if (_currentInGameUnit is Code.UnitSystem.Enemies.AbstractEnemyUnit enemyUnit)
                    skillCompo = enemyUnit.SkillCompo;

                if (skillCompo != null && skillCompo.Skills != null)
                {
                    equippedSkills.AddRange(skillCompo.Skills.Keys);
                }
            }
            else if (_isManualTargeting && _currentUnitState != null)
            {
                if (SkillSendManager.Instance != null && data != null)
                {
                    var skills = SkillSendManager.Instance.GetEquipSkills(data.UnitType);
                    if (skills != null) equippedSkills.AddRange(skills);
                }
            }

            foreach (var btn in _activeSkillButtons)
            {
                if (btn != null) btn.ReturnToPool();
            }
            _activeSkillButtons.Clear();

            if (skillPanelGroup != null && characterSkillButtonPoolingSO != null)
            {
                foreach(var skillData in equippedSkills)
                {
                    if (skillData != null)
                    {
                        var btn = _poolManager.Pop<CharacterSkillButton>(characterSkillButtonPoolingSO);
                        if (btn != null)
                        {
                            btn.transform.SetParent(skillPanelGroup);
                            btn.transform.localScale = Vector3.one;
                            btn.SetSkill(skillData, true);
                            _activeSkillButtons.Add(btn);
                        }
                    }
                }
            }
        }

        private void RefreshArtifacts()
        {
            UnitSO data = GetCurrentUnitData();
            
            for (int i = 0; i < artifactIcons.Count; i++)
            {
                bool hasArtifact = false;
                EquipmentItemSO equipSO = null;

                if (data != null && data.EquippedArtifacts != null && data.EquippedArtifacts.artifacts != null)
                {
                    if (i < data.EquippedArtifacts.artifacts.Count && data.EquippedArtifacts.artifacts[i] != null)
                    {
                        if (data.EquippedArtifacts.artifacts[i] is EquipmentItemSO item)
                        {
                            hasArtifact = true;
                            equipSO = item;
                        }
                    }
                }

                if (hasArtifact && equipSO != null)
                {
                    artifactIcons[i].sprite = equipSO.itemIcon;

                    if (i < artifactRarityImages.Count && artifactRarityImages[i] != null)
                    {
                        artifactRarityImages[i].gameObject.SetActive(true);
                        int rarityIndex = (int)equipSO.rarity;
                        if (raritySprites != null && rarityIndex >= 0 && rarityIndex < raritySprites.Count)
                        {
                            artifactRarityImages[i].sprite = raritySprites[rarityIndex];
                        }
                    }
                }
                else
                {
                    artifactIcons[i].sprite = emptyArtifactSprite;
                    if (i < artifactRarityImages.Count && artifactRarityImages[i] != null)
                    {
                        artifactRarityImages[i].gameObject.SetActive(false);
                    }
                }
                
                var trigger = artifactIcons[i].GetComponent<SlotHoverClickTrigger>();
                if (trigger != null) trigger.SetInteractable(hasArtifact);
            }
        }

        private void SetupArtifactTriggers()
        {
            for (int i = 0; i < artifactIcons.Count; i++)
            {
                int index = i;
                var trigger = artifactIcons[i].GetComponent<SlotHoverClickTrigger>();
                if (trigger == null) trigger = artifactIcons[i].gameObject.AddComponent<SlotHoverClickTrigger>();
                
                trigger.useHoverVisuals = false;
                trigger.OnHoverEnter = (pivot, triggerOffset) =>
                {
                    UnitSO data = GetCurrentUnitData();
                    if (data != null && data.EquippedArtifacts != null && data.EquippedArtifacts.artifacts != null)
                    {
                        var artifacts = data.EquippedArtifacts.artifacts;
                        if (index < artifacts.Count && artifacts[index] != null)
                            Bus<CombatArtifactHoverEvent>.Raise(new CombatArtifactHoverEvent(artifacts[index], true));
                    }
                };
                trigger.OnHoverExit = () => Bus<CombatArtifactHoverEvent>.Raise(new CombatArtifactHoverEvent(null, false));
            }
        }
    }
}
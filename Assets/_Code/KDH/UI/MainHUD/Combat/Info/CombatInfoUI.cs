using System.Collections.Generic;
using Code.Core.Events.Bus;
using Code.Core.Managers;
using Code.SkillSystem;
using Code.UnitSystem;
using Code.Items;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    public class CombatInfoUI : MonoBehaviour
    {
        [Header("UI Panel & Animation")]
        [SerializeField] private RectTransform panelRect;
        [SerializeField] private GameObject backgroundPanel;
        [SerializeField] private Vector2 hiddenPosition = new Vector2(-1000f, 0f);
        [SerializeField] private Vector2 visiblePosition = Vector2.zero;
        [SerializeField] private float slideDuration = 0.3f;
        [SerializeField] private Ease slideEase = Ease.OutQuart;
        [SerializeField] private Button openButton;
        [SerializeField] private Button closeButton;

        [Header("Basic Info")]
        [SerializeField] private Image unitImage;
        [SerializeField] private TextMeshProUGUI unitNameText;
        [SerializeField] private TextMeshProUGUI unitClassText;

        [Header("Stats")]
        [SerializeField] private TextMeshProUGUI turnSpeedText;
        [SerializeField] private TextMeshProUGUI moveSpeedText;
        [SerializeField] private TextMeshProUGUI maxHealthText;
        [SerializeField] private TextMeshProUGUI attackDamageText;
        [SerializeField] private TextMeshProUGUI defensivePowerText;
        [SerializeField] private TextMeshProUGUI avoidProbabilityText;
        [SerializeField] private TextMeshProUGUI criticalProbabilityText;
        [SerializeField] private TextMeshProUGUI criticalDamageIncreaseText;
        [SerializeField] private TextMeshProUGUI maxSkillCostText;
        [SerializeField] private TextMeshProUGUI recoverySkillCostText;

        [Header("Skills & Artifacts")]
        [SerializeField] private List<Image> skillIcons;
        [SerializeField] private Sprite emptySkillSprite;
        [SerializeField] private List<Image> artifactIcons;
        [SerializeField] private Sprite emptyArtifactSprite;

        private Tween _slideTween;
        private UnitState _currentUnit;
        private bool _isVisible;

        private void Awake()
        {
            if (panelRect == null)
            {
                panelRect = GetComponent<RectTransform>();
            }
            
            panelRect.anchoredPosition = hiddenPosition;
            
            if (backgroundPanel != null)
            {
                backgroundPanel.SetActive(false);
            }

            if (openButton != null)
            {
                openButton.onClick.AddListener(ShowUI);
            }

            if (closeButton != null)
            {
                closeButton.onClick.AddListener(HideUI);
            }

            Bus<ShowCombatInfoEvent>.Subscribe(HandleShowCombatInfo);

            SetupSkillTriggers();
            SetupArtifactTriggers();
        }

        private void OnDestroy()
        {
            if (openButton != null)
            {
                openButton.onClick.RemoveListener(ShowUI);
            }

            if (closeButton != null)
            {
                closeButton.onClick.RemoveListener(HideUI);
            }
            
            Bus<ShowCombatInfoEvent>.Unsubscribe(HandleShowCombatInfo);
            _slideTween?.Kill();
        }

        private void HandleShowCombatInfo(ShowCombatInfoEvent evt)
        {
            if (evt.IsShow && evt.TargetUnit != null)
            {
                _currentUnit = evt.TargetUnit;
                RefreshAllUI();
                ShowUI();
            }
            else
            {
                HideUI();
            }
        }

        public void ShowUI()
        {
            if (_isVisible) return;
            
            _isVisible = true;
            
            if (backgroundPanel != null)
            {
                backgroundPanel.SetActive(true);
            }

            _slideTween?.Kill();
            _slideTween = panelRect.DOAnchorPos(visiblePosition, slideDuration).SetEase(slideEase);
        }

        public void HideUI()
        {
            if (!_isVisible) return;
            
            _isVisible = false;
            
            _slideTween?.Kill();
            _slideTween = panelRect.DOAnchorPos(hiddenPosition, slideDuration).SetEase(Ease.InBack).OnComplete(() => 
            {
                if (backgroundPanel != null)
                {
                    backgroundPanel.SetActive(false);
                }
            });
            
            Bus<SkillUIHoverEvent>.Raise(new SkillUIHoverEvent(null, null));
            Bus<CombatArtifactHoverEvent>.Raise(new CombatArtifactHoverEvent(null, false));
        }

        private void RefreshAllUI()
        {
            if (_currentUnit == null || _currentUnit.Data == null) return;
            
            var data = _currentUnit.Data;

            if (unitImage != null) unitImage.sprite = data.UnitImage;
            if (unitNameText != null) unitNameText.text = data.UnitName;
            if (unitClassText != null) unitClassText.text = data.UnitClass;

            if (turnSpeedText != null) turnSpeedText.text = data.turnSpeed.ToString("F1");
            if (moveSpeedText != null) moveSpeedText.text = data.MoveRange.ToString("F1");
            if (maxHealthText != null) maxHealthText.text = data.Maxhealth.ToString("F1");
            if (attackDamageText != null) attackDamageText.text = data.AttackDamage.ToString("F1");
            if (defensivePowerText != null) defensivePowerText.text = data.DefensivePower.ToString("F1");
            
            if (avoidProbabilityText != null) avoidProbabilityText.text = $"{data.AvoidProbability:F1}%";
            if (criticalProbabilityText != null) criticalProbabilityText.text = $"{data.CriticalProbability:F1}%";
            
            if (criticalDamageIncreaseText != null) criticalDamageIncreaseText.text = data.CriticalDamageIncrease.ToString("F1");
            if (maxSkillCostText != null) maxSkillCostText.text = data.MaxSkillCost.ToString();
            if (recoverySkillCostText != null) recoverySkillCostText.text = data.RecoverySkillCost.ToString();

            RefreshSkills();
            RefreshArtifacts();
        }

        private void RefreshSkills()
        {
            SkillSO[] equippedSkills = System.Array.Empty<SkillSO>();
            
            if (SkillSendManager.Instance != null && _currentUnit != null)
            {
                equippedSkills = SkillSendManager.Instance.GetEquipSkills(_currentUnit.Data.UnitType);
            }

            for (int i = 0; i < skillIcons.Count; i++)
            {
                bool hasSkill = i < equippedSkills.Length && equippedSkills[i] != null;
                
                skillIcons[i].sprite = hasSkill ? equippedSkills[i].skillUIImage : emptySkillSprite;
                
                var trigger = skillIcons[i].GetComponent<SlotHoverClickTrigger>();
                if (trigger != null)
                {
                    trigger.SetInteractable(hasSkill);
                }
            }
        }

        private void RefreshArtifacts()
        {
            var data = _currentUnit?.Data;
            
            for (int i = 0; i < artifactIcons.Count; i++)
            {
                bool hasArtifact = data != null && 
                                   data.EquippedArtifacts != null && 
                                   i < data.EquippedArtifacts.artifacts.Count && 
                                   data.EquippedArtifacts.artifacts[i] != null;
                                   
                artifactIcons[i].sprite = hasArtifact ? data.EquippedArtifacts.artifacts[i].itemIcon : emptyArtifactSprite;
                
                var trigger = artifactIcons[i].GetComponent<SlotHoverClickTrigger>();
                if (trigger != null)
                {
                    trigger.SetInteractable(hasArtifact);
                }
            }
        }

        private void SetupSkillTriggers()
        {
            for (int i = 0; i < skillIcons.Count; i++)
            {
                int index = i;
                var trigger = skillIcons[i].GetComponent<SlotHoverClickTrigger>();
                
                if (trigger == null)
                {
                    trigger = skillIcons[i].gameObject.AddComponent<SlotHoverClickTrigger>();
                }
                
                trigger.useHoverVisuals = false;
                trigger.OnHoverEnter = (pivot, triggerOffset) =>
                {
                    if (_currentUnit != null && SkillSendManager.Instance != null)
                    {
                        var equippedSkills = SkillSendManager.Instance.GetEquipSkills(_currentUnit.Data.UnitType);
                        if (index < equippedSkills.Length && equippedSkills[index] != null)
                        {
                            Bus<SkillUIHoverEvent>.Raise(new SkillUIHoverEvent(equippedSkills[index], pivot, triggerOffset));
                        }
                    }
                };
                trigger.OnHoverExit = () => Bus<SkillUIHoverEvent>.Raise(new SkillUIHoverEvent(null, null));
            }
        }

        private void SetupArtifactTriggers()
        {
            for (int i = 0; i < artifactIcons.Count; i++)
            {
                int index = i;
                var trigger = artifactIcons[i].GetComponent<SlotHoverClickTrigger>();
                
                if (trigger == null)
                {
                    trigger = artifactIcons[i].gameObject.AddComponent<SlotHoverClickTrigger>();
                }
                
                trigger.useHoverVisuals = false;
                trigger.OnHoverEnter = (pivot, triggerOffset) =>
                {
                    if (_currentUnit != null && _currentUnit.Data.EquippedArtifacts != null)
                    {
                        var artifacts = _currentUnit.Data.EquippedArtifacts.artifacts;
                        if (index < artifacts.Count && artifacts[index] != null)
                        {
                            Bus<CombatArtifactHoverEvent>.Raise(new CombatArtifactHoverEvent(artifacts[index], true));
                        }
                    }
                };
                trigger.OnHoverExit = () => Bus<CombatArtifactHoverEvent>.Raise(new CombatArtifactHoverEvent(null, false));
            }
        }
    }
}
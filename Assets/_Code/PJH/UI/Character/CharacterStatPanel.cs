using System.Collections.Generic;
using Code.Core.Events.Bus;
using Code.UnitSystem;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    public class CharacterStatPanel : Panel
    {
        [Header("Equipped Items")]
        [SerializeField] private List<Image> skillIcons;
        [SerializeField] private Sprite emptySkillSlotSprite;
        [SerializeField] private List<Image> artifactIcons;
        [SerializeField] private Sprite emptyArtifactSlotSprite;

        [Header("Hp Bar")]
        [SerializeField] private Image hpBarFill;
        [SerializeField] private TextMeshProUGUI hpText;
        [SerializeField] private float hpTweenDuration = 0.3f;

        [Header("Stat & Info")]
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI classText;
        [SerializeField] private TextMeshProUGUI atkText;
        [SerializeField] private TextMeshProUGUI defText;
        [SerializeField] private TextMeshProUGUI moveSpeedText;
        [SerializeField] private TextMeshProUGUI descriptionText;

        private UnitState _currentUnit;
        private Tween _hpTween;

        public override void Awake()
        {
            base.Awake();
            Bus<CharacterInfoEvent>.Subscribe(HandleCharacterInfo);

            for (int i = 0; i < skillIcons.Count; i++)
            {
                int index = i;
                var trigger = skillIcons[i].gameObject.AddComponent<DoubleClickTrigger>();

                trigger.CanDoubleClick = () => _currentUnit != null && 
                                               _currentUnit.Data.SkillStorage != null && 
                                               index < _currentUnit.Data.SkillStorage.skills.Count;
                trigger.OnDoubleClick = () => OpenTargetPanel("SkillPanel");
            }

            for (int i = 0; i < artifactIcons.Count; i++)
            {
                int index = i;
                var trigger = artifactIcons[i].gameObject.AddComponent<DoubleClickTrigger>();
                trigger.CanDoubleClick = () => _currentUnit != null && 
                                               _currentUnit.Data.EquippedArtifacts != null && 
                                               index < _currentUnit.Data.EquippedArtifacts.artifacts.Count;
                trigger.OnDoubleClick = () => OpenTargetPanel("ArtifactPanel");
            }
        }

        private void OnDestroy()
        {
            Bus<CharacterInfoEvent>.Unsubscribe(HandleCharacterInfo);
            UnsubscribeHpEvent();
        }

        private void OpenTargetPanel(string targetPanelId)
        {
            PanelManager.Close("StatPanel");
            PanelManager.Open(targetPanelId);
        }

        private void HandleCharacterInfo(CharacterInfoEvent evt)
        {
            UnsubscribeHpEvent();

            _currentUnit = evt.Unit;
            if (_currentUnit != null)
            {
                _currentUnit.CurrentHp.OnValueChanged += RefreshHpBar;
                if (IsOpen) RefreshAllUI();
            }
        }

        public override void Open()
        {
            base.Open();
            if (_currentUnit != null) RefreshAllUI();
        }

        private void UnsubscribeHpEvent()
        {
            if (_currentUnit != null)
                _currentUnit.CurrentHp.OnValueChanged -= RefreshHpBar;
        }

        private void RefreshAllUI()
        {
            RefreshInfoTexts();
            RefreshHpBar(0f, _currentUnit.CurrentHp.Value);
            RefreshSkillSlots();
            RefreshArtifactSlots();
        }

        private void RefreshInfoTexts()
        {
            var data = _currentUnit.Data;
            
            nameText.text = data.UnitName;
            classText.text = data.UnitClass;
            atkText.text = data.AtkDamage.ToString("F1");
            defText.text = data.DefensivePower.ToString("F1");
            moveSpeedText.text = data.MoveSpeed.ToString("F1");
            descriptionText.text = data.UnitDescription;
        }

        private void RefreshHpBar(float prevValue, float nextValue)
        {
            float maxHp = _currentUnit.Data.Maxhealth;
            hpText.text = $"{nextValue:F0} / {maxHp:F0}";

            float fillAmount = maxHp > 0 ? nextValue / maxHp : 0f;

            _hpTween?.Kill();
            _hpTween = hpBarFill
                .DOFillAmount(fillAmount, hpTweenDuration)
                .SetEase(Ease.OutCubic);
        }

        private void RefreshSkillSlots()
        {
            var data = _currentUnit.Data;
            
            for (int i = 0; i < skillIcons.Count; i++)
            {
                if (data.SkillStorage != null && i < data.SkillStorage.skills.Count)
                {
                    skillIcons[i].sprite = data.SkillStorage.skills[i].skillUIImage;
                    skillIcons[i].color = Color.white;
                }
                else
                {
                    skillIcons[i].sprite = emptySkillSlotSprite;
                }
            }
        }

        private void RefreshArtifactSlots()
        {
            var data = _currentUnit.Data;

            for (int i = 0; i < artifactIcons.Count; i++)
            {
                if (data.EquippedArtifacts != null && i < data.EquippedArtifacts.artifacts.Count)
                {
                    artifactIcons[i].sprite = data.EquippedArtifacts.artifacts[i].artifactIcon;
                    artifactIcons[i].color = Color.white;
                }
                else
                {
                    artifactIcons[i].sprite = emptyArtifactSlotSprite;
                }
            }
        }
    }
}
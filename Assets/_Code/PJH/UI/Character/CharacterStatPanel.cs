using System.Collections.Generic;
using Code.Core.Events.Bus;
using Code.UnitSystem;
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

        [Header("HP Bar")]
        [SerializeField] private Image hpBarFill;
        [SerializeField] private TextMeshProUGUI hpText;

        [Header("Stat & Info")]
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI classText;
        [SerializeField] private TextMeshProUGUI atkText;
        [SerializeField] private TextMeshProUGUI defText;
        [SerializeField] private TextMeshProUGUI moveSpeedText;
        [SerializeField] private TextMeshProUGUI descriptionText;

        private UnitState _currentUnit;

        public override void Awake()
        {
            base.Awake();
            Bus<CharacterInfoEvent>.Subscribe(HandleCharacterInfo);

            for (int i = 0; i < skillIcons.Count; i++)
            {
                var trigger = skillIcons[i].gameObject.AddComponent<SlotHoverClickTrigger>();
                trigger.useHoverVisuals = false;
                trigger.OnClick = () => OpenTargetPanel("SkillPanel");
            }

            for (int i = 0; i < artifactIcons.Count; i++)
            {
                int index = i;
                var trigger = artifactIcons[i].gameObject.AddComponent<SlotHoverClickTrigger>();
                trigger.useHoverVisuals = false;
                
                trigger.OnClick = () => OpenTargetPanel("ArtifactPanel");
                trigger.OnHoverEnter = (pos) =>
                {
                    if (_currentUnit != null && _currentUnit.Data.EquippedArtifacts != null && index < _currentUnit.Data.EquippedArtifacts.artifacts.Count)
                    {
                        var artifact = _currentUnit.Data.EquippedArtifacts.artifacts[index];
                        Bus<ArtifactPopupEvent>.Raise(new ArtifactPopupEvent(artifact, true, pos, true));
                    }
                };
                trigger.OnHoverExit = () => Bus<ArtifactPopupEvent>.Raise(new ArtifactPopupEvent(null, false, Vector2.zero, true));
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
            hpBarFill.fillAmount = maxHp > 0 ? nextValue / maxHp : 0f;
        }

        private void RefreshSkillSlots()
        {
            var data = _currentUnit.Data;
            
            for (int i = 0; i < skillIcons.Count; i++)
            {
                var trigger = skillIcons[i].GetComponent<SlotHoverClickTrigger>();
                bool hasSkill = data.SkillStorage != null && i < data.SkillStorage.skills.Count;

                if (hasSkill)
                {
                    skillIcons[i].sprite = data.SkillStorage.skills[i].skillUIImage;
                }
                else
                {
                    skillIcons[i].sprite = emptySkillSlotSprite;
                }
                
                if (trigger != null) trigger.SetInteractable(hasSkill);
            }
        }

        private void RefreshArtifactSlots()
        {
            var data = _currentUnit.Data;

            for (int i = 0; i < artifactIcons.Count; i++)
            {
                var trigger = artifactIcons[i].GetComponent<SlotHoverClickTrigger>();
                bool hasArtifact = data.EquippedArtifacts != null && i < data.EquippedArtifacts.artifacts.Count;

                if (hasArtifact)
                {
                    artifactIcons[i].sprite = data.EquippedArtifacts.artifacts[i].artifactIcon;
                }
                else
                {
                    artifactIcons[i].sprite = emptyArtifactSlotSprite;
                }
                
                if (trigger != null) trigger.SetInteractable(hasArtifact);
            }
        }
    }
}
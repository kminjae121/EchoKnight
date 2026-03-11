using System.Collections.Generic;
using Code.Core.Events.Bus;
using Code.UnitSystem;
using Code.UnitSystem.SkillSystem;
using GondrLib.Dependencies;
using GondrLib.ObjectPool.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    public class CharacterSkillPanel : Panel
    {
        [Header("Pool Settings")]
        [SerializeField] private PoolingItemSO skillButtonPoolingSO;

        [Header("Containers")]
        [SerializeField] private Transform ownSkillContainer;

        [Header("Detail Settings")]
        [SerializeField] private Image IconImage; 
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI descText;
        [SerializeField] private TextMeshProUGUI costText;
        [SerializeField] private TextMeshProUGUI damageText;
        [SerializeField] private TextMeshProUGUI rangeText;

        [Inject] private PoolManagerMono _poolManager;
        
        private UnitSO _unit;
        private List<CharacterSkillButton> _activeButtons = new();
        private SkillSO _selectedSkill;

        public override void Awake()
        {
            base.Awake();

            if (_poolManager == null)
                _poolManager = FindFirstObjectByType<PoolManagerMono>();

            Bus<CharacterInfoEvent>.Subscribe(HandleCharacterInfo);
            Bus<SkillEquipEvent>.Subscribe(HandleSkillEquipped);
            Bus<SkillUnequipEvent>.Subscribe(HandleSkillUnequipped);
            Bus<SkillDetailSelectEvent>.Subscribe(HandleSkillSelect);
        }

        private void OnDestroy()
        {
            Bus<CharacterInfoEvent>.Unsubscribe(HandleCharacterInfo);
            Bus<SkillEquipEvent>.Unsubscribe(HandleSkillEquipped);
            Bus<SkillUnequipEvent>.Unsubscribe(HandleSkillUnequipped);
            Bus<SkillDetailSelectEvent>.Unsubscribe(HandleSkillSelect);
        }

        public override void Open()
        {
            base.Open();
            if (_unit != null)
            {
                RefreshSkillList();
                RefreshDetail();
            }
        }

        private void HandleCharacterInfo(CharacterInfoEvent evt)
        {
            _unit = evt.Unit.Data;
            _selectedSkill = null;
            if (IsOpen)
            {
                RefreshSkillList();
                RefreshDetail();
            }
        }

        private void HandleSkillSelect(SkillDetailSelectEvent evt)
        {
            _selectedSkill = evt.Skill;
            RefreshDetail();
        }

        private void RefreshSkillList()
        {
            if (_unit == null || _unit.OwnSkillStorage == null) return;

            foreach (var btn in _activeButtons) btn.ReturnToPool();
            _activeButtons.Clear();

            foreach (var skillSO in _unit.OwnSkillStorage.skills)
            {
                var btn = _poolManager.Pop<CharacterSkillButton>(skillButtonPoolingSO);
                btn.transform.SetParent(ownSkillContainer);
                btn.transform.SetAsLastSibling();
                btn.transform.localScale = Vector3.one;

                bool isEquipped = _unit.SkillStorage != null && _unit.SkillStorage.skills.Contains(skillSO);
                btn.SetSkill(skillSO, isEquipped);
                
                _activeButtons.Add(btn);
            }
        }

        private void RefreshDetail()
        {
            bool hasSkill = _selectedSkill != null;

            if (IconImage != null) IconImage.gameObject.SetActive(hasSkill);
            if (nameText != null) nameText.gameObject.SetActive(hasSkill);
            if (descText != null) descText.gameObject.SetActive(hasSkill);
            if (costText != null) costText.gameObject.SetActive(hasSkill);
            if (damageText != null) damageText.gameObject.SetActive(hasSkill);
            if (rangeText != null) rangeText.gameObject.SetActive(hasSkill);

            if (!hasSkill) return;

            if (IconImage != null) IconImage.sprite = _selectedSkill.skillUIImage;
            if (nameText != null) nameText.text = _selectedSkill.skillName;
            if (descText != null) descText.text = _selectedSkill.SkillDescription;
            if (costText != null) costText.text = $"{_selectedSkill.SkillCost}";
            if (damageText != null) damageText.text = $"{_selectedSkill.SkillDamage}";
            if (rangeText != null) rangeText.text = $"{_selectedSkill.SkillRange}";
        }

        private void HandleSkillEquipped(SkillEquipEvent evt)
        {
            if (_unit == null || _unit.SkillStorage == null) return;
            if (!_unit.SkillStorage.skills.Contains(evt.Skill))
            {
                _unit.SkillStorage.skills.Add(evt.Skill);
                if (IsOpen) RefreshSkillList();
            }
        }

        private void HandleSkillUnequipped(SkillUnequipEvent evt)
        {
            if (_unit == null || _unit.SkillStorage == null) return;
            if (_unit.SkillStorage.skills.Remove(evt.Skill))
            {
                if (IsOpen) RefreshSkillList();
            }
        }
    }
}
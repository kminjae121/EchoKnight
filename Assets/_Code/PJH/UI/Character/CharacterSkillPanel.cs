using System.Collections.Generic;
using System.Linq;
using Code.Core.Events.Bus;
using Code.UnitSystem.SkillSystem;
using DG.Tweening;
using GondrLib.Dependencies;
using GondrLib.ObjectPool.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    public class CharacterSkillPanel : Panel
    {
        [Header("Database")]
        [SerializeField] private List<SkillSO> allSkillsDatabase;

        [Header("Pool Settings")]
        [SerializeField] private PoolingItemSO skillButtonPoolingSO;

        [Header("Loadout Area")]
        [SerializeField] private Image loadoutFillImage;
        [SerializeField] private TextMeshProUGUI loadoutCostText;
        [SerializeField] private int maxEquipCount = 4;
        [SerializeField] private float loadoutTweenTime = 0.3f;

        [Header("List Area")]
        [SerializeField] private Transform skillTrm;

        [Header("Detail Area")]
        [SerializeField] private Image detailIcon;
        [SerializeField] private TextMeshProUGUI detailNameText;
        [SerializeField] private TextMeshProUGUI detailDescText;
        [SerializeField] private TextMeshProUGUI detailCostText;
        [SerializeField] private TextMeshProUGUI detailDamageText;
        [SerializeField] private TextMeshProUGUI detailRangeText;

        [Inject] private PoolManagerMono _poolManager;
        
        private UnitSO _unit;
        private List<CharacterSkillButton> _activeButtons = new();
        private Tween _loadoutTween;

        public override void Awake()
        {
            base.Awake();

            if (_poolManager == null)
                _poolManager = FindFirstObjectByType<PoolManagerMono>();

            Bus<CharacterInfoEvent>.Subscribe(HandleCharacterInfo);
            Bus<SkillEquipEvent>.Subscribe(SkillEquip);
            Bus<SkillUnequipEvent>.Subscribe(SkillUnequip);
            Bus<SkillDetailSelectEvent>.Subscribe(HandleDetailSelect);
        }

        private void OnDestroy()
        {
            Bus<CharacterInfoEvent>.Unsubscribe(HandleCharacterInfo);
            Bus<SkillEquipEvent>.Unsubscribe(SkillEquip);
            Bus<SkillUnequipEvent>.Unsubscribe(SkillUnequip);
            Bus<SkillDetailSelectEvent>.Unsubscribe(HandleDetailSelect);
            
            _loadoutTween?.Kill();
        }

        public override void Open()
        {
            base.Open();
            
            if (_unit != null)
            {
                RefreshUI();
                ClearDetailArea();
            }
        }
        
        private void HandleCharacterInfo(CharacterInfoEvent evt)
        {
            _unit = evt.Unit.Data;

            if (IsOpen)
            {
                RefreshUI();
                ClearDetailArea();
            }
        }
        
        private void RefreshUI()
        {
            foreach (var btn in _activeButtons)
                btn.ReturnToPool();
                
            _activeButtons.Clear();

            if (_poolManager == null)
            {
                Debug.LogWarning("풀 매니저를 찾을 수 없습니다.");
                return;
            }

            if (_unit == null)
                return;

            if (allSkillsDatabase == null || allSkillsDatabase.Count == 0)
            {
                Debug.LogWarning("스킬 데이터베이스가 비어있습니다. 인스펙터에서 스킬들을 연결해주세요.");
                return;
            }

            var matchingSkills = allSkillsDatabase.Where(skill => skill.unitType == _unit.UnitType).ToList();

            foreach (var skill in matchingSkills)
            {
                var skillButton = _poolManager.Pop<CharacterSkillButton>(skillButtonPoolingSO);
                skillButton.transform.SetParent(skillTrm);
                skillButton.transform.localScale = Vector3.one;
                
                bool isEquipped = false;
                if (_unit.SkillStorage != null && _unit.SkillStorage.skills != null)
                {
                    isEquipped = _unit.SkillStorage.skills.Contains(skill);
                }
                
                skillButton.SetSkill(skill, isEquipped);
                _activeButtons.Add(skillButton);
            }
            
            RefreshLoadout();
        }

        private void RefreshLoadout()
        {
            int currentCost = GetCurrentCost();
            loadoutCostText.text = $"{currentCost} / {_unit.Cost}";

            float fillValue = _unit.Cost > 0 ? (float)currentCost / _unit.Cost : 0f;

            _loadoutTween?.Kill();
            _loadoutTween = loadoutFillImage
                .DOFillAmount(fillValue, loadoutTweenTime)
                .SetEase(Ease.OutCubic);
        }
        
        private void SkillEquip(SkillEquipEvent evt)
        {
            if (_unit.SkillStorage == null || _unit.SkillStorage.skills == null)
                return;

            if (_unit.SkillStorage.skills.Contains(evt.Skill))
                return;
            
            if (_unit.SkillStorage.skills.Count >= maxEquipCount)
            {
                Bus<ShowMessageUIEvent>.Raise(new ShowMessageUIEvent("스킬은 최대 4개까지만 장착할 수 있습니다."));
                return;
            }
            
            if (GetCurrentCost() + evt.Skill.SkillCost > _unit.Cost)
            {
                Bus<ShowMessageUIEvent>.Raise(new ShowMessageUIEvent("코스트를 초과하여 스킬을 장착할 수 없습니다."));
                return;
            }
            
            _unit.SkillStorage.skills.Add(evt.Skill);
            Bus<SkillEquippedEvent>.Raise(new SkillEquippedEvent(evt.Skill));
            
            RefreshUI();
        }

        private void SkillUnequip(SkillUnequipEvent evt)
        {
            if (_unit.SkillStorage == null || _unit.SkillStorage.skills == null)
                return;

            if (_unit.SkillStorage.skills.Remove(evt.Skill))
            {
                Bus<SkillUnequippedEvent>.Raise(new SkillUnequippedEvent(evt.Skill));
                RefreshUI();
            }
        }

        private void HandleDetailSelect(SkillDetailSelectEvent evt)
        {
            var skill = evt.Skill;
            if (skill == null)
                return;

            detailIcon.sprite = skill.skillUIImage;
            detailIcon.color = Color.white;
            detailNameText.text = skill.skillName;
            detailDescText.text = skill.SkillDescription;
            detailCostText.text = skill.SkillCost.ToString();
            detailDamageText.text = skill.SkillDamage.ToString("F1");
            detailRangeText.text = skill.SkillRange.ToString("F1");
        }

        private void ClearDetailArea()
        {
            detailIcon.color = Color.clear;
            detailNameText.text = string.Empty;
            detailDescText.text = string.Empty;
            detailCostText.text = string.Empty;
            detailDamageText.text = string.Empty;
            detailRangeText.text = string.Empty;
        }
        
        private int GetCurrentCost()
        {
            if (_unit == null || _unit.SkillStorage == null || _unit.SkillStorage.skills == null)
                return 0;
                
            return _unit.SkillStorage.skills.Sum(skill => skill.SkillCost);
        }
    }
}
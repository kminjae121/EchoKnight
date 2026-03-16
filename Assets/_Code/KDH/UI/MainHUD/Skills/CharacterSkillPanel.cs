using System.Collections;
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

        [Header("Loadout Settings")]
        [SerializeField] private Image loadoutFillImage;
        [SerializeField] private TextMeshProUGUI loadoutText;
        [SerializeField] private float fillAnimationDuration = 0.3f;

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
        private Coroutine _fillCoroutine;

        public override void Awake()
        {
            base.Awake();

            Bus<CharacterInfoEvent>.Subscribe(HandleCharacterInfo);
            Bus<SkillEquipEvent>.Subscribe(HandleSkillEquipped);
            Bus<SkillUnequipEvent>.Subscribe(HandleSkillUnequipped);
            Bus<SkillDetailSelectEvent>.Subscribe(HandleSkillSelect);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
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
                RefreshLoadoutUI(true);
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
                RefreshLoadoutUI(true);
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

        private void RefreshLoadoutUI(bool instant = false)
        {
            if (_unit == null) return;

            int currentCost = GetCurrentLoadoutCost();
            int maxCost = _unit.LoadOutCost;

            if (loadoutText != null)
            {
                loadoutText.text = $"{currentCost} / {maxCost}";
            }

            if (loadoutFillImage != null)
            {
                loadoutFillImage.type = Image.Type.Filled;
                loadoutFillImage.fillMethod = Image.FillMethod.Vertical;
                loadoutFillImage.fillOrigin = (int)Image.OriginVertical.Bottom;
                
                float targetFillAmount = maxCost > 0 ? (float)currentCost / maxCost : 0f;

                if (instant || !gameObject.activeInHierarchy)
                {
                    loadoutFillImage.fillAmount = targetFillAmount;
                    if (_fillCoroutine != null)
                    {
                        StopCoroutine(_fillCoroutine);
                        _fillCoroutine = null;
                    }
                }
                else
                {
                    if (_fillCoroutine != null)
                    {
                        StopCoroutine(_fillCoroutine);
                    }
                    _fillCoroutine = StartCoroutine(CoSmoothFill(targetFillAmount));
                }
            }
        }

        private IEnumerator CoSmoothFill(float targetAmount)
        {
            float startAmount = loadoutFillImage.fillAmount;
            float elapsedTime = 0f;

            while (elapsedTime < fillAnimationDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / fillAnimationDuration;
                
                t = t * t * (3f - 2f * t);

                loadoutFillImage.fillAmount = Mathf.Lerp(startAmount, targetAmount, t);
                yield return null;
            }

            loadoutFillImage.fillAmount = targetAmount;
            _fillCoroutine = null;
        }

        private int GetCurrentLoadoutCost()
        {
            int totalCost = 0;
            
            if (_unit.SkillStorage != null)
            {
                foreach (var skill in _unit.SkillStorage.skills)
                {
                    if (skill != null)
                    {
                        totalCost += skill.SkillCost;
                    }
                }
            }
            
            return totalCost;
        }

        private void HandleSkillEquipped(SkillEquipEvent evt)
        {
            if (_unit == null || _unit.SkillStorage == null) return;

            if (_unit.SkillStorage.skills.Contains(evt.Skill)) return;

            if (_unit.SkillStorage.skills.Count >= 4)
            {
                Debug.LogWarning("스킬은 최대 4개까지만 장착할 수 있습니다.");
                return;
            }

            int currentCost = GetCurrentLoadoutCost();
            
            if (currentCost + evt.Skill.SkillCost > _unit.LoadOutCost)
            {
                Debug.LogWarning("스킬 코스트 총량을 초과하여 장착할 수 없습니다.");
                return;
            }

            _unit.SkillStorage.skills.Add(evt.Skill);
            
            if (IsOpen)
            {
                RefreshSkillList();
                RefreshLoadoutUI();
            }
        }

        private void HandleSkillUnequipped(SkillUnequipEvent evt)
        {
            if (_unit == null || _unit.SkillStorage == null) return;
            
            if (_unit.SkillStorage.skills.Remove(evt.Skill))
            {
                if (IsOpen)
                {
                    RefreshSkillList();
                    RefreshLoadoutUI();
                }
            }
        }
    }
}
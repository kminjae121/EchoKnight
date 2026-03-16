using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Code.Core.Events.Bus;
using Code.UnitSystem.ArtifactSystem;
using Code.UnitSystem.SkillSystem;
using GondrLib.Dependencies;
using GondrLib.ObjectPool.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    public class CharacterEquipmentPanel : Panel
    {
        [Header("Pool Settings")]
        [SerializeField] private PoolingItemSO artifactButtonPoolingSO;
        [SerializeField] private PoolingItemSO skillButtonPoolingSO;

        [Header("Artifact Inventory Area")]
        [SerializeField] private Transform artifactInventoryTrm; 
        [SerializeField] private TextMeshProUGUI artifactCountText;
        [SerializeField] private Button artifactSortButton;
        [SerializeField] private int maxArtifactInventoryCapacity = 20;

        [Header("Artifact Equipped Area")]
        [SerializeField] private List<Image> equippedArtifactSlotImages; 
        [SerializeField] private Sprite emptyArtifactSlotSprite;
        [SerializeField] private int maxArtifactEquipCount = 2;

        [Header("Skill Containers")]
        [SerializeField] private Transform ownSkillContainer;

        [Header("Skill Loadout Settings")]
        [SerializeField] private Image skillLoadoutFillImage;
        [SerializeField] private TextMeshProUGUI skillLoadoutText;
        [SerializeField] private float fillAnimationDuration = 0.3f;

        [Header("Skill Detail Settings")]
        [SerializeField] private Image skillIconImage; 
        [SerializeField] private TextMeshProUGUI skillNameText;
        [SerializeField] private TextMeshProUGUI skillDescText;
        [SerializeField] private TextMeshProUGUI skillCostText;
        [SerializeField] private TextMeshProUGUI skillDamageText;
        [SerializeField] private TextMeshProUGUI skillRangeText;

        [Inject] private PoolManagerMono _poolManager;
        
        private UnitSO _unit;
        private List<ArtifactButton> _activeArtifactButtons = new();
        private List<CharacterSkillButton> _activeSkillButtons = new();
        private SkillSO _selectedSkill;
        private bool _isArtifactSortedByRarity = false; 
        private Coroutine _fillCoroutine;

        public override void Awake()
        {
            base.Awake();

            if (_poolManager == null)
                _poolManager = FindFirstObjectByType<PoolManagerMono>();

            if (artifactSortButton != null)
                artifactSortButton.onClick.AddListener(ToggleArtifactSort);

            Bus<CharacterInfoEvent>.Subscribe(HandleCharacterInfo);
            Bus<ArtifactEquipEvent>.Subscribe(HandleArtifactEquip);
            Bus<ArtifactUnequipEvent>.Subscribe(HandleArtifactUnequip);
            Bus<SkillEquipEvent>.Subscribe(HandleSkillEquipped);
            Bus<SkillUnequipEvent>.Subscribe(HandleSkillUnequipped);
            Bus<SkillDetailSelectEvent>.Subscribe(HandleSkillSelect);

            for (int i = 0; i < equippedArtifactSlotImages.Count; i++)
            {
                int index = i;
                var trigger = equippedArtifactSlotImages[i].gameObject.AddComponent<SlotHoverClickTrigger>();
                trigger.useHoverVisuals = false;
                trigger.OnRightClick = (pos) =>
                {
                    if (_unit != null && _unit.EquippedArtifacts != null && index < _unit.EquippedArtifacts.artifacts.Count)
                    {
                        var artifact = _unit.EquippedArtifacts.artifacts[index];
                        Bus<ArtifactPopupEvent>.Raise(new ArtifactPopupEvent(artifact, true, pos));
                    }
                };
            }
        }

        private void OnDestroy()
        {
            if (artifactSortButton != null)
                artifactSortButton.onClick.RemoveListener(ToggleArtifactSort);

            Bus<CharacterInfoEvent>.Unsubscribe(HandleCharacterInfo);
            Bus<ArtifactEquipEvent>.Unsubscribe(HandleArtifactEquip);
            Bus<ArtifactUnequipEvent>.Unsubscribe(HandleArtifactUnequip);
            Bus<SkillEquipEvent>.Unsubscribe(HandleSkillEquipped);
            Bus<SkillUnequipEvent>.Unsubscribe(HandleSkillUnequipped);
            Bus<SkillDetailSelectEvent>.Unsubscribe(HandleSkillSelect);
        }

        public override void Open()
        {
            base.Open();
            if (_unit != null)
            {
                RefreshArtifactUI();
                RefreshSkillList();
                RefreshSkillDetail();
                RefreshSkillLoadoutUI(true);
            }
        }

        private void HandleCharacterInfo(CharacterInfoEvent evt)
        {
            _unit = evt.Unit.Data;
            _selectedSkill = null;
            if (IsOpen)
            {
                RefreshArtifactUI();
                RefreshSkillList();
                RefreshSkillDetail();
                RefreshSkillLoadoutUI(true);
            }
        }

        #region Artifact Logic

        private void ToggleArtifactSort()
        {
            _isArtifactSortedByRarity = !_isArtifactSortedByRarity;
            RefreshArtifactUI();
        }

        private void RefreshArtifactUI()
        {
            if (_unit == null || _unit.OwnArtifactStorage == null) return;

            int currentCount = _unit.OwnArtifactStorage.artifacts.Count;
            if (artifactCountText != null)
                artifactCountText.text = $"{currentCount}/{maxArtifactInventoryCapacity}";

            foreach (var btn in _activeArtifactButtons) btn.ReturnToPool();
            _activeArtifactButtons.Clear();

            var displayList = _unit.OwnArtifactStorage.artifacts
                .Where(a => _unit.EquippedArtifacts == null || !_unit.EquippedArtifacts.artifacts.Contains(a))
                .ToList();

            if (_isArtifactSortedByRarity)
            {
                displayList = displayList.OrderByDescending(a => a.rarity).ToList();
            }

            foreach (var artifact in displayList)
            {
                var btn = _poolManager.Pop<ArtifactButton>(artifactButtonPoolingSO);
                btn.transform.SetParent(artifactInventoryTrm);
                btn.transform.SetAsLastSibling(); 
                btn.transform.localScale = Vector3.one;

                btn.SetArtifact(artifact, false);
                _activeArtifactButtons.Add(btn);
            }

            RefreshEquippedArtifactSlots();
        }

        private void RefreshEquippedArtifactSlots()
        {
            var equippedList = _unit.EquippedArtifacts?.artifacts ?? new List<ArtifactSO>();

            for (int i = 0; i < equippedArtifactSlotImages.Count; i++)
            {
                var trigger = equippedArtifactSlotImages[i].GetComponent<SlotHoverClickTrigger>();
                bool hasArtifact = i < equippedList.Count;

                if (hasArtifact)
                {
                    equippedArtifactSlotImages[i].sprite = equippedList[i].artifactIcon;
                }
                else
                {
                    equippedArtifactSlotImages[i].sprite = emptyArtifactSlotSprite;
                }

                if (trigger != null) trigger.SetInteractable(hasArtifact);
            }
        }

        private void HandleArtifactEquip(ArtifactEquipEvent evt)
        {
            if (_unit == null || _unit.EquippedArtifacts == null) return;
            if (_unit.EquippedArtifacts.artifacts.Contains(evt.Artifact)) return;

            if (_unit.EquippedArtifacts.artifacts.Count >= maxArtifactEquipCount)
            {
                Bus<ShowMessageUIEvent>.Raise(new ShowMessageUIEvent($"아티팩트는 최대 {maxArtifactEquipCount}개까지만 장착할 수 있습니다."));
                return;
            }

            _unit.EquippedArtifacts.artifacts.Add(evt.Artifact);
            RefreshArtifactUI();
        }

        private void HandleArtifactUnequip(ArtifactUnequipEvent evt)
        {
            if (_unit == null || _unit.EquippedArtifacts == null) return;
            
            if (_unit.EquippedArtifacts.artifacts.Remove(evt.Artifact))
            {
                RefreshArtifactUI();
            }
        }

        #endregion

        #region Skill Logic

        private void HandleSkillSelect(SkillDetailSelectEvent evt)
        {
            _selectedSkill = evt.Skill;
            RefreshSkillDetail();
        }

        private void RefreshSkillList()
        {
            if (_unit == null || _unit.OwnSkillStorage == null) return;

            foreach (var btn in _activeSkillButtons) btn.ReturnToPool();
            _activeSkillButtons.Clear();

            foreach (var skillSO in _unit.OwnSkillStorage.skills)
            {
                var btn = _poolManager.Pop<CharacterSkillButton>(skillButtonPoolingSO);
                btn.transform.SetParent(ownSkillContainer);
                btn.transform.SetAsLastSibling();
                btn.transform.localScale = Vector3.one;

                bool isEquipped = _unit.SkillStorage != null && _unit.SkillStorage.skills.Contains(skillSO);
                btn.SetSkill(skillSO, isEquipped);
                
                _activeSkillButtons.Add(btn);
            }
        }

        private void RefreshSkillDetail()
        {
            bool hasSkill = _selectedSkill != null;

            if (skillIconImage != null) skillIconImage.gameObject.SetActive(hasSkill);
            if (skillNameText != null) skillNameText.gameObject.SetActive(hasSkill);
            if (skillDescText != null) skillDescText.gameObject.SetActive(hasSkill);
            if (skillCostText != null) skillCostText.gameObject.SetActive(hasSkill);
            if (skillDamageText != null) skillDamageText.gameObject.SetActive(hasSkill);
            if (skillRangeText != null) skillRangeText.gameObject.SetActive(hasSkill);

            if (!hasSkill) return;

            if (skillIconImage != null) skillIconImage.sprite = _selectedSkill.skillUIImage;
            if (skillNameText != null) skillNameText.text = _selectedSkill.skillName;
            if (skillDescText != null) skillDescText.text = _selectedSkill.SkillDescription;
            if (skillCostText != null) skillCostText.text = $"{_selectedSkill.SkillCost}";
            if (skillDamageText != null) skillDamageText.text = $"{_selectedSkill.SkillDamage}";
            if (skillRangeText != null) skillRangeText.text = $"{_selectedSkill.SkillRange}";
        }

        private void RefreshSkillLoadoutUI(bool instant = false)
        {
            if (_unit == null) return;

            int currentCost = GetCurrentSkillLoadoutCost();
            int maxCost = _unit.LoadOutCost;

            if (skillLoadoutText != null)
            {
                skillLoadoutText.text = $"{currentCost} / {maxCost}";
            }

            if (skillLoadoutFillImage != null)
            {
                skillLoadoutFillImage.type = Image.Type.Filled;
                skillLoadoutFillImage.fillMethod = Image.FillMethod.Vertical;
                skillLoadoutFillImage.fillOrigin = (int)Image.OriginVertical.Bottom;
                
                float targetFillAmount = maxCost > 0 ? (float)currentCost / maxCost : 0f;

                if (instant || !gameObject.activeInHierarchy)
                {
                    skillLoadoutFillImage.fillAmount = targetFillAmount;
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
            if (skillLoadoutFillImage == null) yield break;

            float startAmount = skillLoadoutFillImage.fillAmount;
            float elapsedTime = 0f;

            while (elapsedTime < fillAnimationDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / fillAnimationDuration;
                t = t * t * (3f - 2f * t);

                skillLoadoutFillImage.fillAmount = Mathf.Lerp(startAmount, targetAmount, t);
                yield return null;
            }

            skillLoadoutFillImage.fillAmount = targetAmount;
            _fillCoroutine = null;
        }

        private int GetCurrentSkillLoadoutCost()
        {
            int totalCost = 0;
            if (_unit.SkillStorage != null)
            {
                foreach (var skill in _unit.SkillStorage.skills)
                {
                    if (skill != null) totalCost += skill.SkillCost;
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

            int currentCost = GetCurrentSkillLoadoutCost();
            if (currentCost + evt.Skill.SkillCost > _unit.LoadOutCost)
            {
                Debug.LogWarning("스킬 코스트 총량을 초과하여 장착할 수 없습니다.");
                return;
            }

            _unit.SkillStorage.skills.Add(evt.Skill);
            if (IsOpen)
            {
                RefreshSkillList();
                RefreshSkillLoadoutUI();
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
                    RefreshSkillLoadoutUI();
                }
            }
        }

        #endregion
    }
}
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Code.Core.Events.Bus;
using Code.Core.Managers;
using Code.Items;
using Code.UnitSystem.ArtifactSystem;
using Code.SkillSystem;
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
        [SerializeField] private TextMeshProUGUI artifactSortText;
        [SerializeField] private int maxArtifactInventoryCapacity = 20;

        [Header("Artifact Equipped Area")]
        [SerializeField] private List<Image> equippedArtifactSlotImages; 
        [SerializeField] private List<Image> equippedArtifactRarityImages;
        [SerializeField] private Sprite emptyArtifactSlotSprite;
        [SerializeField] private int maxArtifactEquipCount = 2;

        [Header("Skill Containers")]
        [SerializeField] private Transform ownSkillContainer;

        [Header("Skill Loadout Settings")]
        [SerializeField] private Image skillLoadoutFillImage;
        [SerializeField] private TextMeshProUGUI skillLoadoutText;
        [SerializeField] private float fillAnimationDuration = 0.3f;

        [Inject] private PoolManagerMono _poolManager;
        
        private UnitSO _unit;
        private List<ArtifactButton> _activeArtifactButtons = new();
        private List<CharacterSkillButton> _activeSkillButtons = new();
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

            Vector2 defaultArtifactOffset = Vector2.zero;
            if (artifactButtonPoolingSO != null && artifactButtonPoolingSO.prefab != null)
            {
                var btn = artifactButtonPoolingSO.prefab.GetComponent<ArtifactButton>();
                if (btn != null) defaultArtifactOffset = btn.EquippedPopupOffset;
            }

            for (int i = 0; i < equippedArtifactSlotImages.Count; i++)
            {
                int index = i;
                var trigger = equippedArtifactSlotImages[i].GetComponent<SlotHoverClickTrigger>();
                
                if (trigger == null)
                {
                    trigger = equippedArtifactSlotImages[i].gameObject.AddComponent<SlotHoverClickTrigger>();
                }

                trigger.useHoverVisuals = false;
                
                trigger.OnLeftClick = (pivot, triggerOffset) =>
                {
                    if (_unit != null && _unit.EquippedArtifacts != null && index < _unit.EquippedArtifacts.artifacts.Count)
                    {
                        var artifact = _unit.EquippedArtifacts.artifacts[index];
                        Vector2 finalOffset = triggerOffset != Vector2.zero ? triggerOffset : defaultArtifactOffset;
                        Bus<ArtifactPopupEvent>.Raise(new ArtifactPopupEvent(artifact, true, pivot, finalOffset));
                    }
                };
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            if (artifactSortButton != null)
                artifactSortButton.onClick.RemoveListener(ToggleArtifactSort);

            Bus<CharacterInfoEvent>.Unsubscribe(HandleCharacterInfo);
            Bus<ArtifactEquipEvent>.Unsubscribe(HandleArtifactEquip);
            Bus<ArtifactUnequipEvent>.Unsubscribe(HandleArtifactUnequip);
            Bus<SkillEquipEvent>.Unsubscribe(HandleSkillEquipped);
            Bus<SkillUnequipEvent>.Unsubscribe(HandleSkillUnequipped);
        }

        public override void Open()
        {
            base.Open();
            if (_unit != null)
            {
                if (SkillSendManager.Instance != null)
                    SkillSendManager.Instance.SyncEquippedSkills(_unit);

                RefreshArtifactUI();
                RefreshSkillList();
                RefreshSkillLoadoutUI(true);
            }
        }

        private void HandleCharacterInfo(CharacterInfoEvent evt)
        {
            _unit = evt.Unit.Data;
            
            if (_unit != null && SkillSendManager.Instance != null)
                SkillSendManager.Instance.SyncEquippedSkills(_unit);

            if (IsOpen)
            {
                RefreshArtifactUI();
                RefreshSkillList();
                RefreshSkillLoadoutUI(true);
            }
        }

        private void ToggleArtifactSort()
        {
            _isArtifactSortedByRarity = !_isArtifactSortedByRarity;
            if (artifactSortText != null) artifactSortText.text = _isArtifactSortedByRarity ? "희귀도순" : "획득순";
            RefreshArtifactUI();
        }

        private void RefreshArtifactUI()
        {
            if (_unit == null || _unit.OwnArtifactStorage == null) return;

            int currentCount = _unit.OwnArtifactStorage.artifacts.Count;
            if (artifactCountText != null) artifactCountText.text = $"{currentCount}/{maxArtifactInventoryCapacity}";

            foreach (var btn in _activeArtifactButtons) btn.ReturnToPool();
            _activeArtifactButtons.Clear();

            var displayList = _unit.OwnArtifactStorage.artifacts
                .Where(a => _unit.EquippedArtifacts == null || !_unit.EquippedArtifacts.artifacts.Contains(a))
                .ToList();

            if (_isArtifactSortedByRarity) displayList = displayList.OrderByDescending(a => a.rarity).ToList();

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
            var equippedList = _unit.EquippedArtifacts?.artifacts ?? new List<EquipmentItemSO>();
            ArtifactButton prefabBtn = null;

            if (artifactButtonPoolingSO != null && artifactButtonPoolingSO.prefab != null)
                prefabBtn = artifactButtonPoolingSO.prefab.GetComponent<ArtifactButton>();

            for (int i = 0; i < equippedArtifactSlotImages.Count; i++)
            {
                var trigger = equippedArtifactSlotImages[i].GetComponent<SlotHoverClickTrigger>();
                bool hasArtifact = i < equippedList.Count;

                if (hasArtifact)
                {
                    equippedArtifactSlotImages[i].sprite = equippedList[i].itemIcon;
                    
                    if (equippedArtifactRarityImages != null && i < equippedArtifactRarityImages.Count && equippedArtifactRarityImages[i] != null)
                    {
                        if (prefabBtn != null)
                            equippedArtifactRarityImages[i].sprite = prefabBtn.GetRaritySprite(equippedList[i].rarity);
                        equippedArtifactRarityImages[i].gameObject.SetActive(true);
                    }
                }
                else
                {
                    equippedArtifactSlotImages[i].sprite = emptyArtifactSlotSprite;
                    
                    if (equippedArtifactRarityImages != null && i < equippedArtifactRarityImages.Count && equippedArtifactRarityImages[i] != null)
                    {
                        equippedArtifactRarityImages[i].sprite = null;
                        equippedArtifactRarityImages[i].gameObject.SetActive(false);
                    }
                }

                if (trigger != null) trigger.SetInteractable(hasArtifact);
            }
        }

        private void HandleArtifactEquip(ArtifactEquipEvent evt)
        {
            if (_unit == null || _unit.EquippedArtifacts == null) return;
            if (_unit.EquippedArtifacts.artifacts.Contains(evt.EquipmentItem)) return;

            if (_unit.EquippedArtifacts.artifacts.Count >= maxArtifactEquipCount)
            {
                Bus<ShowMessageUIEvent>.Raise(new ShowMessageUIEvent($"아티팩트는 최대 {maxArtifactEquipCount}개까지만 장착할 수 있습니다."));
                return;
            }

            _unit.EquippedArtifacts.artifacts.Add(evt.EquipmentItem);
            RefreshArtifactUI();
        }

        private void HandleArtifactUnequip(ArtifactUnequipEvent evt)
        {
            if (_unit == null || _unit.EquippedArtifacts == null) return;
            if (_unit.EquippedArtifacts.artifacts.Remove(evt.EquipmentItem)) RefreshArtifactUI();
        }

        private void RefreshSkillList()
        {
            if (_unit == null || SkillSendManager.Instance == null) return;

            foreach (var btn in _activeSkillButtons) btn.ReturnToPool();
            _activeSkillButtons.Clear();

            var availableSkills = SkillSendManager.Instance.GetSkillList(_unit.UnitType);
            var equippedSkills = SkillSendManager.Instance.GetEquipSkills(_unit.UnitType);

            foreach (var skillSO in availableSkills)
            {
                var btn = _poolManager.Pop<CharacterSkillButton>(skillButtonPoolingSO);
                btn.transform.SetParent(ownSkillContainer);
                btn.transform.localScale = Vector3.one;

                bool isEquipped = equippedSkills.Contains(skillSO);
                btn.SetSkill(skillSO, isEquipped);
                
                if (isEquipped) btn.transform.SetAsFirstSibling();
                else btn.transform.SetAsLastSibling();
                
                _activeSkillButtons.Add(btn);
            }
        }

        private void RefreshSkillLoadoutUI(bool instant = false)
        {
            if (_unit == null) return;

            int currentCost = GetCurrentSkillLoadoutCost();
            int maxCost = _unit.LoadOutCost;

            if (skillLoadoutText != null) skillLoadoutText.text = $"{currentCost} / {maxCost}";

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
                    if (_fillCoroutine != null) StopCoroutine(_fillCoroutine);
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
            if (SkillSendManager.Instance != null && _unit != null)
            {
                var equippedSkills = SkillSendManager.Instance.GetEquipSkills(_unit.UnitType);
                foreach (var skill in equippedSkills)
                {
                    if (skill != null) totalCost += skill.SkillCost;
                }
            }
            return totalCost;
        }

        private void HandleSkillEquipped(SkillEquipEvent evt)
        {
            if (_unit == null || evt.Skill == null || evt.Skill.unitType != _unit.UnitType) return;
            if (SkillSendManager.Instance == null) return;

            var equippedSkills = SkillSendManager.Instance.GetEquipSkills(_unit.UnitType);

            if (equippedSkills.Length >= 4)
            {
                Bus<ShowMessageUIEvent>.Raise(new ShowMessageUIEvent("스킬은 최대 4개까지만 장착할 수 있습니다."));
                return;
            }

            int currentCost = GetCurrentSkillLoadoutCost();
            if (currentCost + evt.Skill.SkillCost > _unit.LoadOutCost)
            {
                Bus<ShowMessageUIEvent>.Raise(new ShowMessageUIEvent("스킬 코스트 총량을 초과하여 장착할 수 없습니다."));
                return;
            }

            if (_unit.SkillStorage != null && !_unit.SkillStorage.skills.Contains(evt.Skill))
                _unit.SkillStorage.skills.Add(evt.Skill);

            SkillSendManager.Instance.SyncEquippedSkills(_unit);

            if (IsOpen)
            {
                RefreshSkillList();
                RefreshSkillLoadoutUI();
            }
        }

        private void HandleSkillUnequipped(SkillUnequipEvent evt)
        {
            if (_unit == null || evt.Skill == null || evt.Skill.unitType != _unit.UnitType) return;
            
            if (_unit.SkillStorage != null)
                _unit.SkillStorage.skills.Remove(evt.Skill);

            if (SkillSendManager.Instance != null)
                SkillSendManager.Instance.SyncEquippedSkills(_unit);
            
            if (IsOpen)
            {
                RefreshSkillList();
                RefreshSkillLoadoutUI();
            }
        }
    }
}
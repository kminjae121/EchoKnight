using System.Collections.Generic;
using System.Linq;
using Code.Core.Events.Bus;
using Code.Items;
using Code.UnitSystem;
using Code.UnitSystem.ArtifactSystem;
using GondrLib.Dependencies;
using GondrLib.ObjectPool.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    public class CharacterArtifactPanel : Panel
    {
        [Header("Pool Settings")]
        [SerializeField] private PoolingItemSO artifactButtonPoolingSO;

        [Header("Inventory Area")]
        [SerializeField] private Transform inventoryTrm; 
        [SerializeField] private TextMeshProUGUI countText;
        [SerializeField] private Button sortButton;
        [SerializeField] private int maxInventoryCapacity = 20;

        [Header("Equipped Area")]
        [SerializeField] private List<Image> equippedSlotImages; 
        [SerializeField] private Sprite emptySlotSprite;
        [SerializeField] private int maxEquipCount = 2;

        [Inject] private PoolManagerMono _poolManager;
        
        private UnitSO _unit;
        private List<ArtifactButton> _activeButtons = new();
        private bool _isSortedByRarity = false; 

        public override void Awake()
        {
            base.Awake();

            sortButton.onClick.AddListener(ToggleSort);

            Bus<CharacterInfoEvent>.Subscribe(HandleCharacterInfo);
            Bus<ArtifactEquipEvent>.Subscribe(HandleEquip);
            Bus<ArtifactUnequipEvent>.Subscribe(HandleUnequip);

            for (int i = 0; i < equippedSlotImages.Count; i++)
            {
                int index = i;
                var trigger = equippedSlotImages[i].gameObject.AddComponent<SlotHoverClickTrigger>();
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

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            sortButton.onClick.RemoveListener(ToggleSort);

            Bus<CharacterInfoEvent>.Unsubscribe(HandleCharacterInfo);
            Bus<ArtifactEquipEvent>.Unsubscribe(HandleEquip);
            Bus<ArtifactUnequipEvent>.Unsubscribe(HandleUnequip);
        }

        public override void Open()
        {
            base.Open();
            if (_unit != null) RefreshUI();
        }

        private void HandleCharacterInfo(CharacterInfoEvent evt)
        {
            _unit = evt.Unit.Data;
            if (IsOpen) RefreshUI();
        }

        private void ToggleSort()
        {
            _isSortedByRarity = !_isSortedByRarity;
            RefreshUI();
        }

        private void RefreshUI()
        {
            if (_unit == null || _unit.OwnArtifactStorage == null) return;

            int currentCount = _unit.OwnArtifactStorage.artifacts.Count;
            countText.text = $"{currentCount}/{maxInventoryCapacity}";

            foreach (var btn in _activeButtons) btn.ReturnToPool();
            _activeButtons.Clear();

            var displayList = _unit.OwnArtifactStorage.artifacts
                .Where(a => _unit.EquippedArtifacts == null || !_unit.EquippedArtifacts.artifacts.Contains(a))
                .ToList();

            if (_isSortedByRarity)
            {
                displayList = displayList.OrderByDescending(a => a.rarity).ToList();
            }

            foreach (var artifact in displayList)
            {
                var btn = _poolManager.Pop<ArtifactButton>(artifactButtonPoolingSO);
                btn.transform.SetParent(inventoryTrm);
                btn.transform.SetAsLastSibling(); 
                btn.transform.localScale = Vector3.one;

                btn.SetArtifact(artifact, false);
                _activeButtons.Add(btn);
            }

            RefreshEquippedSlots();
        }

        private void RefreshEquippedSlots()
        {
            var equippedList = _unit.EquippedArtifacts?.artifacts ?? new List<EquipmentItemSO>();

            for (int i = 0; i < equippedSlotImages.Count; i++)
            {
                var trigger = equippedSlotImages[i].GetComponent<SlotHoverClickTrigger>();
                bool hasArtifact = i < equippedList.Count;

                if (hasArtifact)
                {
                    equippedSlotImages[i].sprite = equippedList[i].itemIcon;
                }
                else
                {
                    equippedSlotImages[i].sprite = emptySlotSprite;
                }

                if (trigger != null) trigger.SetInteractable(hasArtifact);
            }
        }

        private void HandleEquip(ArtifactEquipEvent evt)
        {
            if (_unit.EquippedArtifacts == null) return;
            if (_unit.EquippedArtifacts.artifacts.Contains(evt.EquipmentItem)) return;

            if (_unit.EquippedArtifacts.artifacts.Count >= maxEquipCount)
            {
                Bus<ShowMessageUIEvent>.Raise(new ShowMessageUIEvent($"아티팩트는 최대 {maxEquipCount}개까지만 장착할 수 있습니다."));
                return;
            }

            _unit.EquippedArtifacts.artifacts.Add(evt.EquipmentItem);
            RefreshUI();
        }

        private void HandleUnequip(ArtifactUnequipEvent evt)
        {
            if (_unit.EquippedArtifacts == null) return;
            
            if (_unit.EquippedArtifacts.artifacts.Remove(evt.EquipmentItem))
            {
                RefreshUI();
            }
        }
    }
}
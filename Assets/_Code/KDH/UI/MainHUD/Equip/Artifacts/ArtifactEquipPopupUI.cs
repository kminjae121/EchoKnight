using Code.Core.Events.Bus;
using Code.Items;
using Code.UnitSystem.ArtifactSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    public class ArtifactEquipPopupUI : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private TextMeshProUGUI tierText;
        [SerializeField] private Button equipButton;
        [SerializeField] private Button unequipButton;

        private RectTransform _rectTransform;
        private EquipmentItemSO _targetEquipmentItem;
        private bool _isCurrentlyEquipped;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();

            Bus<ArtifactPopupEvent>.Subscribe(HandlePopupEvent);
            
            equipButton.onClick.AddListener(HandleEquip);
            unequipButton.onClick.AddListener(HandleUnequip);

            Hide();
        }

        private void OnDestroy()
        {
            Bus<ArtifactPopupEvent>.Unsubscribe(HandlePopupEvent);
            equipButton.onClick.RemoveListener(HandleEquip);
            unequipButton.onClick.RemoveListener(HandleUnequip);
        }

        private void HandlePopupEvent(ArtifactPopupEvent evt)
        {
            if (evt.EquipmentItem == null)
            {
                Hide();
                return;
            }

            _targetEquipmentItem = evt.EquipmentItem;
            _isCurrentlyEquipped = evt.IsEquipped;

            nameText.text = _targetEquipmentItem.itemName;
            descriptionText.text = _targetEquipmentItem.itemDesc;

            if (tierText != null)
            {
                tierText.text = _targetEquipmentItem.rarity.ToString();
                SetTierTextColor(_targetEquipmentItem.rarity);
            }

            equipButton.gameObject.SetActive(!_isCurrentlyEquipped && !evt.IsReadOnly);
            unequipButton.gameObject.SetActive(_isCurrentlyEquipped && !evt.IsReadOnly);

            if (evt.Pivot != null)
            {
                _rectTransform.position = evt.Pivot.position;
                _rectTransform.anchoredPosition += new Vector2(evt.Offset.x, evt.Offset.y);
            }

            gameObject.SetActive(true);
            transform.SetAsLastSibling();
        }

        private void SetTierTextColor(ArtifactRarity rarity)
        {
            switch (rarity)
            {
                case ArtifactRarity.Legendary: tierText.color = new Color(1f, 0.84f, 0f); break;
                case ArtifactRarity.Epic: tierText.color = new Color(0.63f, 0.13f, 0.94f); break;
                case ArtifactRarity.Rare: tierText.color = new Color(0f, 0.5f, 1f); break;
                case ArtifactRarity.Uncommon: tierText.color = Color.green; break;
                case ArtifactRarity.Common: default: tierText.color = Color.gray; break;
            }
        }

        private void HandleEquip()
        {
            if (_targetEquipmentItem != null && !_isCurrentlyEquipped)
                Bus<ArtifactEquipEvent>.Raise(new ArtifactEquipEvent(_targetEquipmentItem));
            Hide();
        }

        private void HandleUnequip()
        {
            if (_targetEquipmentItem != null && _isCurrentlyEquipped)
                Bus<ArtifactUnequipEvent>.Raise(new ArtifactUnequipEvent(_targetEquipmentItem));
            Hide();
        }

        private void Hide()
        {
            if (!gameObject.activeSelf && _targetEquipmentItem == null) return;
            
            gameObject.SetActive(false);
            _targetEquipmentItem = null;
            
            Bus<ArtifactPopupEvent>.Raise(new ArtifactPopupEvent(null, false, null));
        }
    }
}
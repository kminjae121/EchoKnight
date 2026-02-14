using Code.Core.Events.Bus;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Code.UI
{
    public class PartyCharacterSlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image slotImage;
        [SerializeField] private Button slotButton;
        [SerializeField] private UnitSO characterInfo;

        private bool _isSelected;

        private void Awake()
        {
            slotButton.onClick.AddListener(HandleSlotButton);
            Bus<PartyCharacterSelectEvent>.Subscribe(HandleCharacterSelected);
            Bus<PartyCharacterDeselectEvent>.Subscribe(HandleCharacterDeselected);

            slotImage.sprite = characterInfo.UnitImage;
        }

        private void OnDestroy()
        {
            slotButton.onClick.RemoveListener(HandleSlotButton);
            Bus<PartyCharacterSelectEvent>.Unsubscribe(HandleCharacterSelected);
            Bus<PartyCharacterDeselectEvent>.Unsubscribe(HandleCharacterDeselected);
        }

        private void HandleSlotButton()
        {
            if (_isSelected)
                Bus<PartyCharacterDeselectEvent>.Raise(new PartyCharacterDeselectEvent(characterInfo));
            else
                Bus<PartyCharacterSelectEvent>.Raise(new PartyCharacterSelectEvent(characterInfo));
        }

        private void HandleCharacterSelected(PartyCharacterSelectEvent evt)
        {
            if (evt.Unit == characterInfo)
                _isSelected = true;
        }

        private void HandleCharacterDeselected(PartyCharacterDeselectEvent evt)
        {
            if (evt.Unit == characterInfo)
                _isSelected = false;
        }

        public void OnPointerEnter(PointerEventData eventData)
            => Bus<PartyCharacterHoverEvent>.Raise(new PartyCharacterHoverEvent(characterInfo.UnitImage, characterInfo.UnitName, null));

        public void OnPointerExit(PointerEventData eventData)
            => Bus<PartyCharacterHoverEvent>.Raise(new PartyCharacterHoverEvent(null, null, null));
    }
}
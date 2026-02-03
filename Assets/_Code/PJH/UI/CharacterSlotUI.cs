using Code.Core.Events.Bus;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Code.UI
{
    public class CharacterSlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image slotImage;
        [SerializeField] private Button slotButton;
        [SerializeField] private UnitSO characterInfo;

        private bool _isSelected;
        
        private void Awake()
        {
            slotButton.onClick.AddListener(HandleSlotButton);
            Bus<CharacterSelectEvent>.Subscribe(HandleCharacterSelected);
            Bus<CharacterDeselectEvent>.Subscribe(HandleCharacterDeselected);

            slotImage.sprite = characterInfo.UnitImage;
        }

        private void OnDestroy()
        {
            slotButton.onClick.RemoveListener(HandleSlotButton);
            Bus<CharacterSelectEvent>.Unsubscribe(HandleCharacterSelected);
            Bus<CharacterDeselectEvent>.Unsubscribe(HandleCharacterDeselected);
        }

        private void HandleSlotButton()
        {
            if (_isSelected)
                Bus<CharacterDeselectEvent>.Raise(new CharacterDeselectEvent(characterInfo));
            else
                Bus<CharacterSelectEvent>.Raise(new CharacterSelectEvent(characterInfo));
        }
        
        private void HandleCharacterSelected(CharacterSelectEvent evt)
        {
            if (evt.Unit == characterInfo)
                _isSelected = true;
        }
        
        private void HandleCharacterDeselected(CharacterDeselectEvent evt)
        {
            if (evt.Unit == characterInfo)
                _isSelected = false;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Bus<CharacterHoverEvent>.Raise(new CharacterHoverEvent(characterInfo.UnitName));
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Bus<CharacterHoverEvent>.Raise(new CharacterHoverEvent(null));
        }
    }
}
using Code.Core.Events.Bus;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    public class SelectedCharacterSlotUI : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private Image slotImage;
        [SerializeField] private Button slotButton;
        
        [Header("Data")]
        [SerializeField] private UnitSO characterInfo;

        private Sprite _defaultSprite;
        
        private void Awake()
        {
            slotButton.onClick.AddListener(HandleSlotButton);
            _defaultSprite = slotImage.sprite;
        }

        private void OnDestroy()
        {
            slotButton.onClick.RemoveListener(HandleSlotButton);
        }
        
        public void SetUnit(UnitSO unit)
        {
            if (unit == null)
            {
                characterInfo = null;
                slotImage.sprite = _defaultSprite;
            }
            else
            {
                characterInfo = unit;
                slotImage.sprite = characterInfo.UnitImage;
            }
        }

        private void HandleSlotButton()
        {
            if (characterInfo == null)
                return;

            var removedInfo = characterInfo;
            characterInfo = null;
            slotImage.sprite = _defaultSprite;
            
            Bus<PartyCharacterDeselectEvent>.Raise(new PartyCharacterDeselectEvent(removedInfo));
        }
    }
}
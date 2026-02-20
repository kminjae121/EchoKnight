using Code.Core.Events.Bus;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    public class PartyCharacterInfoUI : MonoBehaviour
    {
        [SerializeField] private Image characterImage;
        [SerializeField] private TextMeshProUGUI characterNameText;
        [SerializeField] private TextMeshProUGUI characterDescText;

        private void Awake()
        {
            Bus<PartyCharacterHoverEvent>.Subscribe(HandleHover);
        }

        private void OnDestroy()
        {
            Bus<PartyCharacterHoverEvent>.Unsubscribe(HandleHover);
        }

        private void HandleHover(PartyCharacterHoverEvent evt)
        {
            characterImage.gameObject.SetActive(evt.CharacterImage != null);
            characterImage.sprite = evt.CharacterImage;
            
            characterNameText.text = evt.CharacterName ?? string.Empty;
            characterDescText.text = evt.CharacterDesc ?? string.Empty;
        }
    }
}
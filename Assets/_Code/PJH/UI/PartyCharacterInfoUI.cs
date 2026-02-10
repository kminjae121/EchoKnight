using Code.Core.Events.Bus;
using TMPro;
using UnityEngine;

namespace Code.UI
{
    public class PartyCharacterInfoUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI characterNameText;

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
            characterNameText.text = evt.CharacterName ?? string.Empty;
        }
    }
}
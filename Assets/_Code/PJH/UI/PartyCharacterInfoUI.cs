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
            Bus<CharacterHoverEvent>.Subscribe(HandleHover);
        }

        private void OnDestroy()
        {
            Bus<CharacterHoverEvent>.Unsubscribe(HandleHover);
        }

        private void HandleHover(CharacterHoverEvent evt)
        {
            characterNameText.text = evt.CharacterName ?? string.Empty;
        }
    }
}
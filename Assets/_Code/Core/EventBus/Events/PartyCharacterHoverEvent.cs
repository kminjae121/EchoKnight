using UnityEngine;

namespace Code.Core.Events.Bus
{
    public struct PartyCharacterHoverEvent : IEvent
    {
        public Sprite CharacterImage { get; }
        public string CharacterName { get; }
        public string CharacterDesc { get; }
        
        public PartyCharacterHoverEvent(Sprite characterImage, string characterName, string characterDesc)
        {
            CharacterImage = characterImage;
            CharacterName = characterName;
            CharacterDesc = characterDesc;
        }
    }
}
namespace Code.Core.Events.Bus
{
    public struct PartyCharacterHoverEvent : IEvent
    {
        public string CharacterName { get; }
        
        public PartyCharacterHoverEvent(string characterName)
        {
            CharacterName = characterName;
        }
    }
}
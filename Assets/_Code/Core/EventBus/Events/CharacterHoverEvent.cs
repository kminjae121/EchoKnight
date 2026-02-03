namespace Code.Core.Events.Bus
{
    public struct CharacterHoverEvent : IEvent
    {
        public string CharacterName { get; }
        
        public CharacterHoverEvent(string characterName)
        {
            CharacterName = characterName;
        }
    }
}
using Code.Items;

namespace Code.Core.Events.Bus
{
    public class CombatArtifactHoverEvent : IEvent
    {
        public ItemSO Artifact { get; }
        public bool IsShow { get; }

        public CombatArtifactHoverEvent(ItemSO artifact, bool isShow)
        {
            Artifact = artifact;
            IsShow = isShow;
        }
    }
}
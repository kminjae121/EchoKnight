using Code.UnitSystem.ArtifactSystem;

namespace Code.Core.Events.Bus
{
    public struct EquipArtifactEvent : IEvent
    {
        public ArtifactSO artifact;

        public EquipArtifactEvent(ArtifactSO artifact)
        {
            this.artifact = artifact;
        }
    }
}
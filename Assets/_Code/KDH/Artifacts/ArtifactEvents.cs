using Code.UnitSystem.ArtifactSystem;
using UnityEngine;

namespace Code.Core.Events.Bus
{
    public class ArtifactEquipEvent : IEvent
    { public ArtifactSO Artifact { get; } public ArtifactEquipEvent(ArtifactSO a) => Artifact = a; }
    public class ArtifactUnequipEvent : IEvent
    { public ArtifactSO Artifact { get; } public ArtifactUnequipEvent(ArtifactSO a) => Artifact = a; }
    
    public class ArtifactPopupEvent : IEvent
    {
        public ArtifactSO Artifact { get; }
        public bool IsEquipped { get; }
        public Vector2 Position { get; }
        public bool IsReadOnly { get; }

        public ArtifactPopupEvent(ArtifactSO artifact, bool isEquipped, Vector2 position, bool isReadOnly = false)
        {
            Artifact = artifact;
            IsEquipped = isEquipped;
            Position = position;
            IsReadOnly = isReadOnly;
        }
    }
}
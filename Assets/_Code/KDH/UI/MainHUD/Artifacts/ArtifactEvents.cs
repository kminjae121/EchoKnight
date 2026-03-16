using Code.Items;
using Code.UnitSystem.ArtifactSystem;
using UnityEngine;

namespace Code.Core.Events.Bus
{
    public class ArtifactEquipEvent : IEvent
    { public EquipmentItemSO EquipmentItem { get; } public ArtifactEquipEvent(EquipmentItemSO a) => EquipmentItem = a; }
    public class ArtifactUnequipEvent : IEvent
    { public EquipmentItemSO EquipmentItem { get; } public ArtifactUnequipEvent(EquipmentItemSO a) => EquipmentItem = a; }
    
    public class ArtifactPopupEvent : IEvent
    {
        public EquipmentItemSO EquipmentItem { get; }
        public bool IsEquipped { get; }
        public Vector2 Position { get; }
        public bool IsReadOnly { get; }

        public ArtifactPopupEvent(EquipmentItemSO equipmentItem, bool isEquipped, Vector2 position, bool isReadOnly = false)
        {
            EquipmentItem = equipmentItem;
            IsEquipped = isEquipped;
            Position = position;
            IsReadOnly = isReadOnly;
        }
    }
}
using UnityEngine;

namespace Code.Items
{
    public enum ArtifactRarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary
    }

    [CreateAssetMenu(fileName = "ArtifactSO", menuName = "SO/ArtifactSystem/ArtifactSO")]
    public class EquipmentItemSO : ItemSO
    {
        public ArtifactRarity rarity;
    }
}
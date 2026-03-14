using UnityEngine;

namespace Code.UnitSystem.ArtifactSystem
{
    public enum ArtifactRarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary
    }

    [CreateAssetMenu(fileName = "ArtifactSO", menuName = "ArtifactSystem/ArtifactSO")]
    public class ArtifactSO : ScriptableObject
    {
        public string artifactName;
        public Sprite artifactIcon;
        [TextArea(3, 10)]
        public string description;
        public ArtifactRarity rarity;
    }
}
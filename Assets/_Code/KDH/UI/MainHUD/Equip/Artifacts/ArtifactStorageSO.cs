using System.Collections.Generic;
using Code.Items;
using UnityEngine;

namespace Code.UnitSystem.ArtifactSystem
{
    [CreateAssetMenu(fileName = "ArtifactStorage", menuName = "ArtifactSystem/ArtifactStorage")]
    public class ArtifactStorageSO : ScriptableObject
    {
        public List<EquipmentItemSO> artifacts = new List<EquipmentItemSO>();
    }
}
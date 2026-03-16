using System.Collections.Generic;
using UnityEngine;

namespace Code.UnitSystem.ArtifactSystem
{
    [CreateAssetMenu(fileName = "ArtifactStorage", menuName = "ArtifactSystem/ArtifactStorage")]
    public class ArtifactStorageSO : ScriptableObject
    {
        public List<ArtifactSO> artifacts = new List<ArtifactSO>();
    }
}
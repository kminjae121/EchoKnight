using Code.UnitSystem;
using Code.UnitSystem.ArtifactSystem;
using UnityEngine;

namespace Code.Artifact
{
    public class ArtifactStroage : MonoBehaviour, IUnitComponent
    {
        private ArtifactStorageSO artifactStorageSO;
        
        private Unit _unit;
        public void Initialize(Unit owner)
        {
            _unit = owner;
        }
    }
}
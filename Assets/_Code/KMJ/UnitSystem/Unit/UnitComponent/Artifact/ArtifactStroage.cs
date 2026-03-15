using System;
using System.Collections.Generic;
using Code.Core.Events.Bus;
using Code.UnitSystem;
using Code.UnitSystem.ArtifactSystem;
using UnityEngine;

namespace Code.Artifact
{
    public class ArtifactStroage : MonoBehaviour, IUnitComponent
    {
        private List<ArtifactSO> artifactStorageSO;
        
        private Unit _unit;
        
        public void Initialize(Unit owner)
        {
            _unit = owner;
        }

        private void Start()
        {
            
        }

        public void GetArtifact(ArtifactSO artifact)
        {
            artifactStorageSO.Add(artifact);
            Bus<EquipArtifactEvent>.Raise(new EquipArtifactEvent(artifact));
        }
    }
}
using System;
using System.Collections.Generic;
using Code.Core.Events.Bus;
using NUnit.Framework;
using UnityEngine;

namespace Code.UnitSystem.GimicSystem
{
    public class KnightSword : MonoBehaviour
    {
        [SerializeField] private List<Material> swordMaterials;

        [SerializeField] private MeshRenderer meshRenderer;


        private void Start()
        {
            Bus<KnightSwordEvent>.Subscribe(SetMatIntnsity);
        }
        private void OnDestroy()
        {
            Bus<KnightSwordEvent>.Unsubscribe(SetMatIntnsity);
        }

        private void SetMatIntnsity(KnightSwordEvent evt)
        {
            meshRenderer.material = swordMaterials[evt.idx];
        }
    }
}
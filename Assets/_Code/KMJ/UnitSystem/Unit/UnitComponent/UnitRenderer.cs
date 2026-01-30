using System;
using UnitSystem;
using UnityEngine;

namespace Code.UnitSystem
{
    public class UnitRenderer : MonoBehaviour, IUnitComponent
    {
        [SerializeField] private Animator thisAnimator;

        private Unit _owner;

        private bool isMove = false;
        
        public void Initialize(Unit owner)
        {
            _owner = owner;
        }

    }
}
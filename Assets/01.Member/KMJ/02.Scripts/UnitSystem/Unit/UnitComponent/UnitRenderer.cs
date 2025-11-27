using System;
using UnityEngine;

namespace UnitSystem
{
    public class UnitRenderer : MonoBehaviour, IUnitComponent
    {
        [SerializeField] private Animator thisAnimator;

        private Unit _owner;
        
        public void Initialize(Unit owner)
        {
            _owner = owner;
            
            if(thisAnimator.runtimeAnimatorController == null)
                thisAnimator.runtimeAnimatorController = _owner.unitSO.animationController;
        }

    }
}
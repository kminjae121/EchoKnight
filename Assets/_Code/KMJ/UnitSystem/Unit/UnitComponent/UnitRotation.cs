using System;
using Code.UnitSystem;
using UnityEngine;

namespace Code.UnitSystem
{
    public class UnitRotation : MonoBehaviour, IUnitComponent
    {
        private Vector3 _targetRotation;
        
        public void Initialize(Code.UnitSystem.Unit owner)
        {
            
        }
        private void Update()
        {
            RotationUnit();
        }

        public void SetDir(Vector3 dir)
        {
            _targetRotation = dir;
            
        }
        
        public void RotationUnit()
        {
            Vector3 direction = _targetRotation - transform.position;
            
            direction.y = 0;
            
            if (direction.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 8f);
            }
        }

    }
}
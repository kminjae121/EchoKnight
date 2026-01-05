using System;
using UnityEngine;

namespace _01.Member.KMJ._02.Scripts.UnitSystem.Unit.UnitComponent
{
    public class UnitRotation : MonoBehaviour
    {
        private Vector3 _targetRotation;
        
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
                
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
            }
        }

    }
}
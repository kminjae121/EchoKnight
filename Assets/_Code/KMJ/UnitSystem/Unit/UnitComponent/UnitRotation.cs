using UnityEngine;

namespace Code.UnitSystem
{
    public class UnitRotation : MonoBehaviour, IUnitComponent
    {
        private Vector3 _targetDirection;
        
        [Header("Settings")]
        [SerializeField] private float _rotationSpeed = 30f;

        private Unit _owner;
        
        public void Initialize(Unit owner)
        {
            _targetDirection = transform.forward;
            _owner = owner;
        }

        private void Update()
        {
            RotationUnit();
        }

        public void SetDir(Vector3 targetPosition)
        {
            Vector3 direction = targetPosition - transform.position;
            direction.y = 0;
            
            if (direction.sqrMagnitude > 0.001f)
            {
                _targetDirection = direction.normalized;
            }
        }
        
        public void RotationUnit()
        {
            if (_targetDirection.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(_targetDirection);
                _owner.transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * _rotationSpeed);
            }
        }
    }
}
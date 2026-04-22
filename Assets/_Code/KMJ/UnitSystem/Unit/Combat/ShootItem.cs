using System;
using UnityEngine;
using UnityEngine.Events;

namespace Code.UnitSystem.Combat
{
    public class ShootItem : MonoBehaviour
    {
        [field : SerializeField] public string itemName { get; private set; }

        [SerializeField] private float _moveSpeed = 5f;


        public UnityEvent AtkEvent;

        private GameObject _target = null;
        private ShootItemAttackManager _shootItemManager;

        [SerializeField] private bool isDirectDie = true;


        private void Awake()
        {
            AtkEvent.AddListener(SetDie);
        }

        private void FixedUpdate()
        {
            transform.rotation = Quaternion.LookRotation(transform.position - _target.transform.position);
            
            transform.position = Vector3.MoveTowards(transform.position, _target.transform.position, _moveSpeed * Time.fixedDeltaTime);

            float distance = Vector3.Distance(transform.position, _target.transform.position);
            
            if (distance <= 0.2f)
            {
                _shootItemManager.hitEvent?.Invoke();
                AtkEvent?.Invoke();
            }
        }


        public void SetDie()
        {
            gameObject.SetActive(false);
        }

        public void SetTarget(GameObject target)
        {
            _target = target;
        }

        public void SetShootItemCompo(ShootItemAttackManager shootItemManaer)
        {
            _shootItemManager = shootItemManaer;
        }
        
    }
}
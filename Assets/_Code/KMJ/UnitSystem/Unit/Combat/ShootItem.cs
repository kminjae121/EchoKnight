using UnityEngine;

namespace Code.UnitSystem.Combat
{
    public class ShootItem : MonoBehaviour
    {
        [field : SerializeField] public string itemName { get; private set; }
        
        [SerializeField] private LayerMask _whatIsEnemy;

        [SerializeField] private float _moveSpeed = 5f;
        

        private GameObject _target = null;
        private ShootItemAttackManager _shootItemManager;
        

        private void FixedUpdate()
        {
            transform.rotation = Quaternion.LookRotation(transform.position - _target.transform.position);
            
            transform.position = Vector3.MoveTowards(transform.position, _target.transform.position, _moveSpeed * Time.fixedDeltaTime);
        }

        public void SetTarget(GameObject target)
        {
            _target = target;
        }

        public void SetShootItemCompo(ShootItemAttackManager shootItemManaer)
        {
            _shootItemManager = shootItemManaer;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (((1 << other.gameObject.layer) & _whatIsEnemy) != 0)
                if (other.gameObject == _target)
                {
                    _shootItemManager.hitEvent.Invoke();
                    gameObject.SetActive(false);   
                }
        }
    }
}
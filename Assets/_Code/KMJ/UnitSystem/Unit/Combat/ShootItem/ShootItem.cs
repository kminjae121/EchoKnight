using Code.Core.Events.Bus;
using UnityEngine;
using UnityEngine.Events;

namespace Code.UnitSystem.Combat
{
    public abstract class ShootItem : MonoBehaviour
    {
        [field : SerializeField] public string itemName { get; private set; }
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] protected bool isDirectDie = true;
        
        public UnityEvent AtkEvent;

        private ShootItemAttackManager _shootItemManager;
        private Rigidbody _rbCompo = null;
        private GameObject _target = null;
        
        private void Awake()
        {
            AtkEvent.AddListener(SetDie);
            _rbCompo = GetComponent<Rigidbody>();
        }

        public virtual void SetDie()
        {
            if(isDirectDie)
                gameObject.SetActive(false);
        }

        public void SetTarget(GameObject target)
        {
            _target = target;

            if (_target == null) return;
            
            Vector3 dir = (_target.transform.position - transform.position).normalized;

            transform.rotation = Quaternion.Euler(transform.position - _target.transform.position);

            _rbCompo.AddForce(dir * _moveSpeed, ForceMode.Impulse);
        }

        public void SetShootItemCompo(ShootItemAttackManager shootItemManaer)
        {
            _shootItemManager = shootItemManaer;
        }

        public abstract void GiveDamage();

        private void OnTriggerEnter(Collider other)
        {
            Bus<DamageEvent>.Raise(new DamageEvent(_shootItemManager.DamageData,_target.gameObject,0,_shootItemManager.Unit
                , false,false,0.2f));
            
            _shootItemManager.hitEvent?.Invoke();
            AtkEvent?.Invoke();
            
            GiveDamage();
        }
    }
}
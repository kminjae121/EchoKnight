using Code.Core.Events.Bus;
using Code.EntityComponent;
using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;

namespace _Code.KMJ.UnitSystem.Unit.UnitComponent
{
    public class ShootItem : MonoBehaviour
    {
        [SerializeField] private LayerMask _whatIsEnemy;

        [SerializeField] private float _moveSpeed = 5f;

        private DamageData _damageData;

        [SerializeField] private AttackDataSO atkData;

        private GameObject _target = null;
        
        protected CinemachineImpulseSource impulseSource;
        
        private void Awake()
        {
            impulseSource = GameObject.Find("ImpulseSource").GetComponent<CinemachineImpulseSource>();
        }

        private void FixedUpdate()
        {
            transform.rotation = Quaternion.LookRotation(transform.position - _target.transform.position);
            transform.position = Vector3.MoveTowards(transform.position, _target.transform.position, _moveSpeed * Time.fixedDeltaTime);
        }

        public void SetDamageData(DamageData damageData)
        {
            _damageData = damageData;
        }

        public void SetTarget(GameObject target)
        {
            _target = target;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (((1 << other.gameObject.layer) & _whatIsEnemy) != 0)
            {
                if (other.gameObject == _target)
                {
                    Bus<HitStopEvent>.Raise(new HitStopEvent(0.2f,0.25f));
                    Bus<TurnEndUIEvent>.Raise(new TurnEndUIEvent(false));
                
                    impulseSource.GenerateImpulse(0.4f);  
                    other.GetComponent<EntityHealth>().ApplyDamage(_damageData,transform.position, transform.position,
                        atkData,null);
                    gameObject.SetActive(false);   
                }
            }
        }
    }
}
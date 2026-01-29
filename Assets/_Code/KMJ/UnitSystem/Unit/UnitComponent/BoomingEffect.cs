using System;
using System.Collections;
using Code.Core.Events.Bus;
using Code.EntityComponent;
using Unity.Cinemachine;
using UnityEngine;

namespace _Code.KMJ.UnitSystem.Unit.UnitComponent
{
    public class BoomingEffect : MonoBehaviour
    {
        private CinemachineImpulseSource impulseSource;
        
        private Collider _collider;
        
        [SerializeField] private LayerMask _whatIsEnemy;

        private DamageData _damageData;

        [SerializeField] private AttackDataSO atkData;

        private void Awake()
        {
            _collider = GetComponent<Collider>();
            _collider.enabled = false;
            _damageData.damage = 4;
            impulseSource = GameObject.Find("ImpulseSource").GetComponent<CinemachineImpulseSource>();
        }

        private void OnEnable()
        {
            StartCoroutine(StartEffect());
        }

        public void SetDamageData(DamageData damageData)
        {
            _damageData = damageData;
        }

        private IEnumerator StartEffect()
        {
            yield return new WaitForSeconds(1.13f);

            _collider.enabled = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (((1 << other.gameObject.layer) & _whatIsEnemy) != 0)
            {
                impulseSource.GenerateImpulse(0.3f);
                Bus<TurnEndUIEvent>.Raise(new TurnEndUIEvent(false));
                Bus<HitStopEvent>.Raise(new HitStopEvent(0.2f,0.25f));
                
                other.GetComponent<EntityHealth>().ApplyDamage(_damageData,transform.position, transform.position,
                    atkData,null);
                
                _collider.enabled = false;
                gameObject.SetActive(false);
            }
        }
    }
}
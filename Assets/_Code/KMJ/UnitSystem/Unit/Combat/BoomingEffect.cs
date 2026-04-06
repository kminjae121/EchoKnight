using System.Collections;
using Code.Core.Events.Bus;
using Code.UnitSystem.Combat;
using UnityEngine;

namespace Code.UnitSystem
{
    public class BoomingEffect : MonoBehaviour
    {
        private Collider _collider;
        
        [SerializeField] private LayerMask _whatIsEnemy;

        private DamageData _damageData;

        [SerializeField] private AttackDataSO atkData;

        private float _addDamage;

        private void Awake()
        {
            _collider = GetComponent<Collider>();
            _collider.enabled = false;
            _damageData.damage = 4;
        }

        private void OnEnable()
        {
            StartCoroutine(StartEffect());
        }

        public void SetDamageData(DamageData damageData,float addDamage)
        {
            _damageData = damageData;
            _addDamage = addDamage;
        }

        private IEnumerator StartEffect()
        {
            yield return new WaitForSeconds(0.35f);

            _collider.enabled = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (((1 << other.gameObject.layer) & _whatIsEnemy) != 0)
            {
                Bus<CamShakeEvent>.Raise(new CamShakeEvent(0.5f));
                
                Bus<DamageEvent>.Raise(new DamageEvent(_damageData,atkData,other.gameObject,0,null,false,false));
                
                _collider.enabled = false;
                gameObject.SetActive(false);
            }
        }
    }
}
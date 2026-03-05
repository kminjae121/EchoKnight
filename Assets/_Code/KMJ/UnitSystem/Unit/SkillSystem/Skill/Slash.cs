using System;
using Code.Core.Events.Bus;
using Code.EntityComponent;
using UnityEngine;

namespace Code.UnitSystem.SkillSystem.Skill
{
    public class Slash : MonoBehaviour
    {
        [SerializeField] private LayerMask _whatIsEnemy;

        [SerializeField] private float _moveSpeed = 5f;

        private DamageData _damageData;

        [SerializeField] private AttackDataSO atkData;
        
        private void Awake()
        {
            _damageData.damage = 4.567f;
        }

        private void FixedUpdate()
        {
            transform.position += -transform.right * _moveSpeed * Time.fixedDeltaTime;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (((1 << other.gameObject.layer) & _whatIsEnemy) != 0)
            {
                Bus<HitStopEvent>.Raise(new HitStopEvent(0.2f,0.25f));
                other.GetComponent<EntityHealth>().ApplyDamage(_damageData,transform.position, transform.position,
                    atkData,null);
                
                gameObject.SetActive(false);
            }
        }
    }
}
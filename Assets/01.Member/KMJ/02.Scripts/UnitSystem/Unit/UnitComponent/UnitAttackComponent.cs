using System;
using EntityComponent;
using UnityEngine;

namespace UnitSystem
{
    public class UnitAttackComponent : MonoBehaviour, IUnitComponent
    {
        private Unit _owner;
        
        private Enemy _targetEnemy;
        private DamageData _damageData;
        [SerializeField] private AttackDataSO attackData;

        private void Awake()
        {
            _damageData = new DamageData();
            _damageData.damage = 1;
        }

        public void Initialize(Unit owner)
        {
            _owner = owner; 
        }

        public void AttackEnemy()
        {   
            _targetEnemy.GetCompo<EntityHealth>().ApplyDamage(_damageData,transform.position,transform.position,attackData,null);
        }
    }
}
using System;
using EntityComponent;
using Input;
using UnityEngine;

namespace UnitSystem
{
    public class UnitAttackComponent : MonoBehaviour, IUnitComponent
    {
        private Unit _owner;
        
        private DamageData _damageData;
        [SerializeField] private AttackDataSO attackData;

        private InputReader _inputReader;

        private UnitSO _unitSO;
        
        private BasicUnit _unit;


        public void Initialize(Unit owner)
        {
            _owner = owner; 
            
            _unit = _owner as  BasicUnit;

            _inputReader = _unit.inputSO;
            _unitSO = _unit.unitSO;
        }
        
        
        private void Awake()
        {
            _damageData = new DamageData();
            _damageData.damage = 1;

            _inputReader.OnAttackEvent += AttackEnemy;
        }

        private void OnDestroy()
        {
            _inputReader.OnAttackEvent -= AttackEnemy;
        }

        public void AttackEnemy()
        {
            Entity enemy = _inputReader.GetEnemy();

            float distance = Vector3.Distance(_unit.transform.position, enemy.transform.position);

            if (distance >= _unitSO.attackDistance)
            {
                Debug.Log("떄림");
                return;
            }
            
            enemy.GetCompo<EntityHealth>().ApplyDamage(_damageData, 
                transform.position,transform.position,attackData,null);
        }
    }
}
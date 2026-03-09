using System;
using _Code.KMJ.UnitSystem.Unit.UnitComponent;
using Code.Core.Events.Bus;
using Code.EntityComponent;
using Code.UnitSystem;
using EnemySystem;
using Input;
using UnityEngine;

namespace UnitSystem
{
    public class UnitTargeting : MonoBehaviour , IUnitComponent
    {
        [SerializeField] private UnitAttackComponent atkCompo;
        [SerializeField] private InputReader inputSO;
        [SerializeField] private UnitBehaviorCompo behaveCompo;

        private GameObject _targetEnemy;

        private EnemyTargeting _targetingCompo;

        private Unit unit;
        public void Initialize(Unit owner)
        {
            unit = owner;
        }

        private void Update()
        {
            HandleTargeting();
        }

        private void HandleTargeting()
        {
            if (!unit.isMyTurn || inputSO == null) return;
            if (atkCompo != null && atkCompo.IsActive) return;

            GameObject enemy = inputSO.GetEnemy();

            if (behaveCompo.visualPrefabs.activeInHierarchy)
                ClearTarget();
            else if (enemy == null && _targetEnemy != null)
                ClearTarget();
            else if (enemy != null)
                SetTarget(enemy);
        }
        
        private void SetTarget(GameObject enemy)
        {
            _targetEnemy = enemy;
            if (_targetEnemy == null) return;

            if (_targetingCompo == null)
            {
                _targetingCompo = _targetEnemy.GetComponent<EnemyTargeting>();
                if (_targetingCompo != null) _targetingCompo.Targeting();

                var health = _targetEnemy.GetComponent<EntityHealth>();
                var unit = _targetEnemy.GetComponent<Code.UnitSystem.Unit>();
                
                Sprite img = (unit != null && unit.unitSO != null) ? unit.unitSO.UnitImage : null;
                float currentHp = health != null ? health.CurrentHealth : 0;
                float maxHp = health != null ? health.MaxHealth : 0;    

                Bus<EnemyHpInfo>.Raise(new EnemyHpInfo(0, currentHp, maxHp, 0, true, img, false, 3));
            }
        }

        private void ClearTarget()
        {
            if (_targetEnemy != null)
            {
                if (_targetingCompo == null) _targetingCompo = _targetEnemy.GetComponent<EnemyTargeting>();
                if (_targetingCompo != null) _targetingCompo.OffTargeting();

                Sprite img = null;
                var unit = _targetEnemy.GetComponent<Unit>();
                if (unit != null && unit.unitSO != null) img = unit.unitSO.UnitImage;

                Bus<EnemyHpInfo>.Raise(new EnemyHpInfo(0, 0, 0, 0, false, img, false, 0));
            }
            
            _targetEnemy = null;
            _targetingCompo = null;
        }

    }
}
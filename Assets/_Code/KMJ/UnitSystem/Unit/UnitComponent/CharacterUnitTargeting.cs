using System;
using _Code.KMJ.UnitSystem.Unit.UnitComponent;
using Code.Core.Events.Bus;
using Code.EntityComponent;
using Code.UnitSystem;
using Code.UnitSystem.SkillSystem;
using EnemySystem;
using Input;
using UnityEngine;

namespace UnitSystem
{
    public class CharacterUnitTargeting : MonoBehaviour , IUnitComponent
    {
        [SerializeField] private UnitAttackComponent atkCompo;
        [SerializeField] private InputReader inputSO;
        [SerializeField] private UnitBehaviorCompo behaveCompo;
        [SerializeField] private SkillManageComponent skillManager;

        private GameObject _targetEnemy;

        private Unit _targetUnit;

        private EnemyTargeting _targetingCompo;

        private CharacterUnit unit;
        public void Initialize(Unit owner)
        {
            unit = owner as CharacterUnit;
        }

        private void Update()
        {
            HandleTargeting();
        }

        private void HandleTargeting()
        {
            if (!unit.isMyTurn || inputSO == null) return;
            
            if (atkCompo != null && atkCompo.IsActive)
            {
                AttackTargeting();
            }
            if (skillManager.GetSkillInfo() != null && skillManager.GetSkillInfo().IsActive)
            {
                SetSkillTargeting();
            }
            else
            {
                EnemyInfoTargeting();
            }
        }

        private void EnemyInfoTargeting()
        {
            GameObject enemy = inputSO.GetEnemy();

            if (behaveCompo.visualPrefabs.activeInHierarchy)
                ClearTarget();
            else if (enemy == null && _targetEnemy != null)
                ClearTarget();
            else if (enemy != null)
                SetTarget(enemy);
        }

        private void SetSkillTargeting()
        {
            GameObject enemy = inputSO.GetEnemy();
            unit.BehaveCompo.ResetTile();
                
            if (enemy == null)
            {
                if (_targetEnemy != null)
                {
                    _targetingCompo = _targetEnemy.GetComponent<EnemyTargeting>();
                    _targetingCompo.OffTargeting();
                        
                    Bus<EnemyHpInfo>.Raise(new EnemyHpInfo(0, 0, 0, 0, false, 
                        _targetEnemy.GetComponent<Unit>().unitSO.UnitImage,true));

                    Bus<SetAtkUIEvent>.Raise(new SetAtkUIEvent());
                    _targetingCompo = null;
                    skillManager.GetSkillInfo().SetEnemyTargeting(null);
                }
            }
            else if (enemy != null)
            {
                skillManager.GetSkillInfo().FindEnemyIsThere(enemy);

                if (_targetEnemy != null && _targetingCompo == null)
                {
                    skillManager.GetSkillInfo().rotationCompo.SetDir(_targetEnemy.transform.position);
                    skillManager.GetSkillInfo().criticalSpot.CheckEnemyBody(skillManager.GetSkillInfo().
                        DamageData,_targetEnemy,skillManager.GetSkillInfo().damage,skillManager.GetSkillInfo().AddDamage);
                        
                    EntityHealth health = _targetEnemy.GetComponent<EntityHealth>();
                    _targetingCompo = _targetEnemy.GetComponent<EnemyTargeting>();
                        
                    if (_targetingCompo != null) _targetingCompo.Targeting();
                        
                    Bus<EnemyHpInfo>.Raise(new EnemyHpInfo(skillManager.GetSkillInfo().AddDamage, health.CurrentHealth,
                        health.MaxHealth, skillManager.GetSkillInfo().DamageData.damage, true,
                        _targetEnemy.GetComponent<Unit>().unitSO.UnitImage,true));

                    skillManager.GetSkillInfo().SetEnemyTargeting(_targetingCompo);
                }
            }
        }

        private void AttackTargeting()
        {
            unit.BehaveCompo.ResetTile();
            
            GameObject enemy = inputSO.GetEnemy();
            

            if(enemy == null)
            {
                if (_targetEnemy != null)
                {
                    if(_targetingCompo != null)
                        _targetingCompo.OffTargeting();
                        
                    Bus<EnemyHpInfo>.Raise(new EnemyHpInfo(0,0,0, 
                        0, false,null,true));

                    _targetingCompo = null;

                    atkCompo.SetTargeting(null);
                }
            }
            else
            {
                atkCompo.FindEnemyIsThere(enemy);
                    
                if (_targetEnemy != null && _targetingCompo == null)
                {
                    atkCompo.RotationCompo.SetDir(_targetEnemy.transform.position);
                        
                    EntityHealth health = _targetEnemy.GetComponent<EntityHealth>();
                    _targetingCompo = _targetEnemy.GetComponent<EnemyTargeting>();
                    _targetUnit = _targetEnemy.GetComponent<Unit>();
                    
                    atkCompo.SetTargeting(_targetingCompo);
                        
                    _targetingCompo.Targeting();
                        
                    atkCompo.CriticalSpot.CheckEnemyBody(atkCompo.DamageData, _targetEnemy.gameObject, atkCompo.AtkDamage, atkCompo.AddDamage);
                        
                    Bus<EnemyHpInfo>.Raise(new EnemyHpInfo(atkCompo.AddDamage,health.CurrentHealth, 
                        health.MaxHealth,atkCompo.DamageData.damage, true,_targetUnit.unitSO.UnitImage,true));
                }
            }
        }

        private void SetTarget(GameObject enemy)
        {
            _targetEnemy = enemy;
            if (_targetEnemy == null) return;

            if (_targetingCompo == null)
            {
                _targetingCompo = _targetEnemy.GetComponent<EnemyTargeting>();
                if (_targetingCompo != null) _targetingCompo.Targeting();

                EntityHealth health = _targetEnemy.GetComponent<EntityHealth>();
                Unit unit = _targetEnemy.GetComponent<Code.UnitSystem.Unit>();
                
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
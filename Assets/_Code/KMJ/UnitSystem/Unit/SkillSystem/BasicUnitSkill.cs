using System;
using Code.Core.Events.Bus;
using Code.Map;
using Code.UnitSystem;
using Code.UnitSystem.Combat;
using EnemySystem;
using Input;
using UnityEngine;

namespace Code.SkillSystem
{
    public class BasicUnitSkill : BaseSkill
    {
        [Header("Basic Settings")]
        [field: SerializeField] public CriticalSpot CriticalSpot { get; private set; }
        
        [SerializeField]  protected CharacterUnit _characterUnit;
        
        private InputReader _inputReader;
        private EnemyTargeting _targetingCompo;
        

        private void OnEnable()
        {
            if (_characterUnit != null)
            {
                _inputReader = _characterUnit.InputSO;
                
                if (_inputReader != null)
                {
                    _inputReader.OnAttackEvent -= UseSkill;
                    _inputReader.OnAttackEvent += UseSkill;
                }
            }

            if (_characterUnit != null)
            {
                RotationCompo = _characterUnit.GetUnitCompo<UnitRotation>();
                triggerCompo = _characterUnit.GetUnitCompo<UnitAnimationTrigger>();
                _skillCompo = _characterUnit.GetUnitCompo<SkillComponent>();
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (_inputReader != null)
                _inputReader.OnAttackEvent -= UseSkill;
        }
        
        public void SetEnemyTargeting(EnemyTargeting targeting)
        {
            _targetingCompo = targeting;
        }     
        
        protected override void CanUseSkillTrue()
        {
            base.CanUseSkillTrue();
            
            Bus<UnitSkilStartEvent>.Raise(new UnitSkilStartEvent(false));
            Bus<UsingSkillEvent>.Raise(new UsingSkillEvent(true));
        }

        protected virtual void SkillEnd()
        {
            IsActive = false;
            Bus<SetAtkUIEvent>.Raise(new SetAtkUIEvent(true));
            Bus<UnitCamSettingEvent>.Raise(new UnitCamSettingEvent(null, false,new Vector3(0.1f,0.1f,0.1f)));
        }

        public override void SkillFinished(bool isCancel)
        {
            base.SkillFinished(isCancel);
        }

        public override void AttackEnemy()
        {
            if (!isCanUseSkill)
            {
                SkillFinished(false);
                return;
            }

            if (_targetEnemy == null) return;

            if (_characterUnit.SkillCostCompo != null)
            {
                _characterUnit.SkillCostCompo.UseSkillCost(SkillSO.SkillCost);
            }
            
            if (RotationCompo != null)
                RotationCompo.SetDir(_targetEnemy.transform.position);
            
            if (_targetingCompo != null)
                _targetingCompo.OffTargeting();
            
            Bus<UnitCamSettingEvent>.Raise(new UnitCamSettingEvent(_characterUnit.gameObject, true,new Vector3(0.1f,0.1f,0.1f)));
            Bus<EnemyHpInfo>.Raise(new EnemyHpInfo(0, 0, 0, 0, false, 
                null,true));
            
            StartEvent();
            
            GridMap.Instance.SetGridVisible(false);
            SkillEvent?.Invoke(_targetEnemy);
           
            SkillFinished(false);
        }

        public void SetEnemy(GameObject target)
        {
            _targetEnemy = target;
        }

        public override void ShowSkillRange()
        {
            base.ShowSkillRange();

            if (_characterUnit == null || _characterUnit.SkillCostCompo == null)
                return;

            if (!_characterUnit.SkillCostCompo.CanUseSkillCost(SkillSO.SkillCost))
            {
                Bus<SendSkillEvent>.Raise(new SendSkillEvent(null));
                Bus<SetAtkUIEvent>.Raise(new SetAtkUIEvent(true));
                Bus<TurnEndUIEvent>.Raise(new TurnEndUIEvent(false));
                Bus<WarningUIEvent>.Raise(new WarningUIEvent("코스트가 부족합니다"));
                return;
            }

            if (SkillSO.IsOwnSkill)
            {
                SkillStartEvent();
                _characterUnit.SkillCostCompo.UseSkillCost(SkillSO.SkillCost);
                _characterUnit.MoveCompo.ResetTile();
                StartEvent();
                SkillEvent?.Invoke(null);
            }
            else
            {
                _characterUnit.MoveCompo.ResetTile();
                SkillStartEvent();
                CheckCanAttack();
                BooleanSkillUse(true);
            }
        }
        
        public void FindEnemyIsThere(GameObject enemy)
        {
            if (enemy == null)
            {
                _targetEnemy = null;
                return;
            }
            
            if (_targetEnemy != null && _targetEnemy != enemy)
                _targetingCompo?.OffTargeting();
            
            Vector2Int enemyPos = GridMap.Instance.WorldToGridPosition(enemy.transform.position);
            
            foreach (var tile in rangeCompo.TilesInRange)
            {
                if (tile.GridPos == enemyPos)
                {
                    _targetEnemy = enemy;
                    return;
                }
            }

            _targetEnemy = null;
        }

        public GameObject GetEnemy()
        {
            return _targetEnemy;
        }

        private void SkillStartEvent()
        {
            Bus<UnitCamSettingEvent>.Raise(new UnitCamSettingEvent(_characterUnit.gameObject, true,
                new Vector3(0.1f, 0.1f, 0.1f)));
            Bus<TurnEndUIEvent>.Raise(new TurnEndUIEvent(true));
            Bus<SendSkillEvent>.Raise(new SendSkillEvent(this));
        }
    }
}
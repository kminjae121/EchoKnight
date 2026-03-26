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
            if(_characterUnit != null)
                impulseSource = _characterUnit.impulseSource;

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
            Bus<SetAtkUIEvent>.Raise(new SetAtkUIEvent(false));
            Bus<UnitCamSettingEvent>.Raise(new UnitCamSettingEvent(null, false,new Vector3(0.1f,0.1f,0.1f)));
            _characterUnit.TurnEnd();
        }

        public override void AttackEnemy()
        {
            if (!isCanUseSkill)
            {
                SkillFinished();
                return;
            }

            if (_targetEnemy == null) return;

            _characterUnit.GaugeManager.UseSkill(SkillSO.UsingSkillCost);
            
            if (RotationCompo != null)
                RotationCompo.SetDir(_targetEnemy.transform.position);
            
            if (_targetingCompo != null)
                _targetingCompo.OffTargeting();
            
            Bus<UnitCamSettingEvent>.Raise(new UnitCamSettingEvent(_characterUnit.gameObject, true,new Vector3(0.1f,0.1f,0.1f)));
            Bus<EnemyHpInfo>.Raise(new EnemyHpInfo(0, 0, 0, 0, false, 
                null,true));
            Bus<SetAtkUIEvent>.Raise(new SetAtkUIEvent());
            
            GridMap.Instance.SetGridVisible(false);
            SkillEvent?.Invoke(_targetEnemy);
            _targetEnemy = null;
           
            SkillFinished();
        }

        public void SetEnemy(GameObject target)
        {
            _targetEnemy = target;
        }

        public override void ShowSkillRange()
        {
            base.ShowSkillRange();

            if (_characterUnit == null || _characterUnit.GaugeManager == null)
                return;

            if (!_characterUnit.GaugeManager.CanUseSkill(SkillSO.UsingSkillCost))
            {
                Bus<SendSkillEvent>.Raise(new SendSkillEvent(null));
                Bus<SetAtkUIEvent>.Raise(new SetAtkUIEvent(false));
                Bus<TurnEndUIEvent>.Raise(new TurnEndUIEvent(false));
                Bus<WarningUIEvent>.Raise(new WarningUIEvent("코스트가 부족합니다"));
                return;
            }

            if (SkillSO.IsOwnSkill)
            {
                SkillStartEvent();
                _characterUnit.GaugeManager.UseSkill(SkillSO.UsingSkillCost);
                _characterUnit.BehaviorCompo.ResetTile();
                SkillEvent?.Invoke(null);
            }
            else
            {
                _characterUnit.BehaviorCompo.ResetTile();
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
            StartEvent();
            
            Bus<SetAtkUIEvent>.Raise(new SetAtkUIEvent(true));
            Bus<UnitCamSettingEvent>.Raise(new UnitCamSettingEvent(_characterUnit.gameObject, true,
                new Vector3(0.1f, 0.1f, 0.1f)));
            Bus<TurnEndUIEvent>.Raise(new TurnEndUIEvent(true));
            Bus<SendSkillEvent>.Raise(new SendSkillEvent(this));
        }
    }
}
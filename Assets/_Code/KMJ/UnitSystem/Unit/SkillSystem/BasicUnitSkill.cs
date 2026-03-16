using System;
using Code.Core.Events.Bus;
using Code.Core.Interfaces;
using Code.Map;
using EnemySystem;
using Input;
using UnitSystem;
using UnityEngine;

namespace Code.UnitSystem.SkillSystem
{
    public class BasicUnitSkill : BaseSkill
    {
        [Header("Basic Settings")]
        [field: SerializeField] public CriticalSpot criticalSpot { get; private set; }
        
        protected CharacterUnit _characterUnit;
        
        private InputReader _inputReader;
        private EnemyTargeting _targetingCompo;

        private void OnEnable()
        {
            base.Start();

            _characterUnit = _owner as CharacterUnit;

            if (_characterUnit != null)
            {
                _inputReader = _characterUnit.InputSO;
                
                if (_inputReader != null)
                {
                    _inputReader.OnAttackEvent -= UseSkill;
                    _inputReader.OnAttackEvent += UseSkill;
                }
            }

            if (_unitBase != null)
            {
                rotationCompo = _unitBase.GetUnitCompo<UnitRotation>();
                triggerCompo = _unitBase.GetUnitCompo<UnitAnimationTrigger>();
                _skillCompo = _unitBase.GetUnitCompo<SkillComponent>();
            }

            skillEndEvent.AddListener(SetMovingTrue);
        }


        public override void OnDisable()
        {
            base.OnDisable();
            
            if (_inputReader != null)
                _inputReader.OnAttackEvent -= UseSkill;
            
            skillEndEvent.RemoveListener(SetMovingTrue);
        }

        public void SetMovingTrue()
        {
            _characterUnit.BehaveCompo.ReCheckInRange();
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

        public override void AttackEnemy()
        {
            if (!isCanUseSkill)
            {
                skillEnd();
                return;
            }

            if (_targetEnemy == null) return;
            

            ownCircleMesh.material = basicMaterial;
            
            _characterUnit.GaugeManager.UseSkill(UseSkillPoint);
            
            if (rotationCompo != null)
                rotationCompo.SetDir(_targetEnemy.transform.position);
            
            if (_targetingCompo != null)
                _targetingCompo.OffTargeting();
            
            Bus<UnitCamSettingEvent>.Raise(new UnitCamSettingEvent(_unitBase.gameObject, true,new Vector3(0.1f,0.1f,0.1f)));
            Bus<EnemyHpInfo>.Raise(new EnemyHpInfo(0, 0, 0, 0, false, 
                null,true));
            Bus<SetAtkUIEvent>.Raise(new SetAtkUIEvent());
            
            skillEvent?.Invoke(_targetEnemy);
            _targetEnemy = null;
           
            skillEnd();
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

            if (!_characterUnit.GaugeManager.CanUseSkill(UseSkillPoint))
            {
                Bus<SendSkillEvent>.Raise(new SendSkillEvent(null));
                Bus<SetAtkUIEvent>.Raise(new SetAtkUIEvent(false));
                Bus<TurnEndUIEvent>.Raise(new TurnEndUIEvent(false));
                Bus<WarningUIEvent>.Raise(new WarningUIEvent("코스트가 부족합니다"));
                return;
            }

            if (ownSkill)
            {
                _characterUnit.GaugeManager.UseSkill(UseSkillPoint);
                SkillStartEvent();
                skillEvent?.Invoke(null);
            }
            else
            {
                SkillStartEvent();
                CheckCanAttack();
                CanUseThisSkill();
            }
        }
        
        public void FindEnemyIsThere(GameObject enemy)
        {
            if (enemy == null)
            {
                _targetEnemy = null;
                return;
            }

            Debug.Log(enemy);
            
            if (_targetEnemy != null && _targetEnemy != enemy)
                _targetingCompo?.OffTargeting();
            
            Vector2Int enemyPos = GridMap.Instance.WorldToGridPosition(enemy.transform.position);
            
            foreach (var tile in _tilesInRange)
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
            Bus<SetAtkUIEvent>.Raise(new SetAtkUIEvent(true));
            Bus<UnitCamSettingEvent>.Raise(new UnitCamSettingEvent(_unitBase.gameObject, true,
                new Vector3(0.1f, 0.1f, 0.1f)));
            Bus<TurnEndUIEvent>.Raise(new TurnEndUIEvent(true));
            Bus<SendSkillEvent>.Raise(new SendSkillEvent(this));
        }
    }
}
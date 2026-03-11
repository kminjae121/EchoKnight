using System.Linq;
using _01.Member.KMJ._02.Scripts.UnitSystem.Unit.UnitComponent;
using _Code.KMJ.UnitSystem.Unit.UnitComponent;
using Code.Core.Events.Bus;
using Code.EntityComponent;
using EnemySystem;
using Input;
using UnitSystem;
using Unity.Cinemachine;
using UnityEngine;

namespace Code.UnitSystem.SkillSystem
{
    public class BasicUnitSkill : BaseSkill
    {
        [Header("Basic Settings")]
        [field: SerializeField] public CriticalSpot criticalSpot { get; private set; }
        protected CharacterUnit characterUnit;
        private InputReader _inputReader;
        private EnemyTargeting _targetingCompo = null;

        protected override void Start()
        {
            base.Start();

            characterUnit = _owner as CharacterUnit;

            if (characterUnit != null)
            {
                _inputReader = characterUnit.InputSO;
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
        }

        public override void OnDisable()
        {
            base.OnDisable();
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

        public override void AttackEnemy()
        {
            base.AttackEnemy();
            
            if (isCanUseSkill)
            {
                GameObject enemy = _inputReader.GetEnemy();
                FindEnemyIsThere(enemy);
                
                if (_targetEnemy != null)
                {
                    ownCircleMesh.material = basicMaterial;
                    characterUnit.GaugeManager.UseSkill(UseSkillPoint);
                    
                    if (rotationCompo != null) rotationCompo.SetDir(enemy.transform.position);
                    if(_targetingCompo != null) _targetingCompo.OffTargeting();
                    
                    Bus<UnitCamSettingEvent>.Raise(new UnitCamSettingEvent(_unitBase.gameObject, true,new Vector3(0.1f,0.1f,0.1f)));
                    Bus<EnemyHpInfo>.Raise(new EnemyHpInfo(0, 0, 0, 0, false, 
                        null,true));
                    Bus<SetAtkUIEvent>.Raise(new SetAtkUIEvent());
                    
                    skillEvent?.Invoke(_targetEnemy);
                    _targetEnemy = null;
                }
            }
            skillEnd(); 
        }

        public void FindEnemyIsThere(GameObject enemy)
        {
            _targetEnemy = null;
            
            if (_verticalCollider != null)
                foreach (var obj in _verticalCollider)
                    if (enemy == obj.gameObject)
                        _targetEnemy = enemy;

            if (_horizontalCollider != null)
                foreach (var obj in _horizontalCollider)
                    if (enemy == obj.gameObject)
                        _targetEnemy = enemy;
        }

        public override void ShowSkillRange()
        {
            base.ShowSkillRange();

            if (characterUnit != null && characterUnit.GaugeManager != null)
                if (characterUnit.GaugeManager.CanUseSkill(UseSkillPoint))
                {
                    if (ownSkill)
                    {
                        characterUnit.GaugeManager.UseSkill(UseSkillPoint);
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
                else
                {
                    Bus<SendSkillEvent>.Raise(new SendSkillEvent(null));
                    Bus<SetAtkUIEvent>.Raise(new SetAtkUIEvent(false));
                    Bus<TurnEndUIEvent>.Raise(new TurnEndUIEvent(false));
                    Bus<WarningUIEvent>.Raise(new WarningUIEvent("코스트가 부족합니다"));
                }
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
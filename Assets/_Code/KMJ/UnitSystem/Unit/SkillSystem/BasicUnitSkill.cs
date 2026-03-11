using Code.Core.Events.Bus;
using Code.Core.Interfaces;
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

        protected override void Start()
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
            
            if (!isCanUseSkill)
            {
                skillEnd();
                return;
            }

            IMapTile tile = _inputReader.GetSelectedTile();

            if (tile == null || !_tilesInRange.Contains(tile) || !tile.HasEnemy)
            {
                skillEnd();
                return;
            }

            GameObject enemy = _inputReader.GetEnemy();
            
            if (enemy == null)
            {
                skillEnd();
                return;
            }

            _targetEnemy = enemy;

            ownCircleMesh.material = basicMaterial;
            
            _characterUnit.GaugeManager.UseSkill(UseSkillPoint);
            
            if (rotationCompo != null)
                rotationCompo.SetDir(enemy.transform.position);
            
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
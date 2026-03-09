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
        [Header("Basic Settings")] [SerializeField]
        protected CriticalSpot criticalSpot;
        protected CharacterUnit characterUnit;
        private InputReader _inputReader;
        private EnemyTargeting _targetingCompo = null;

        protected override void Awake()
        {
            base.Awake();
        }

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

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (_inputReader != null)
                _inputReader.OnAttackEvent -= UseSkill;
        }

        public virtual void Update()
        {
            if (characterUnit != null && characterUnit.isMyTurn && IsActive && _inputReader != null)
            {
                GameObject enemy = _inputReader.GetEnemy();
                characterUnit.BehaveCompo.ResetTile();
                
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
                    }
                }
                else if (enemy != null)
                {
                    FindEnemyIsThere(enemy);

                    if (_targetEnemy != null && _targetingCompo == null)
                    {
                        rotationCompo.SetDir(_targetEnemy.transform.position);
                        criticalSpot.CheckEnemyBody(_damageData,_targetEnemy,damage,addDamage);
                        
                        EntityHealth health = _targetEnemy.GetComponent<EntityHealth>();
                        _targetingCompo = _targetEnemy.GetComponent<EnemyTargeting>();
                        
                        if (_targetingCompo != null) _targetingCompo.Targeting();
                        
                        Bus<EnemyHpInfo>.Raise(new EnemyHpInfo(addDamage, health.CurrentHealth,
                            health.MaxHealth, _damageData.damage, true,
                            _targetEnemy.GetComponent<Unit>().unitSO.UnitImage,true));
                    }
                }
            }
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
                GameObject enemy = null;
                if (_inputReader != null) 
                    enemy = _inputReader.GetEnemy();

                if (enemy == null && _targetEnemy != null)
                    enemy = _targetEnemy;

                if (enemy != null)
                {
                    FindEnemyIsThere(enemy);
                    if (_targetEnemy == null) _targetEnemy = enemy; 
                    
                    if (rotationCompo != null)
                        rotationCompo.SetDir(enemy.transform.position);

                    skillEvent?.Invoke(_targetEnemy);
                    
                    ownCircleMesh.material = basicMaterial;

                    if (_unitBase != null)
                        Bus<UnitCamSettingEvent>.Raise(new UnitCamSettingEvent(_unitBase.gameObject, true,new Vector3(0.1f,0.1f,0.1f)));
                    
                    if (_targetingCompo != null) _targetingCompo.OffTargeting();
                    
                    Bus<EnemyHpInfo>.Raise(new EnemyHpInfo(0, 0, 0, 0, false, 
                        _targetEnemy.GetComponent<Unit>().unitSO.UnitImage,true));
                    
                    if (characterUnit != null && characterUnit.GaugeManager != null)
                        characterUnit.GaugeManager.UseSkill(useSkillPoint);
                    
                    Bus<SetAtkUIEvent>.Raise(new SetAtkUIEvent());
                    
                    _targetEnemy = null;
                }
            }
            skillEnd(); 
        }

        private void FindEnemyIsThere(GameObject enemy)
        {
            _targetEnemy = null;
            
            
            if (_verticalCollider != null)
            {
                foreach (var obj in _verticalCollider)
                {
                    if (enemy == obj.gameObject) _targetEnemy = enemy;
                }
            }

            if (_horizontalCollider != null)
            {
                foreach (var obj in _horizontalCollider)
                {
                    if (enemy == obj.gameObject) _targetEnemy = enemy;
                }
            }
        }

        public override void ShowSkillRange()
        {
            base.ShowSkillRange();
            
            if (characterUnit != null && characterUnit.GaugeManager != null)
            {
                if (characterUnit.GaugeManager.CanUseSkill(useSkillPoint))
                {
                    if (ownSkill)
                    {
                        characterUnit.GaugeManager.UseSkill(useSkillPoint);
                        
                        Bus<SetAtkUIEvent>.Raise(new SetAtkUIEvent(true));
                        Bus<UnitCamSettingEvent>.Raise(new UnitCamSettingEvent(_unitBase.gameObject, true,new Vector3(0.1f,0.1f,0.1f)));
                        Bus<UnitSkilStartEvent>.Raise(new UnitSkilStartEvent(true));
                        
                        skillEvent?.Invoke(null);
                    }
                    else
                    {
                        Bus<SetAtkUIEvent>.Raise(new SetAtkUIEvent(true));
                        Bus<TurnEndUIEvent>.Raise(new TurnEndUIEvent(true));
                        
                        CheckCanAttack();
                        CanUseThisSkill();
                    }
                }
                else
                {
                    Bus<SetAtkUIEvent>.Raise(new SetAtkUIEvent(false));
                    Bus<TurnEndUIEvent>.Raise(new TurnEndUIEvent(false));
                    Bus<WarningUIEvent>.Raise(new WarningUIEvent("코스트가 부족합니다"));
                }
            }
        }
    }
}
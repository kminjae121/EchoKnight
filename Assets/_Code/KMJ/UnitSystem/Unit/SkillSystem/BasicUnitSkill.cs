using System.Linq;
using _01.Member.KMJ._02.Scripts.UnitSystem.Unit.UnitComponent;
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
        
        protected BasicUnit _basicUnit;
        
        private InputReader _inputReader;
        private EnemyTargeting _targetingCompo = null;

        protected override void Awake()
        {
            base.Awake();
            _basicUnit = _owner as BasicUnit;

            if (_basicUnit != null)
            {
                _inputReader = _basicUnit.inputSO;
                if (_inputReader != null)
                {
                    _inputReader.OnAttackEvent += UseSkill;
                }
            }

            if (_unitBase != null)
            {
                rotationCompo = _unitBase.GetUnitCompo<UnitRotation>();
                triggerCompo = _unitBase.GetUnitCompo<UnitAnimationTrigger>();
                _skillCompo = _unitBase.GetUnitCompo<SkillComponent>();
            }
            

            var impulseObj = GameObject.Find("ImpulseSource");
            if (impulseObj) impulseSource = impulseObj.GetComponent<CinemachineImpulseSource>();

            var camObj = GameObject.Find("TopCam");
            if (camObj) unitCam = camObj.GetComponent<SetUnitCamera>();
        }

        protected override void Start()
        {
            base.Start();
        }

        public override void OnDisable()
        {
            base.OnDisable();
            if (_inputReader != null)
                _inputReader.OnAttackEvent -= UseSkill;
        }
        
        private void OnDestroy()
        {
            if (_inputReader != null)
                _inputReader.OnAttackEvent -= UseSkill;
        }

        


        private void Update()
        {
            if (_basicUnit != null && _basicUnit.isMyTurn && _isAct && _inputReader != null)
            {
                GameObject enemy = _inputReader.GetEnemy();
                _basicUnit.behaveCompo.ResetTile();
                if (enemy == null && _targetEnemy != null)
                {
                    _targetingCompo = _targetEnemy.GetComponent<EnemyTargeting>();
                    if (_targetingCompo != null) _targetingCompo.OffTargeting();
                    
                    Bus<EnemyHpInfo>.Raise(new EnemyHpInfo(0, 0, 0, 0, false, 
                        _targetEnemy.GetComponent<Unit>().unitSO.UnitImage,true));

                    Bus<SetAtkUIEvent>.Raise(new SetAtkUIEvent());
                    _targetingCompo = null;
                }
                else if (enemy != null)
                {
                    FindEnemyIsThere(enemy);

                    if (_targetEnemy != null && _targetingCompo == null)
                    {
                        CheckEnemyBody(_targetEnemy);
                        EntityHealth health = _targetEnemy.GetComponent<EntityHealth>();
                        _targetingCompo = _targetEnemy.GetComponent<EnemyTargeting>();
                        if (_targetingCompo != null) _targetingCompo.Targeting();
                        
                        Bus<EnemyHpInfo>.Raise(new EnemyHpInfo(addDamage, health.CurrentHealth,
                            health.MaxHealth, _damageData.damage, true, 
                            _targetEnemy.GetComponent<Unit>().unitSO.UnitImage,true));
                    }
                }
            }
            else
            {
                if (_targetEnemy != null && _targetingCompo != null)
                {
                    _targetingCompo = _targetEnemy.GetComponent<EnemyTargeting>();
                    if (_targetingCompo != null) _targetingCompo.OffTargeting();

                    Bus<EnemyHpInfo>.Raise(new EnemyHpInfo(0, 0, 0, 0, false, 
                        _targetEnemy.GetComponent<Unit>().unitSO.UnitImage,true));
                    _targetingCompo = null;
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

                    if (_unitBase != null)
                        Bus<UnitCamSettingEvent>.Raise(new UnitCamSettingEvent(_unitBase.gameObject, true));
                    
                    if (_targetingCompo != null) _targetingCompo.OffTargeting();
                    
                    Bus<EnemyHpInfo>.Raise(new EnemyHpInfo(0, 0, 0, 0, false, 
                        _targetEnemy.GetComponent<Unit>().unitSO.UnitImage,true));
                    
                    if (_basicUnit != null && _basicUnit.gaugeManager != null)
                        _basicUnit.gaugeManager.UseSkill(useSkillPoint);
                    
                    Bus<SetAtkUIEvent>.Raise(new SetAtkUIEvent());
                    
                    _targetEnemy = null;
                }
            }
            skillEnd(); 
        }

        private void FindEnemyIsThere(GameObject enemy)
        {
            _targetEnemy = null;
            _verticalCollider.ToList().ForEach(obj =>
            {
                if (enemy == obj.gameObject) _targetEnemy = enemy;
            });

            _horizontalCollider.ToList().ForEach(obj =>
            {
                if (enemy == obj.gameObject) _targetEnemy = enemy;
            });
        }

        public override void ShowSkillRange()
        {
            base.ShowSkillRange();
            if (_basicUnit != null && _basicUnit.gaugeManager != null)
            {
                if (_basicUnit.gaugeManager.CanUseSkill(useSkillPoint))
                {
                    if (ownSkill)
                    {
                        Bus<SetAtkUIEvent>.Raise(new SetAtkUIEvent(true));
                        _basicUnit.gaugeManager.UseSkill(useSkillPoint);
                        Bus<UnitCamSettingEvent>.Raise(new UnitCamSettingEvent(_unitBase.gameObject, true));
                        skillEvent?.Invoke(null);
                        Bus<UnitSkilStartEvent>.Raise(new UnitSkilStartEvent(true));
                    }
                    else
                    {
                        Bus<SetAtkUIEvent>.Raise(new SetAtkUIEvent());

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
                    return;
                }
            }
            else
            {

            }   
        }
    }
}
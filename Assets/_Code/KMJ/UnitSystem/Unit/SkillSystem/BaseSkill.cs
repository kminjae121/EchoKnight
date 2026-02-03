using System;
using System.Linq;
using _01.Member.KMJ._02.Scripts.UnitSystem.Unit.UnitComponent;
using Code.Core.Events.Bus;
using Code.Core.Interfaces;
using Code.EntityComponent;
using EnemySystem;
using Input;
using UnitSystem;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Code.UnitSystem.SkillSystem
{
    public abstract class BaseSkill : RangeComponent
    {
        [Header("Base Settings")]
        [SerializeField] protected AttackDataSO attackData;
        [field: SerializeField] public Sprite skillImage { get; set; }
        public float damage;
        public int useSkillPoint;
        [SerializeField] private bool ownSkill = false;

        #region UnitComponent
        protected SkillComponent _skillCompo;
        private UnitRotation rotationCompo;
        protected UnitAnimationTrigger triggerCompo;
        #endregion

        protected CinemachineImpulseSource impulseSource;
        protected GameObject _targetEnemy = null;
        protected DamageData _damageData;
        
        protected Unit _unitBase; 
        protected BasicUnit _basicUnit;

        private InputReader _inputReader;
        private EnemyTargeting _targetingCompo = null;
        private SetUnitCamera unitCam;
        
        public bool isCanUseSkill = false;

        #region SkillEvent
        public UnityEvent skillStartEvent;
        public UnityEvent<GameObject> skillEvent;
        public UnityEvent skillEndEvent;
        #endregion

        protected override void Awake()
        {
            base.Awake();

            _unitBase = _owner as Unit;
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

            _damageData.damage = damage;

            var impulseObj = GameObject.Find("ImpulseSource");
            if (impulseObj) impulseSource = impulseObj.GetComponent<CinemachineImpulseSource>();

            var camObj = GameObject.Find("TopCam");
            if (camObj) unitCam = camObj.GetComponent<SetUnitCamera>();

            skillEndEvent.AddListener(CanUseSkillTrue);
            skillEvent.AddListener(StartSkill);
            ResetTileEvent += skillEnd;
        }

        public virtual void InitializeSkill()
        {
        }

        private void StartSkill(GameObject arg0)
        {
        }

        public virtual void OnDisable()
        {
            skillEndEvent.RemoveListener(CanUseSkillTrue);
            
            if (_inputReader != null)
                _inputReader.OnAttackEvent -= UseSkill;
                
            ResetTileEvent -= skillEnd;
        }

        private void CanUseSkillTrue()
        {
            Bus<UnitSkilStartEvent>.Raise(new UnitSkilStartEvent(false));
            Bus<UsingSkillEvent>.Raise(new UsingSkillEvent(true));
        }

        public virtual void ShowSkillRange()
        {
            if (_basicUnit != null && _basicUnit.gaugeManager != null)
            {
                if (_basicUnit.gaugeManager.CanUseSkill(useSkillPoint))
                {
                    if (ownSkill)
                    {
                        _basicUnit.gaugeManager.UseSkill(useSkillPoint);
                        Bus<UnitCamSettingEvent>.Raise(new UnitCamSettingEvent(_unitBase.gameObject, true));
                        skillEvent?.Invoke(null);
                        Bus<UnitSkilStartEvent>.Raise(new UnitSkilStartEvent(true));
                    }
                    else
                    {
                        Bus<TurnEndUIEvent>.Raise(new TurnEndUIEvent(true));
                        CheckCanAttack();
                        CanUseThisSkill();
                    }
                }
                else
                {
                    Bus<TurnEndUIEvent>.Raise(new TurnEndUIEvent(false));
                    Bus<WarningUIEvent>.Raise(new WarningUIEvent("코스트가 부족합니다"));
                    return;
                }
            }
            else
            {

            }
        }

        private void Update()
        {
            if (_basicUnit != null && _basicUnit.isMyTurn && _isAct && _inputReader != null)
            {
                GameObject enemy = _inputReader.GetEnemy();

                if (enemy == null && _targetEnemy != null)
                {
                    _targetingCompo = _targetEnemy.GetComponent<EnemyTargeting>();
                    if (_targetingCompo != null) _targetingCompo.OffTargeting();
                    
                    Bus<EnemyHpInfo>.Raise(new EnemyHpInfo(0, 0, 0, 0, false, 
                        _targetEnemy.GetComponent<Unit>().unitSO.UnitImage));

                    _targetingCompo = null;
                }
                else if (enemy != null)
                {
                    FindEnemyIsThere(enemy);

                    if (_targetEnemy != null && _targetingCompo == null)
                    {
                        EntityHealth health = _targetEnemy.GetComponent<EntityHealth>();
                        _targetingCompo = _targetEnemy.GetComponent<EnemyTargeting>();
                        if (_targetingCompo != null) _targetingCompo.Targeting();

                        Bus<EnemyHpInfo>.Raise(new EnemyHpInfo(0, health.CurrentHealth,
                            health.MaxHealth, _damageData.damage, true, 
                            _targetEnemy.GetComponent<Unit>().unitSO.UnitImage));
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
                        _targetEnemy.GetComponent<Unit>().unitSO.UnitImage));
                    _targetingCompo = null;
                }
            }
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

        public void CheckCanAttack()
        {
            if (unitCam != null) unitCam.SetThisUnit();
            Bus<UnitAttackControlEvent>.Raise(new UnitAttackControlEvent(true));
            Bus<UnitMoveControlEvent>.Raise(new UnitMoveControlEvent(true));
            FindObjectInRange();
        }

        public void skillEnd()
        {
            BlockThisSkill();
            ResetTile();
            if (unitCam != null) unitCam.EndThisUnit();
        }

        private void OnDestroy()
        {
            if (_inputReader != null)
                _inputReader.OnAttackEvent -= UseSkill;
        }

        public void AttackEnemy()
        {
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
                        _targetEnemy.GetComponent<Unit>().unitSO.UnitImage));
                    
                    if (_basicUnit != null && _basicUnit.gaugeManager != null)
                        _basicUnit.gaugeManager.UseSkill(useSkillPoint);
                    
                    _targetEnemy = null;
                }
            }
            skillEnd(); 
        }

        public void TurnEnd()
        {
            BlockThisSkill();
        }

        public virtual void UseSkill()
        {
            if (isCanUseSkill == false)
                return;

            AttackEnemy();
        }

        public void CanUseThisSkill()
        {
            isCanUseSkill = true;
        }

        public void BlockThisSkill()
        {
            isCanUseSkill = false;
        }
        
        public virtual void ForceUseSkill(GameObject target)
        {
            if (target == null) return;

            _targetEnemy = target;
            isCanUseSkill = true;

            if (rotationCompo != null)
                rotationCompo.SetDir(target.transform.position);

            skillEvent?.Invoke(_targetEnemy);
        }
    }
}
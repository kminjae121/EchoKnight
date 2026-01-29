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
        [SerializeField] protected AttackDataSO attackData;
        
        #region UnitComponent
            protected SkillComponent _skillCompo;
            private UnitRotation rotationCompo; 
            protected UnitAnimationTrigger triggerCompo;
        #endregion

        private EnemyTargeting _targetingCompo = null;
        
        protected CinemachineImpulseSource impulseSource;
        [field: SerializeField] public Sprite skillImage { get; set; }
        
        private InputReader _inputReader;
        
        protected GameObject _targetEnemy = null;
        
        public float damage;

        public int useSkillPoint;

        public bool isCanUseSkill = false;
        
        protected DamageData _damageData;

        private SetUnitCamera unitCam;
        
        private BasicUnit _unit;

        [SerializeField] private bool ownSkill = false;
        
        #region SkillEvent
            public UnityEvent skillStartEvent;
            public UnityEvent<GameObject> skillEvent;
            public UnityEvent skillEndEvent;

        #endregion

        public virtual void InitializeSkill()
        {
            
        }

        protected override void Awake()
        {
            base.Awake();
            
            _unit = _owner as BasicUnit;

            _inputReader = _unit.inputSO;

            _inputReader.OnAttackEvent += UseSkill;

            rotationCompo = _unit.GetUnitCompo<UnitRotation>();
            triggerCompo = _unit.GetUnitCompo<UnitAnimationTrigger>();
            _skillCompo = _unit.GetUnitCompo<SkillComponent>();

            _damageData.damage = damage;
            
            impulseSource = GameObject.Find("ImpulseSource").GetComponent<CinemachineImpulseSource>();


            unitCam = GameObject.Find("TopCam").GetComponent<SetUnitCamera>();

            skillEndEvent.AddListener(CanUseSkillTrue);
            
            skillEvent.AddListener(StartSkill);
            ResetTileEvent += skillEnd;
        }

        private void StartSkill(GameObject arg0)
        {
            
        }

        protected override void Start()
        {
            base.Start();
            
        }


        public virtual void OnDisable()
        {
            skillEndEvent.RemoveListener(CanUseSkillTrue);
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
            if (_unit.gaugeManager.CanUseSkill(useSkillPoint))
            {
                if (ownSkill)
                {
                    _unit.gaugeManager.UseSkill(useSkillPoint);
                    Bus<UnitCamSettingEvent>.Raise(new UnitCamSettingEvent(_unit.gameObject, true));
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

        private void Update()
        {
            if (_unit.isMyTurn && _isAct)
            {
                GameObject enemy = _inputReader.GetEnemy();

                if(enemy == null && _targetEnemy != null)
                {
                    _targetingCompo = _targetEnemy.GetComponent<EnemyTargeting>();
                    
                    _targetingCompo.OffTargeting();
                    Bus<EnemyHpInfo>.Raise(new EnemyHpInfo(0,0,0, 
                        0, false,_targetEnemy.GetComponent<Unit>().unitSO.UnitImage));

                    _targetingCompo = null;
                }
                else if (enemy != null)
                {
                    FindEnemyIsThere(enemy);
                    
                    if (_targetEnemy != null && _targetingCompo == null)
                    {
                        EntityHealth health = _targetEnemy.GetComponent<EntityHealth>();
                        
                        _targetingCompo = _targetEnemy.GetComponent<EnemyTargeting>();
                        _targetingCompo.Targeting();
                        
                        Bus<EnemyHpInfo>.Raise(new EnemyHpInfo(0,health.CurrentHealth, 
                            health.MaxHealth,_damageData.damage, true,_targetEnemy.GetComponent<Unit>().unitSO.UnitImage));
                    }
                }
            }
            else
            {
                if (_targetEnemy != null && _targetingCompo != null) 
                {
                    _targetingCompo = _targetEnemy.GetComponent<EnemyTargeting>();
                    _targetingCompo.OffTargeting();
                    
                    Bus<EnemyHpInfo>.Raise(new EnemyHpInfo(0,0,0, 
                        0, false,_targetEnemy.GetComponent<Unit>().unitSO.UnitImage));
                    _targetingCompo = null;
                }
            }
        }

        private void FindEnemyIsThere(GameObject enemy)
        {
            _verticalCollider.ToList().ForEach(obj =>
            {
                if (enemy == obj.gameObject)
                {
                    _targetEnemy = enemy;
                }
            });
            
            _horizontalCollider.ToList().ForEach(obj =>
            {
                if (enemy == obj.gameObject)
                {
                    _targetEnemy = enemy;
                }
            });
        }

        public void CheckCanAttack()
        {
            unitCam.SetThisUnit();
            Bus<UnitAttackControlEvent>.Raise(new UnitAttackControlEvent(true));
            Bus<UnitMoveControlEvent>.Raise(new UnitMoveControlEvent(true));
            FindObjectInRange();
        }
        
        public void skillEnd()
        {
            BlockThisSkill();
            ResetTile();
            unitCam.EndThisUnit();
        }


        private void OnDestroy()
        {
            _inputReader.OnAttackEvent -= UseSkill;
        }
        

        public void AttackEnemy()
        {
            if (isCanUseSkill)
            {
                GameObject enemy = _inputReader.GetEnemy();

                FindEnemyIsThere(enemy);
            
                rotationCompo.SetDir(enemy.transform.position);
            
                skillEvent?.Invoke(_targetEnemy);
                    
                Bus<UnitCamSettingEvent>.Raise(new UnitCamSettingEvent(_unit.gameObject, true));
                _targetEnemy.GetComponent<EnemyTargeting>().OffTargeting();
                _targetEnemy = null;
                _unit.gaugeManager.UseSkill(useSkillPoint);
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
    }
}
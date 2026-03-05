 using System;
using System.Collections;
using System.Linq;
using _01.Member.KMJ._02.Scripts.UnitSystem.Unit.UnitComponent;
using _Code.KMJ.UnitSystem.Unit.UnitComponent;
using Code.Core.Events.Bus;
using Code.Core.Interfaces;
using Code.EntityComponent;
using EnemySystem;
using EntityComponent;
using GameEventChannel;
using Input;
using UnitSystem;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.UI;

namespace Code.UnitSystem
{
    public class UnitAttackComponent : RangeComponent
    {
        private CinemachineImpulseSource impulseSource;
        [SerializeField] private LayerMask whatIsBody;
        [SerializeField] private UnitRotation rotationCompo; 
        [SerializeField] private AttackDataSO attackData; 
        [SerializeField] private UnitAnimationTrigger triggerCompo;
        [SerializeField] private MeshRenderer ownCircleMesh;



        [SerializeField] private Material CriticalMaterial;
        [SerializeField] private Material basicMaterial;
        
        private float _atkDamage;

        public DamageData _damageData;


        private float addDamage = 0;

        
        private InputReader _inputReader;
        
        private UnitSO _unitSO;
        
        public CharacterUnit _characterUnit { get; set; }

        private GameObject _targetEnemy = null;
        private EnemyTargeting _targetingCompo = null;

        public UnityEvent<GameObject> attackEvent = new UnityEvent<GameObject>();
        public UnityEvent attackStartEvent;
        public UnityEvent attackEndEvent;

        private SetUnitCamera unitCam;

        private UnitCost unitCostCompo;

        
        protected override void Awake()
        {
            if (attackEndEvent == null)
            {
                attackEndEvent = new UnityEvent();
            }

        }

        protected override void Start()
        {
            base.Start();
            
            _characterUnit = _owner as CharacterUnit;
            unitCostCompo = _characterUnit.GetUnitCompo<UnitCost>();
            
            
            _inputReader = _characterUnit.InputSO;
            
            _unitSO = _characterUnit.unitSO;

            _atkDamage = _characterUnit.UnitStatCompo.GetStat<float>(StatInfo.AtkDamage);
            
            Bus<UnitAttackEvent>.Subscribe(CheckCanAttack);
            
            Bus<TopCamEvent>.Subscribe(SetUp);

            ResetTileEvent += EndUnit;
            
            triggerCompo.OnTakeDamageTrigger += TakeDamage;
            
            _damageData = new DamageData();
            _damageData.damage = _atkDamage;

            _inputReader.OnAttackEvent += AttackEnemy;

            attackEndEvent.AddListener(AttackEnded);
        }

        private void SetUp(TopCamEvent evt)
        {
            unitCam = evt.cam.GetComponent<SetUnitCamera>();
        }

        protected override void OnDestroy()
        {
            attackEndEvent.RemoveListener(AttackEnded);
            _inputReader.OnAttackEvent -= AttackEnemy;
            triggerCompo.OnTakeDamageTrigger -= TakeDamage;
            Bus<UnitAttackEvent>.Unsubscribe(CheckCanAttack);
            Bus<TopCamEvent>.Unsubscribe(SetUp);
            ResetTileEvent -= EndUnit;
        }

        private void AttackEnded()
        {
            Bus<UnitSetMoveEvent>.Raise(new UnitSetMoveEvent(true));
        }
        


        private void FindEnemyIsThere(GameObject enemy)
        {
            if (_targetEnemy != null && _targetEnemy != enemy)
            {
                _targetEnemy.GetComponent<EnemyTargeting>().OffTargeting();
            }
            
            _targetEnemy = null;
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

        public void CheckCanAttack(UnitAttackEvent evt)
        {
            if (evt.isAttack)
            {
                if (_characterUnit.isMyTurn)
                {
                    Bus<SetAtkUIEvent>.Raise(new SetAtkUIEvent(true));
                    if (unitCostCompo.GetCurrentCost() - 15 < 0)
                    {
                        Bus<WarningUIEvent>.Raise(new WarningUIEvent("AP가 부족합니다."));
                        return;
                    }
                    unitCam.SetThisUnit();
                    attackStartEvent?.Invoke();
                    FindObjectInRange();
                }
            }
            else
            {
                Bus<SetAtkUIEvent>.Raise(new SetAtkUIEvent(false));
                ResetTile();
                EndAct();
            }
            
        }

        public void EndUnit()
        {
            unitCam.EndThisUnit();
        }

        private void Update()
        {
            if (_characterUnit.isMyTurn && _isAct)
            {
                _characterUnit.BehaveCompo.ResetTile();
                GameObject enemy = _inputReader.GetEnemy();

                if(enemy == null && _targetEnemy != null)
                {
                    _targetingCompo = _targetEnemy.GetComponent<EnemyTargeting>();
                    
                    _targetingCompo.OffTargeting();
                    Bus<EnemyHpInfo>.Raise(new EnemyHpInfo(0,0,0, 
                        0, false,_targetEnemy.GetComponent<Unit>().unitSO.UnitImage,true));

                    _targetingCompo = null;
                }
                else if (enemy != null)
                {
                    FindEnemyIsThere(enemy);
                    
                    
                    if (_targetEnemy != null && _targetingCompo == null)
                    {
                        rotationCompo.SetDir(_targetEnemy.transform.position);
                        
                        EntityHealth health = _targetEnemy.GetComponent<EntityHealth>();
                        
                        _targetingCompo = _targetEnemy.GetComponent<EnemyTargeting>();
                        _targetingCompo.Targeting();
                        
                        CheckEnemyBody(_targetEnemy);
                        Bus<EnemyHpInfo>.Raise(new EnemyHpInfo(addDamage,health.CurrentHealth, 
                            health.MaxHealth,_damageData.damage, true,_targetEnemy.GetComponent<Unit>().unitSO.UnitImage,true));
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
                        0, false,_targetEnemy.GetComponent<Unit>().unitSO.UnitImage,true));
                    _targetingCompo = null;
                }
            }
        }
        

        private void CheckEnemyBody(GameObject target)
        {
            _damageData.damage = _atkDamage;
            addDamage = 0;
            
            Vector3 toAttacker = _characterUnit.transform.position - target.transform.position;
            toAttacker.y = 0f;

            Vector3 enemyForward = target.transform.forward;
            enemyForward.y = 0f;

            toAttacker.Normalize();
            enemyForward.Normalize();

            float dot = Vector3.Dot(enemyForward, toAttacker);
            
            float deadZone = 0.2f;

            BodyType type =
                dot > deadZone ? BodyType.Head :
                dot < -deadZone ? BodyType.Back :
                BodyType.None;

            if (_unitSO.EntityType == EntityType.MeleeAttacker && type == BodyType.Head)
            {
                addDamage = _damageData.damage * 0.4f;
                ownCircleMesh.material = CriticalMaterial;
            }
            else if (_unitSO.EntityType == EntityType.LongRanger && type == BodyType.Back)
            {
                addDamage = _damageData.damage * 0.4f;
                
                ownCircleMesh.material = CriticalMaterial;
            }
            else
            {
                addDamage = 0f;
                
                ownCircleMesh.material = basicMaterial;
            }
        }




        public void AttackEnemy()
        {
            if (_characterUnit.isMyTurn && _isAct)
            {
                _damageData.damage += addDamage;
                GameObject enemy = _inputReader.GetEnemy();

                FindEnemyIsThere(enemy);
                

                if (_targetEnemy == null)
                {
                    return;
                }
                
                Bus<SetAtkUIEvent>.Raise(new SetAtkUIEvent());
                Bus<UnitAttackControlEvent>.Raise(new UnitAttackControlEvent(true));
                
                _targetEnemy.GetComponent<EnemyTargeting>().OffTargeting();
                
                AttackStart();
            }
            ResetTile();
        }

        private void AttackStart()
        {
            if (_targetEnemy != null)
            {
                rotationCompo.SetDir(_targetEnemy.transform.position);
                
                attackEvent?.Invoke(_targetEnemy);
                Bus<EnemyHpInfo>.Raise(new EnemyHpInfo(0,0,0, 
                    0, false,_targetEnemy.GetComponent<Unit>().unitSO.UnitImage,true));
                Bus<UnitCamSettingEvent>.Raise(new UnitCamSettingEvent(this.gameObject, true,new Vector3(0.1f,0.1f,0.1f)));
                unitCostCompo.RemoveCost(15f);   
                ownCircleMesh.material = basicMaterial;
            }
        }

        public void TurnEnd()
        {
                
            EndAct();
        }

        public void TakeDamage()
        {
            Bus<HitStopEvent>.Raise(new HitStopEvent(0.2f,0.25f));
            _characterUnit.impulseSource.GenerateImpulse(0.6f);  
            
            _targetEnemy.GetComponent<EntityHealth>().ApplyDamage(_damageData, 
                _targetEnemy.transform.position,transform.position,attackData,_owner);
            
            Bus<TurnEndUIEvent>.Raise(new TurnEndUIEvent(false)); 
        }
    }
}
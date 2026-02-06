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

namespace Code.UnitSystem
{
    public class UnitAttackComponent : RangeComponent
    {
        private CinemachineImpulseSource impulseSource;
        
        [SerializeField] private UnitRotation rotationCompo; 
        [SerializeField] private AttackDataSO attackData; 
        private float _atkDamage;

        public DamageData _damageData;

        [SerializeField] private UnitAnimationTrigger triggerCompo;



        private float addDamage = 0;

        
        private InputReader _inputReader;
        
        private UnitSO _unitSO;
        
        private BasicUnit _basicUnit;

        private GameObject _targetEnemy = null;
        private EnemyTargeting _targetingCompo = null;

        private bool isAttack = false;

        public UnityEvent<GameObject> attackEvent = new UnityEvent<GameObject>();
        public UnityEvent attackStartEvent;
        public UnityEvent attackEndEvent;

        private SetUnitCamera unitCam;

        
        private void Awake()
        {
            if (attackEndEvent == null)
            {
                attackEndEvent = new UnityEvent();
            }
        }

        protected override void Start()
        {
            base.Start();
            
            _basicUnit = _owner as BasicUnit;
            
            
            _inputReader = _basicUnit.inputSO;
            
            _unitSO = _basicUnit.unitSO;

            _atkDamage = _basicUnit.unitStatCompo.GetStat<float>(StatInfo.AtkDamage);
            
            Bus<UnitAttackEvent>.Subscribe(CheckCanAttack);

            unitCam = GameObject.Find("TopCam").GetComponent<SetUnitCamera>();

            ResetTileEvent += EndUnit;
            
            triggerCompo.OnTakeDamageTrigger += TakeDamage;
            
            _damageData = new DamageData();
            _damageData.damage = _atkDamage;

            _inputReader.OnAttackEvent += AttackEnemy;
            impulseSource = GameObject.Find("ImpulseSource").GetComponent<CinemachineImpulseSource>();

            attackEndEvent.AddListener(AttackEnded);
        }
        
        private void OnDestroy()
        {
            attackEndEvent.RemoveListener(AttackEnded);
            _inputReader.OnAttackEvent -= AttackEnemy;
            triggerCompo.OnTakeDamageTrigger -= TakeDamage;
            Bus<UnitAttackEvent>.Unsubscribe(CheckCanAttack);
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
                if (_basicUnit.isMyTurn)
                {
                    Bus<SetAtkUIEvent>.Raise(new SetAtkUIEvent(true));
                    if (_basicUnit.GetCurrentCost() - 15 < 0)
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
            if (_basicUnit.isMyTurn && _isAct)
            {
                _basicUnit.behaveCompo.ResetTile();
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


        public void AttackEnemy()
        {
            if (_basicUnit.isMyTurn && _isAct)
            {
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
                Bus<UnitCamSettingEvent>.Raise(new UnitCamSettingEvent(this.gameObject, true));
                _basicUnit.RemoveCost(15f);   
            }
        }

        public void TurnEnd()
        {
                
            EndAct();
        }

        public void TakeDamage()
        {
            Bus<HitStopEvent>.Raise(new HitStopEvent(0.2f,0.25f));
            impulseSource.GenerateImpulse(0.6f);  
            
            _targetEnemy.GetComponent<EntityHealth>().ApplyDamage(_damageData, 
                _targetEnemy.transform.position,transform.position,attackData,_owner);
            
            Bus<TurnEndUIEvent>.Raise(new TurnEndUIEvent(false)); 
            Bus<UnitSetMoveEvent>.Raise(new UnitSetMoveEvent(true));
        }
    }
}
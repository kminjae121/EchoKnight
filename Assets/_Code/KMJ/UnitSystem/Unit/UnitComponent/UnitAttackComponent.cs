 using System;
using System.Collections;
using System.Linq;
using _01.Member.KMJ._02.Scripts.UnitSystem.Unit.UnitComponent;
using Code.Core.Events.Bus;
using Code.Core.Interfaces;
using Code.EntityComponent;
using EnemySystem;
using EntityComponent;
using GameEventChannel;
using Input;
using TMPro.EditorUtilities;
using UnitSystem;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;

namespace Code.UnitSystem
{
    public class UnitAttackComponent : RangeComponent
    {
        private CinemachineImpulseSource impulseSource;
        
        private EntityStatCompo _statCompo;
        
        [SerializeField] private StatSO atkDamageStat;
        
        [SerializeField] private UnitRotation rotationCompo; 
        [SerializeField] private AttackDataSO attackData;
        [SerializeField] private float _atkDamage;
        
        private DamageData _damageData;

        [SerializeField] private UnitAnimationTrigger triggerCompo;

        public UnityEvent<GameObject> attackEvent = new UnityEvent<GameObject>();
        
        private InputReader _inputReader;
        
        private UnitSO _unitSO;
        
        private BasicUnit _basicUnit;

        private GameObject _targetEnemy = null;

        private bool isAttack = false;

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

            _statCompo = _basicUnit.GetUnitCompo<EntityStatCompo>();
            
            _inputReader = _basicUnit.inputSO;
            
            _unitSO = _basicUnit.unitSO;
            
            StatSO target = _statCompo.GetStat(atkDamageStat);
            Debug.Assert(target != null, $"{atkDamageStat.statName} does not exist");
            target.OnValueChanged += HandleAtkDamageChanged;
            _atkDamage = target.Value;
            
            
            Bus<UnitAttackEvent>.Subscribe(CheckCanAttack);

            unitCam = GameObject.Find("TopCam").GetComponent<SetUnitCamera>();

            ResetTileEvent += EndUnit;
            
            triggerCompo.OnTakeDamageTrigger += TakeDamage;
            
            _damageData = new DamageData();
            _damageData.damage = _atkDamage;

            _inputReader.OnAttackEvent += AttackEnemy;
            impulseSource = GameObject.Find("ImpulseSource").GetComponent<CinemachineImpulseSource>();
        }
        
        private void OnDestroy()
        {
            _inputReader.OnAttackEvent -= AttackEnemy;
            triggerCompo.OnTakeDamageTrigger -= TakeDamage;
            Bus<UnitAttackEvent>.Unsubscribe(CheckCanAttack);
            
            StatSO target = _statCompo.GetStat(atkDamageStat);
            Debug.Assert(target != null, $"{atkDamageStat.statName} does not exist");
            target.OnValueChanged -= HandleAtkDamageChanged;
            ResetTileEvent -= EndUnit;
        }
        
        private void HandleAtkDamageChanged(StatSO stat, float currentvalue, float previousvalue)
        {
            _atkDamage = _atkDamage + currentvalue;
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

        public void CheckCanAttack(UnitAttackEvent evt)
        {
            if (evt.isAttack)
            {
                if (_basicUnit.isMyTurn)
                {
                    if (_basicUnit.GetCurrentCost() - 25 < 0)
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
                GameObject enemy = _inputReader.GetEnemy();

                FindEnemyIsThere(enemy);

                if (_targetEnemy)
                {
                    _targetEnemy.GetComponent<EnemyTargeting>().Targeting();
                }
            }
            else
            {
                if (_targetEnemy != null)
                {
                    _targetEnemy.GetComponent<EnemyTargeting>().OffTargeting();
                }
            }
        }


        public void AttackEnemy()
        {
            if (_basicUnit.isMyTurn && _isAct)
            {
                _targetEnemy = null;
                
                GameObject enemy = _inputReader.GetEnemy();

                FindEnemyIsThere(enemy);

                if (_targetEnemy == null)
                {
                    //ResetTile();
                    return;
                }
                
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
                
                _basicUnit.RemoveCost(25f);   
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
        }
    }
}
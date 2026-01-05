 using System;
using System.Collections;
using System.Linq;
using _01.Member.KMJ._02.Scripts.UnitSystem.Unit.UnitComponent;
using Code.Core.Events.Bus;
using Code.Core.Interfaces;
using Code.EntityComponent;
using EntityComponent;
using GameEventChannel;
using Input;
using UnitSystem;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;

namespace Code.UnitSystem
{
    public class UnitAttackComponent : RangeComponent, IUnitComponent
    {
        private CinemachineImpulseSource impulseSource;
        
        [SerializeField] private UnitRotation rotationCompo; 
        [SerializeField] private AttackDataSO attackData;
        
        
        private DamageData _damageData;

        [SerializeField] private UnitAnimationTrigger triggerCompo;

        public UnityEvent<GameObject> attackEvent;
        
        private InputReader _inputReader;

        private Unit _owner;
        private UnitSO _unitSO;
        
        private BasicUnit _unit;

        private GameObject _targetEnemy = null;

        private bool isAttack = false;

        public UnityEvent attackStartEvent;
        public UnityEvent attackEndEvent;

        private SetUnitCamera unitCam;


        public void Initialize(Unit owner)
        {
            _owner = owner; 
            
            _unit = _owner as BasicUnit;

            _inputReader = _unit.inputSO;
            
            _unitSO = _unit.unitSO;
            
            Bus<UnitAttackEvent>.Subscribe(CheckCanAttack);

            unitCam = GameObject.Find("TopCam").GetComponent<SetUnitCamera>();
        }
        private void Awake()
        {
            _damageData = new DamageData();
            _damageData.damage = 1.2345f;

            _inputReader.OnAttackEvent += AttackEnemy;
            impulseSource = GameObject.Find("ImpulseSource").GetComponent<CinemachineImpulseSource>();
            attackEndEvent.AddListener(TurnEnd);
        }

        private void Start()
        {
            triggerCompo.OnTakeDamageTrigger += TakeDamage;
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
                if (_unit.isMyTurn)
                {
                    unitCam.SetThisUnit();
                    attackStartEvent?.Invoke();
                    FindObjectInRange();
                }
            }
            else
            {
                ResetsTile();
                EndAct();
            }
            
        }
        
        public void ResetsTile()
        {
            ResetTile();
            
            unitCam.EndThisUnit();
        }


        private void OnDestroy()
        {
            _inputReader.OnAttackEvent -= AttackEnemy;
        }
        

        public void AttackEnemy()
        {
            if (_unit.isMyTurn && _isAct)
            {
                _targetEnemy = null;
                
                GameObject enemy = _inputReader.GetEnemy();

                FindEnemyIsThere(enemy);

                if (_targetEnemy == null)
                {
                    attackEndEvent?.Invoke();
                    ResetTile();
                    return;
                }
                
                rotationCompo.SetDir(_targetEnemy.transform.position);
                
                attackEvent?.Invoke(_targetEnemy);
            }   
            ResetsTile();
        }

        public void TurnEnd()
        {
            _unit.TurnEnd();
                
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
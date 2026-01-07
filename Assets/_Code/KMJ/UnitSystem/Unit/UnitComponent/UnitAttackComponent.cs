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
    public class UnitAttackComponent : RangeComponent
    {
        private CinemachineImpulseSource impulseSource;
        
        [SerializeField] private UnitRotation rotationCompo; 
        [SerializeField] private AttackDataSO attackData;
        
        private DamageData _damageData;

        [SerializeField] private UnitAnimationTrigger triggerCompo;

        public UnityEvent<GameObject> attackEvent;
        
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
            
        }

        protected override void Start()
        {
            base.Start();
            
            _basicUnit = _owner as BasicUnit;

            _inputReader = _basicUnit.inputSO;
            
            _unitSO = _basicUnit.unitSO;
            
            Bus<UnitAttackEvent>.Subscribe(CheckCanAttack);

            unitCam = GameObject.Find("TopCam").GetComponent<SetUnitCamera>();

            ResetTileEvent += EndUnit;
            
            triggerCompo.OnTakeDamageTrigger += TakeDamage;
            
            _damageData = new DamageData();
            _damageData.damage = 1.2345f;

            _inputReader.OnAttackEvent += AttackEnemy;
            impulseSource = GameObject.Find("ImpulseSource").GetComponent<CinemachineImpulseSource>();
            attackEndEvent.AddListener(TurnEnd);
        }
        
        private void OnDestroy()
        {
            _inputReader.OnAttackEvent -= AttackEnemy;
            triggerCompo.OnTakeDamageTrigger -= TakeDamage;
            attackEndEvent.RemoveListener(TurnEnd);
            Bus<UnitAttackEvent>.Unsubscribe(CheckCanAttack);
            ResetTileEvent -= EndUnit;
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


        

        public void AttackEnemy()
        {
            if (_basicUnit.isMyTurn && _isAct)
            {
                _targetEnemy = null;
                
                GameObject enemy = _inputReader.GetEnemy();

                FindEnemyIsThere(enemy);

                if (_targetEnemy == null)
                {
                    ResetTile();
                    return;
                }
                
                rotationCompo.SetDir(_targetEnemy.transform.position);
                
                attackEvent?.Invoke(_targetEnemy);
            }   
            ResetTile();
        }

        public void TurnEnd()
        {
            _basicUnit.TurnEnd();
                
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
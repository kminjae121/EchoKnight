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
    public class UnitAttackComponent : MonoBehaviour, IUnitComponent
    {
        private CinemachineImpulseSource impulseSource;
        
        [SerializeField] private Vector3 _attackVerticalCheckBoxSize;
        [SerializeField] private Vector3 _attackHorizontalCheckBoxSize;

        private Collider[] _attackVerticalCollider;
        private Collider[] _attackHorizontalCollider;
        
        [SerializeField] private LayerMask _whatIsGround;
        
        [SerializeField] private UnitRotation rotationCompo; 
        
        
        private DamageData _damageData;
        [SerializeField] private AttackDataSO attackData;

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
            //attackEndEvent.AddListener(TurnEnd);
        }

        private void Start()
        {
            triggerCompo.OnTakeDamageTrigger += TakeDamage;
        }

        private void FindEnemyIsThere(GameObject enemy)
        {
            _attackVerticalCollider.ToList().ForEach(obj =>
            {
                if (enemy == obj.gameObject)
                {
                    _targetEnemy = enemy;
                }
            });
            
            _attackHorizontalCollider.ToList().ForEach(obj =>
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
                    _attackVerticalCollider = Physics.OverlapBox(transform.position, _attackVerticalCheckBoxSize, Quaternion.identity, _whatIsGround);
                    _attackHorizontalCollider = Physics.OverlapBox(transform.position, _attackHorizontalCheckBoxSize, Quaternion.identity, _whatIsGround);
            
                    _attackVerticalCollider.ToList().ForEach(obj =>
                    {
                        if (obj.TryGetComponent(out IMapTile tile))
                        {
                            if (!tile.HasObstacle)    
                            {
                                tile.SetWalkable(true);      
                            }
                        }
                    });
            
                    _attackHorizontalCollider.ToList().ForEach(obj =>
                    {
                        if (obj.TryGetComponent(out IMapTile tile))
                        {
                            if (!tile.HasObstacle)
                            {
                                tile.SetWalkable(true);
                            }
                        }
                    });
                    isAttack = true;
                }
            }
            else
            {
                ResetTile();
                isAttack = false;
            }
            
        }
        
        public void ResetTile()
        {
            if (_attackHorizontalCollider == null && _attackVerticalCollider == null)
                return;
            
            _attackHorizontalCollider.ToList().ForEach(obj =>
            {
                if (obj.TryGetComponent(out IMapTile tile))
                {
                    if (!tile.HasObstacle)
                    {
                        tile.SetWalkable(false);
                    }
                }
            });
            
            _attackVerticalCollider.ToList().ForEach(obj =>
            {
                if (obj.TryGetComponent(out IMapTile tile))
                {
                    if (!tile.HasObstacle)
                    {
                        tile.SetWalkable(false);
                    }
                }
            });
            
            _attackHorizontalCollider.ToList().Clear();
            _attackVerticalCollider.ToList().Clear();

            isAttack = true;
            
            unitCam.EndThisUnit();
        }


        private void OnDestroy()
        {
            _inputReader.OnAttackEvent -= AttackEnemy;
        }
        

        public void AttackEnemy()
        {
            if (_unit.isMyTurn && isAttack)
            {
                GameObject enemy = _inputReader.GetEnemy();

                FindEnemyIsThere(enemy);
                
                rotationCompo.SetDir(enemy.transform.position);
                
                attackEvent?.Invoke(_targetEnemy);      
            }   
            ResetTile();
        }

        public void TurnEnd()
        {
            _unit.TurnEnd();
                
            isAttack = false;
        }

        public void TakeDamage()
        {
            Bus<HitStopEvent>.Raise(new HitStopEvent(0.3f));
            impulseSource.GenerateImpulse(0.6f);  
            
            _targetEnemy.GetComponent<EntityHealth>().ApplyDamage(_damageData, 
                _targetEnemy.transform.position,transform.position,attackData,_owner);
        }


        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, _attackVerticalCheckBoxSize);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(transform.position, _attackHorizontalCheckBoxSize);
        }
    }
}
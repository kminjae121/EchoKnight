using _Code.KMJ.UnitSystem.Unit.UnitComponent;
using Code.Core.Events.Bus;
using Code.EntityComponent;
using EnemySystem;
using Input;
using UnitSystem;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;

namespace Code.UnitSystem
{
    public class UnitAttackComponent : RangeComponent
    {
        [SerializeField] private LayerMask whatIsBody;
        [SerializeField] private UnitAnimationTrigger triggerCompo;
        [SerializeField] private UnitRotation rotationCompo; 
        [SerializeField] private MeshRenderer ownCircleMesh;
        [SerializeField] private Material CriticalMaterial;
        [SerializeField] private Material basicMaterial;
        
        public CharacterUnit _characterUnit { get; private set; }
        private UnitCostComponent _unitCostComponentCompo;
        
        private InputReader _inputReader;
        private UnitSO _unitSO;
        private CinemachineImpulseSource _impulseSource;
        
        public DamageData _damageData;
        private float _atkDamage;
        private float addDamage;

        private GameObject _targetEnemy;
        private EnemyTargeting _targetingCompo;

        public UnityEvent<GameObject> attackEvent = new();
        public UnityEvent attackStartEvent;
        public UnityEvent attackEndEvent;
        
        protected override void Awake()
        {
            attackEndEvent ??= new UnityEvent();
        }

        protected override void Start()
        {
            base.Start();
            
            _characterUnit = _owner as CharacterUnit;
            _unitCostComponentCompo = _characterUnit.GetUnitCompo<UnitCostComponent>();
            
            _unitSO = _characterUnit.unitSO;
            _inputReader = _characterUnit.InputSO;
            
            _atkDamage = _characterUnit.UnitStatCompo.GetStat<float>(StatInfo.AtkDamage);
            _damageData = new DamageData();
            _damageData.damage = _atkDamage;

            Bus<UnitAttackEvent>.Subscribe(CheckCanAttack);
            _inputReader.OnAttackEvent += AttackEnemy;
            attackEndEvent.AddListener(AttackEnded);
        }


        protected override void OnDestroy()
        {
            attackEndEvent.RemoveListener(AttackEnded);
            _inputReader.OnAttackEvent -= AttackEnemy;
            Bus<UnitAttackEvent>.Unsubscribe(CheckCanAttack);
        }
        
        private void FindEnemyIsThere(GameObject enemy)
        {
            if (_targetEnemy != null && _targetEnemy != enemy)
                _targetEnemy.GetComponent<EnemyTargeting>().OffTargeting();
            
            _targetEnemy = null;

            foreach (var obj in _verticalCollider)
                if (enemy == obj.gameObject)
                    _targetEnemy = enemy;

            foreach (var obj in _horizontalCollider)
                if (enemy == obj.gameObject)
                    _targetEnemy = enemy;
        }
        
        private void AttackEnded()
        {
            Bus<UnitSetMoveEvent>.Raise(new UnitSetMoveEvent(true));
        }

        public void CheckCanAttack(UnitAttackEvent evt)
        {
            if (evt.isAttack)
            {
                if (_characterUnit.isMyTurn)
                {
                    Bus<SetAtkUIEvent>.Raise(new SetAtkUIEvent(true));
                    
                    //임시 후에 수정
                    if (_unitCostComponentCompo.GetCurrentCost() - 15 < 0)
                    {
                        Bus<WarningUIEvent>.Raise(new WarningUIEvent("AP가 부족합니다."));
                        return;
                    }
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


        private void Update()
        {
            if (_characterUnit.isMyTurn && IsActive)
            {
                _characterUnit.BehaveCompo.ResetTile();
                GameObject enemy = _inputReader.GetEnemy();

                if(enemy == null)
                {
                    if (_targetEnemy != null)
                    {
                        _targetingCompo = _targetEnemy.GetComponent<EnemyTargeting>();
                        _targetingCompo.OffTargeting();
                        
                        Bus<EnemyHpInfo>.Raise(new EnemyHpInfo(0,0,0, 
                            0, false,_targetEnemy.GetComponent<Unit>().unitSO.UnitImage,true));

                        _targetingCompo = null;
                    }
                }
                else
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
            if (_characterUnit.isMyTurn && IsActive)
            {
                _damageData.damage += addDamage;
                GameObject enemy = _inputReader.GetEnemy();

                FindEnemyIsThere(enemy);
                
                if (_targetEnemy == null)
                    return;
                
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
                
                Bus<EnemyHpInfo>.Raise(new EnemyHpInfo(0, 0, 0,
                    0, false, _targetEnemy.GetComponent<Unit>().unitSO.UnitImage, true));
                
                Bus<UnitCamSettingEvent>.Raise(new UnitCamSettingEvent(this.gameObject,
                    true,new Vector3(0.1f,0.1f,0.1f)));
                
                //예비 후에 수정
                _unitCostComponentCompo.RemoveCost(15f);   
                ownCircleMesh.material = basicMaterial;
            }
        }
    }
}
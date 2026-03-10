using System.Linq;
using _01.Member.KMJ._02.Scripts.UnitSystem.Unit.UnitComponent;
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
        [field: SerializeField] public UnitRotation RotationCompo { get; private set; }
        [field : SerializeField] public CriticalSpot CriticalSpot { get; private set; }
        
        
        [SerializeField] private UnitAnimationTrigger triggerCompo;
        [SerializeField] private LayerMask whatIsBody;
        [SerializeField] private MeshRenderer ownCircleMesh;
        [SerializeField] private Material CriticalMaterial;
        [SerializeField] private Material basicMaterial;
        
        public CharacterUnit CharacterUnit { get; set; }
        public float AtkDamage { get; private set; }
        public float AddDamage { get; private set; }
        
        
        private CinemachineImpulseSource _impulseSource;
        private InputReader _inputReader;
        private UnitCostComponent _unitCostComponentCompo;
        private UnitSO _unitSO;
        
        public DamageData DamageData;

        private GameObject _targetEnemy;
        private EnemyTargeting _targetingCompo;
        private Unit targetUnit;

        public UnityEvent<GameObject> attackEvent = new();
        public UnityEvent attackStartEvent;
        public UnityEvent attackEndEvent;
        
        protected override void Awake()
        {
            if (attackEndEvent == null)
                attackEndEvent = new UnityEvent();
        }

        protected override void Start()
        {
            base.Start();
            
            CharacterUnit = _owner as CharacterUnit;
            _unitCostComponentCompo = CharacterUnit.GetUnitCompo<UnitCostComponent>();
            
            Bus<UnitAttackEvent>.Subscribe(CheckCanAttack);
            _inputReader.OnAttackEvent += AttackEnemy;
            attackEndEvent.AddListener(AttackEnded);
            
            _unitSO = CharacterUnit.unitSO;
            _inputReader = CharacterUnit.InputSO;
            
            AtkDamage = CharacterUnit.UnitStatCompo.GetStat<float>(StatInfo.AtkDamage);
            DamageData = new DamageData();
            DamageData.damage = AtkDamage;
        }


        protected override void OnDestroy()
        {
            attackEndEvent.RemoveListener(AttackEnded);
            _inputReader.OnAttackEvent -= AttackEnemy;
            
            Bus<UnitAttackEvent>.Unsubscribe(CheckCanAttack);
        }
        
        public void FindEnemyIsThere(GameObject enemy)
        {
            if (_targetEnemy != null && _targetEnemy != enemy)
                _targetingCompo.OffTargeting();
            
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
                if (CharacterUnit.isMyTurn)
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

        public void AttackEnemy()
        {
            if (CharacterUnit.isMyTurn && IsActive)
            {
                DamageData.damage += AddDamage;
                GameObject enemy = _inputReader.GetEnemy();

                FindEnemyIsThere(enemy);
                
                if (_targetEnemy == null)
                    return;
                
                Bus<SetAtkUIEvent>.Raise(new SetAtkUIEvent());
                Bus<UnitAttackControlEvent>.Raise(new UnitAttackControlEvent(true));
                
                _targetingCompo.OffTargeting();
                AttackStart();
            }
            ResetTile();
        }

        private void AttackStart()
        {
            if (_targetEnemy != null)
            {
                RotationCompo.SetDir(_targetEnemy.transform.position);
                
                attackEvent?.Invoke(_targetEnemy);
                
                Bus<EnemyHpInfo>.Raise(new EnemyHpInfo(0, 0, 0,
                    0, false, targetUnit.unitSO.UnitImage, true));
                
                Bus<UnitCamSettingEvent>.Raise(new UnitCamSettingEvent(this.gameObject,
                    true,new Vector3(0.1f,0.1f,0.1f)));
                
                //예비 후에 수정
                _unitCostComponentCompo.RemoveCost(15f);   
                ownCircleMesh.material = basicMaterial;
            }
        }
    }
}
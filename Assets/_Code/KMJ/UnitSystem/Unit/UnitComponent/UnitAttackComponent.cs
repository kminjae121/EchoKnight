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
        [field: SerializeField] public CriticalSpot CriticalSpot { get; private set; }
        
        [SerializeField] private MeshRenderer ownCircleMesh;    
        [SerializeField] private Material basicMaterial;

        public CharacterUnit CharacterUnit { get; set; }
        public float AtkDamage { get; private set; }
        public float AddDamage { get; private set; }

        private const float AttackCost = 15f;
        
        private InputReader _inputReader;
        private UnitCostComponent _unitCostComponentCompo;

        public DamageData DamageData;

        private GameObject _targetEnemy;
        private EnemyTargeting _targetingCompo;

        public UnityEvent<GameObject> attackEvent;
        public UnityEvent attackStartEvent;
        public UnityEvent attackEndEvent;
        
        protected override void Start()
        {
            base.Start();

            CharacterUnit = _owner as CharacterUnit;
            _unitCostComponentCompo = CharacterUnit.GetUnitCompo<UnitCostComponent>();

            Bus<UnitAttackEvent>.Subscribe(CheckCanAttack);
            attackEndEvent.AddListener(AttackEnded);
            
            _inputReader = CharacterUnit.InputSO;
            _inputReader.OnAttackEvent += AttackEnemy;

            AtkDamage = CharacterUnit.UnitStatCompo.GetStat<float>(StatInfo.AtkDamage);

            DamageData = new DamageData
            {
                damage = AtkDamage
            };
        }

        protected override void OnDestroy()
        {
            attackEndEvent?.RemoveListener(AttackEnded);

            if (_inputReader != null)
                _inputReader.OnAttackEvent -= AttackEnemy;

            Bus<UnitAttackEvent>.Unsubscribe(CheckCanAttack);

            base.OnDestroy();
        }

        public void SetTargeting(EnemyTargeting targetingCompo) => 
            _targetingCompo = targetingCompo;

        private float ComputeDamage() => AtkDamage + AddDamage;

        private bool HasEnoughCost()
        {
            if (_unitCostComponentCompo == null) return false;
            return _unitCostComponentCompo.GetCurrentCost() - AttackCost >= 0;
        }

        public void FindEnemyIsThere(GameObject enemy)
        {
            if (enemy == null)
            {
                _targetEnemy = null;
                return;
            }

            if (_targetEnemy != null && _targetEnemy != enemy) _targetingCompo?.OffTargeting();

            _targetEnemy = null;

            foreach (var obj in _verticalCollider)
                if (enemy == obj.gameObject)
                {
                    _targetEnemy = enemy;
                    return;
                }

            foreach (var obj in _horizontalCollider)
                if (enemy == obj.gameObject)
                {
                    _targetEnemy = enemy;
                    return;
                }
        }

        private void AttackEnded()
        {
            Bus<UnitSetMoveEvent>.Raise(new UnitSetMoveEvent(true));
        }

        public void CheckCanAttack(UnitAttackEvent evt)
        {
            if (evt.isAttack)
            {
                if (!CharacterUnit.isMyTurn) return;
                Bus<SetAtkUIEvent>.Raise(new SetAtkUIEvent(true));
                
                if (!HasEnoughCost())
                {
                    Bus<WarningUIEvent>.Raise(new WarningUIEvent("AP가 부족합니다."));
                    return;
                }

                attackStartEvent?.Invoke();
                FindObjectInRange();
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
            if (!(CharacterUnit.isMyTurn && IsActive)) return;

            var enemy = _inputReader.GetEnemy();
            FindEnemyIsThere(enemy);
            
            if (_targetEnemy == null) return;
            
            DamageData.damage = ComputeDamage();

            Bus<SetAtkUIEvent>.Raise(new SetAtkUIEvent());
            Bus<UnitAttackControlEvent>.Raise(new UnitAttackControlEvent(true));

            _targetingCompo?.OffTargeting();
            AttackStart();
            ResetTile();
        }

        private void AttackStart()
        {
            if (_targetEnemy == null)
                return;
            
            if (!HasEnoughCost())
            {
                Bus<WarningUIEvent>.Raise(new WarningUIEvent("AP가 부족합니다."));
                return;
            }
            _unitCostComponentCompo.RemoveCost(AttackCost);

            RotationCompo.SetDir(_targetEnemy.transform.position);

            attackEvent?.Invoke(_targetEnemy);

            Bus<EnemyHpInfo>.Raise(new EnemyHpInfo(0, 0, 0,
                0, false, null, true));

            Bus<UnitCamSettingEvent>.Raise(new UnitCamSettingEvent(this.gameObject,
                true, new Vector3(0.1f, 0.1f, 0.1f)));

            ownCircleMesh.material = basicMaterial;
        }
    }
}
using System;
using Code.Core.Events.Bus;
using Code.Core.Interfaces;
using Code.Map;
using Code.UnitSystem.Combat;
using EnemySystem;
using Input;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;

namespace Code.UnitSystem.SkillSystem
{
    public enum SkillType
    {
        BasicSkill,
        ActiveSkill,
    }

    public abstract class BaseSkill : RangeComponent
    {
        [Header("Base Settings")]
        [field: SerializeField] public SkillSO SkillSO { get; private set; }
        [SerializeField] protected AttackDataSO attackData;
        [field: SerializeField] public float basicSkillDamage { get; private set; }
        
        [Header("Basic Skill Settings")]
        [field: SerializeField] public CriticalSpot CriticalSpot { get; private set; }

        public float AddDamage { get; private set; } = 0;
        public float Damage { get; set; }
        protected int SkillRange { get; private set; }

        [Header("Unit Component")]
        protected CharacterUnit _characterUnit; // CharacterUnit으로 캐스팅된 오너
        protected SkillComponent _skillCompo;
        [SerializeField] protected UnitAnimationTrigger triggerCompo;
        [SerializeField] private UnitStatCompo statCompo;
        public UnitRotation RotationCompo { get; set; }
        
        protected InputReader _inputReader;
        protected EnemyTargeting _targetingCompo;

        [Header("Skill Event")]
        public UnityEvent SkillStartEvent; // UnityEvent 필드
        public UnityEvent<GameObject> SkillEvent;
        public UnityEvent SkillEndEvent;

        [Header("Camera & Effects")]
        protected CinemachineImpulseSource impulseSource;
        
        public DamageData DamageData;
        protected Unit _unitBase; 
        protected GameObject _targetEnemy = null;
        public bool isCanUseSkill = false;

        #region Unity Lifecycle

        protected override void Awake()
        {
            _unitBase = _owner;
            _characterUnit = _owner as CharacterUnit; // BasicUnitSkill의 Awake 로직 통합
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();
        }

        protected virtual void OnEnable()
        {
            // BasicUnitSkill의 OnEnable 로직 통합
            if (_characterUnit != null)
            {
                impulseSource = _characterUnit.impulseSource;
                _inputReader = _characterUnit.InputSO;
                
                if (_inputReader != null)
                {
                    _inputReader.OnAttackEvent -= UseSkill;
                    _inputReader.OnAttackEvent += UseSkill;
                }
            }

            if (_unitBase != null)
            {
                RotationCompo = _unitBase.GetUnitCompo<UnitRotation>();
                triggerCompo = _unitBase.GetUnitCompo<UnitAnimationTrigger>();
                _skillCompo = _unitBase.GetUnitCompo<SkillComponent>();
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            SkillEndEvent.RemoveListener(CanUseSkillTrue);
            
            if (_inputReader != null)
                _inputReader.OnAttackEvent -= UseSkill;
        }

        #endregion

        #region Setup Methods

        public virtual void InitializeSkill()
        {
            SkillEndEvent.AddListener(CanUseSkillTrue);
            SkillEvent.AddListener(StartSkill);
        }

        public void ConfigureSkillRange(SkillSO skillData)
        {
            SkillRange = skillData == null ? 0 : Mathf.Max(0, Mathf.RoundToInt(skillData.SkillRange));
        }

        public void SetEnemyTargeting(EnemyTargeting targeting)
        {
            _targetingCompo = targeting;
        }

        public void SetDamage(float damage)
        {
            DamageData.damage = damage += AddDamage;
        }

        public void SetAddDamage(float addDamage)
        {
            AddDamage = addDamage;
        }

        #endregion

        #region Skill Logic

        private void StartSkill(GameObject arg0) { }

        protected virtual void StartEvent() { }

        protected virtual void RemoveEvent() { }

        protected virtual void CanUseSkillTrue()
        {
            Bus<UnitSkilStartEvent>.Raise(new UnitSkilStartEvent(false));
            Bus<UsingSkillEvent>.Raise(new UsingSkillEvent(true));
        }

        public virtual void ShowSkillRange()
        {
            if (_characterUnit == null || _characterUnit.GaugeManager == null)
                return;

            if (!_characterUnit.GaugeManager.CanUseSkill(SkillSO.UsingSkillCost))
            {
                Bus<SendSkillEvent>.Raise(new SendSkillEvent(null));
                Bus<SetAtkUIEvent>.Raise(new SetAtkUIEvent(false));
                Bus<TurnEndUIEvent>.Raise(new TurnEndUIEvent(false));
                Bus<WarningUIEvent>.Raise(new WarningUIEvent("코스트가 부족합니다"));
                return;
            }

            if (SkillSO.IsOwnSkill)
            {
                ProcessSkillStart();
                _characterUnit.GaugeManager.UseSkill(SkillSO.UsingSkillCost);
                _characterUnit.BehaviorCompo.ResetTile();
                SkillEvent?.Invoke(null);
            }
            else
            {
                _characterUnit.BehaviorCompo.ResetTile();
                ProcessSkillStart();
                CheckCanAttack();
                BooleanSkillUse(true);
            }
        }

        public virtual void CheckCanAttack()
        {
            Bus<UnitAttackControlEvent>.Raise(new UnitAttackControlEvent(true));
            Bus<UnitMoveControlEvent>.Raise(new UnitMoveControlEvent(true));
            FindObjectInRange();
        }

        public virtual void SkillFinished()
        {
            BooleanSkillUse(false);
            ResetTile();    
        }

        public virtual void UseSkill()
        {
            if (isCanUseSkill == false)
                return;

            AttackEnemy();
        }

        public virtual void AttackEnemy()
        {
            if (!isCanUseSkill)
            {
                SkillFinished();
                return;
            }

            if (_targetEnemy == null) return;

            _characterUnit.GaugeManager.UseSkill(SkillSO.UsingSkillCost);
            
            if (RotationCompo != null)
                RotationCompo.SetDir(_targetEnemy.transform.position);
            
            if (_targetingCompo != null)
                _targetingCompo.OffTargeting();
            
            Bus<UnitCamSettingEvent>.Raise(new UnitCamSettingEvent(_unitBase.gameObject, true, new Vector3(0.1f, 0.1f, 0.1f)));
            Bus<EnemyHpInfo>.Raise(new EnemyHpInfo(0, 0, 0, 0, false, null, true));
            Bus<SetAtkUIEvent>.Raise(new SetAtkUIEvent());
            
            GridMap.Instance.SetGridVisible(false);
            SkillEvent?.Invoke(_targetEnemy);
            _targetEnemy = null;
           
            SkillFinished();
        }

        protected virtual void SkillEnd()
        {
            Bus<SetAtkUIEvent>.Raise(new SetAtkUIEvent(false));
            Bus<UnitCamSettingEvent>.Raise(new UnitCamSettingEvent(null, false, new Vector3(0.1f, 0.1f, 0.1f)));
            _characterUnit.TurnEnd();
        }

        public virtual void ForceUseSkill(GameObject target)
        {
            if (target == null) return;
            
            _targetEnemy = target;
            isCanUseSkill = true;

            if (RotationCompo != null)
                RotationCompo.SetDir(target.transform.position);

            StartEvent();
            SkillEvent?.Invoke(_targetEnemy);
        }

        #endregion

        #region Enemy & Range Helpers

        public void SetEnemy(GameObject target) => _targetEnemy = target;
        public GameObject GetEnemy() => _targetEnemy;
        public void BooleanSkillUse(bool isSkill) => isCanUseSkill = isSkill;

        public void FindEnemyIsThere(GameObject enemy)
        {
            if (enemy == null)
            {
                _targetEnemy = null;
                return;
            }
            
            if (_targetEnemy != null && _targetEnemy != enemy)
                _targetingCompo?.OffTargeting();
            
            Vector2Int enemyPos = GridMap.Instance.WorldToGridPosition(enemy.transform.position);
            
            foreach (var tile in _tilesInRange)
            {
                if (tile.GridPos == enemyPos)
                {
                    _targetEnemy = enemy;
                    return;
                }
            }
            _targetEnemy = null;
        }

        protected override int GetRange() => SkillRange;

        protected override void CalculateRange()
        {
            _tilesInRange.Clear();
            Vector2Int start = GridMap.Instance.WorldToGridPosition(transform.position);
            int range = GetRange();

            for (int x = -range; x <= range; x++)
            {
                int remain = range - Mathf.Abs(x);
                for (int y = -remain; y <= remain; y++)
                {
                    if (x == 0 && y == 0) continue;

                    Vector2Int position = start + new Vector2Int(x, y);
                    IMapTile tile = GridMap.Instance.GetTile(position);

                    if (tile != null)
                        _tilesInRange.Add(tile);
                }
            }
        }
        private void ProcessSkillStart()
        {
            StartEvent();
            Bus<SetAtkUIEvent>.Raise(new SetAtkUIEvent(true));
            Bus<UnitCamSettingEvent>.Raise(new UnitCamSettingEvent(_unitBase.gameObject, true, new Vector3(0.1f, 0.1f, 0.1f)));
            Bus<TurnEndUIEvent>.Raise(new TurnEndUIEvent(true));
            Bus<SendSkillEvent>.Raise(new SendSkillEvent(this));
        }

        #endregion
    }
}
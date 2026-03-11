using Code.AttackSystem;
using _Code.KMJ.Cam;
using Code.UnitSystem;
using Code.Core.Events.Bus;
using Code.EntityComponent;
using UnitSystem;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;

namespace Code.UnitSystem.SkillSystem
{
    public abstract class BaseSkill : RangeComponent
    {
        [Header("Base Settings")]
        [field: SerializeField] public Sprite SkillImage { get; set; }
        [SerializeField] protected AttackDataSO attackData;
        [SerializeField] private float basicSkillDamage;
        [SerializeField] protected bool ownSkill = false;
        
        public DamageData DamageData;
        public int UseSkillPoint;
        public bool isCanUseSkill = false;
        
        public float AddDamage { get; private set; }
        public UnitRotation rotationCompo { get; set; }
        public float damage { get; set; }
        
        
        protected Unit _unitBase; 
        protected GameObject _targetEnemy = null;

        [Header("Unit Component")]
        protected SkillComponent _skillCompo;
        protected UnitAnimationTrigger triggerCompo;
        [SerializeField] private UnitStatCompo statCompo;

        [Header("Skill Event")]
        public UnityEvent skillStartEvent;
        public UnityEvent<GameObject> skillEvent;
        public UnityEvent skillEndEvent;

        [Header("Camera & Effects")]
        protected CinemachineImpulseSource impulseSource;

        [Header("Materials & Mesh")]
        [SerializeField] protected MeshRenderer ownCircleMesh;
        [SerializeField] protected Material CriticalMaterial;
        [SerializeField] protected Material basicMaterial;
        
        protected override void Awake()
        {
            base.Awake();

            skillEndEvent.AddListener(CanUseSkillTrue);
            skillEvent.AddListener(StartSkill);
            _resetTileEvent += skillEnd;
        }

        protected override void Start()
        {
            base.Start();

            _unitBase = _owner;

            if (_unitBase != null && statCompo == null)
                statCompo = _unitBase.GetUnitCompo<UnitStatCompo>();
            
            
            if (statCompo != null)
            {
                float skillDamageValue = statCompo.GetStat<float>(StatInfo.SkillDamage);
                float floatdamage = basicSkillDamage * skillDamageValue;
                damage = (int)floatdamage;
            }
            else
                damage = basicSkillDamage;

            DamageData.damage = damage;

            if (_unitBase as CharacterUnit)
            {
                CharacterUnit unit = _unitBase as CharacterUnit;
                impulseSource = unit.impulseSource;
            }
        }
        

        public virtual void InitializeSkill()
        {
        }

        private void StartSkill(GameObject arg0)
        {
        }

        public virtual void OnDisable()
        {
            skillEndEvent.RemoveListener(CanUseSkillTrue);
            _resetTileEvent -= skillEnd;
        }
        
        protected virtual void CanUseSkillTrue()
        {
        }
        
        public virtual void ShowSkillRange()
        {
        }
        

        public virtual void CheckCanAttack()
        {
            Bus<UnitAttackControlEvent>.Raise(new UnitAttackControlEvent(true));
            Bus<UnitMoveControlEvent>.Raise(new UnitMoveControlEvent(true));
            
            FindObjectInRange();
        }

        public virtual void skillEnd()
        {
            BlockThisSkill();
            ResetTile();
            
            skillEndEvent?.Invoke();
        }

        public virtual void AttackEnemy()
        {
        }

        public void TurnEnd()
        {
            BlockThisSkill();
        }

        public virtual void UseSkill()
        {
            if (isCanUseSkill == false)
                return;

            AttackEnemy();
        }

        public void CanUseThisSkill()
        {
            isCanUseSkill = true;
        }

        public void BlockThisSkill()
        {
            isCanUseSkill = false;
        }
        
        public virtual void ForceUseSkill(GameObject target)
        {
            if (target == null) return;

            _targetEnemy = target;
            isCanUseSkill = true;

            if (rotationCompo != null)
                rotationCompo.SetDir(target.transform.position);

            skillEvent?.Invoke(_targetEnemy);
        }
    }
}
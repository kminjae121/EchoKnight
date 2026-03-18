using Code.Core.Events.Bus;
using Code.UnitSystem.Combat;
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
        [field: SerializeField] public Sprite SkillImage { get; set; }
        [SerializeField] protected AttackDataSO attackData;
        [field: SerializeField] public float basicSkillDamage { get; private set; }
        [SerializeField] protected bool ownSkill = false;

        [field: SerializeField] public SkillType SkillType { get; protected set; } = SkillType.ActiveSkill;
        
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
        [SerializeField] protected UnitAnimationTrigger triggerCompo;
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
            _unitBase = _owner;
            base.Awake();
        }

        public virtual void InitializeSkill()
        {
            skillEndEvent.AddListener(CanUseSkillTrue);
            skillEvent.AddListener(StartSkill);
        }
        
        
        public virtual void OnDisable()
        {
            skillEndEvent.RemoveListener(CanUseSkillTrue);
        }
        
        public void SetDamage(float damage)
        {
            DamageData.damage = damage;
        }

        private void StartSkill(GameObject arg0)
        {
        }
        public void SetAddDamage(float addDamage)
        {
            this.AddDamage = addDamage;
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
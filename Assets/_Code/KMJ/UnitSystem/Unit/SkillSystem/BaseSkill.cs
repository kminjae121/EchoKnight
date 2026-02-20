using _01.Member.KMJ._02.Scripts.UnitSystem.Unit.UnitComponent;
using _Code.KMJ.UnitSystem.Unit.UnitComponent;
using Code.Core.Events.Bus;
using Code.EntityComponent;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;

namespace Code.UnitSystem.SkillSystem
{
    public abstract class BaseSkill : RangeComponent
    {
        [Header("Base Settings")]
        [SerializeField] protected AttackDataSO attackData;
        [field: SerializeField] public Sprite skillImage { get; set; }
        [SerializeField] private float basicSkillDamage;
        public int useSkillPoint;
        [SerializeField] protected bool ownSkill = false;
        
        protected float damage;
        protected DamageData _damageData;
        public float addDamage { get; set; }
        protected Unit _unitBase; 
        public bool isCanUseSkill = false;
        protected GameObject _targetEnemy = null;

        [Header("Unit Component")]
        protected SkillComponent _skillCompo;
        protected UnitRotation rotationCompo;
        protected UnitAnimationTrigger triggerCompo;
        [SerializeField] private UnitStatCompo statCompo;

        [Header("Skill Event")]
        public UnityEvent skillStartEvent;
        public UnityEvent<GameObject> skillEvent;
        public UnityEvent skillEndEvent;

        [Header("Camera & Effects")]
        protected CinemachineImpulseSource impulseSource;
        protected SetUnitCamera unitCam;

        [Header("Materials & Mesh")]
        [SerializeField] protected MeshRenderer ownCircleMesh;
        [SerializeField] protected Material CriticalMaterial;
        [SerializeField] protected Material basicMaterial;
        
        protected override void Awake()
        {
            base.Awake();

            skillEndEvent.AddListener(CanUseSkillTrue);
            skillEvent.AddListener(StartSkill);
            ResetTileEvent += skillEnd;
        }

        protected override void Start()
        {
            base.Start();

            _unitBase = _owner;

            if (_unitBase != null && statCompo == null)
            {
                statCompo = _unitBase.GetUnitCompo<UnitStatCompo>();
            }
            
            if (statCompo != null)
            {
                float skillDamageValue = statCompo.GetStat<float>(StatInfo.SkillDamage);
                float floatdamage = basicSkillDamage * skillDamageValue;
                damage = (int)floatdamage;
            }
            else
            {
                damage = basicSkillDamage;
            }

            _damageData.damage = damage;
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
            ResetTileEvent -= skillEnd;
        }
        
        protected virtual void CanUseSkillTrue()
        {
        }
        
        public virtual void ShowSkillRange()
        {
        }
        
        public void CheckEnemyBody(GameObject target)
        {
            _damageData.damage = damage;
            addDamage = 0;
            
            Vector3 toAttacker = _unitBase.transform.position - target.transform.position;
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

            if (_unitBase.unitSO.EntityType == EntityType.MeleeAttacker && type == BodyType.Head)
            {
                addDamage = _damageData.damage * 0.4f;
                ownCircleMesh.material = CriticalMaterial;
            }
            else if (_unitBase.unitSO.EntityType == EntityType.LongRanger && type == BodyType.Back)
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

        public virtual void CheckCanAttack()
        {
            if (unitCam != null) unitCam.SetThisUnit();
            Bus<UnitAttackControlEvent>.Raise(new UnitAttackControlEvent(true));
            Bus<UnitMoveControlEvent>.Raise(new UnitMoveControlEvent(true));
            FindObjectInRange();
        }

        public virtual void skillEnd()
        {
            BlockThisSkill();
            ResetTile();
            if (unitCam != null) unitCam.EndThisUnit();
            
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
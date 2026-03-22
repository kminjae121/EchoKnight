using Code.Core.Events.Bus;
using Code.Core.Interfaces;
using Code.Map;
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

        public float AddDamage { get; private set; } = 0;
        public UnitRotation rotationCompo { get; set; }
        public float damage { get; set; }
        protected int SkillRange { get; private set; }
        
        
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

        public void ConfigureSkillRange(SkillSO skillData)
        {
            SkillRange = skillData == null ? 0 : Mathf.Max(0, Mathf.RoundToInt(skillData.SkillRange));
        }
        
        
        public virtual void OnDisable()
        {
            skillEndEvent.RemoveListener(CanUseSkillTrue);
        }
        
        public void SetDamage(float damage)
        {
            DamageData.damage = damage += AddDamage;
        }

        private void StartSkill(GameObject arg0)
        {
        }
        public void SetAddDamage(float addDamage)
        {
            AddDamage = addDamage;
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

        protected override int GetRange()
            => SkillRange;

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
                    if (x == 0 && y == 0)
                        continue;

                    Vector2Int position = start + new Vector2Int(x, y);
                    IMapTile tile = GridMap.Instance.GetTile(position);

                    if (tile != null)
                        _tilesInRange.Add(tile);
                }
            }

        }
    }
}

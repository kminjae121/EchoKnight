using System;
using System.Linq;
using _01.Member.KMJ._02.Scripts.UnitSystem.Unit.UnitComponent;
using _Code.KMJ.UnitSystem.Unit.UnitComponent;
using Code.Core.Events.Bus;
using Code.Core.Interfaces;
using Code.EntityComponent;
using EnemySystem;
using Input;
using UnitSystem;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Code.UnitSystem.SkillSystem
{
    public abstract class BaseSkill : RangeComponent
    {
        [Header("Base Settings")]
        [SerializeField] protected AttackDataSO attackData;
        [field: SerializeField] public Sprite skillImage { get; set; }
        [SerializeField] private float basicSkillDamage;
        protected float damage;
        public int useSkillPoint;
        [SerializeField] protected bool ownSkill = false;

        #region UnitComponent
        protected SkillComponent _skillCompo;
        protected UnitRotation rotationCompo;
        protected UnitAnimationTrigger triggerCompo;
        #endregion
        
        protected DamageData _damageData;
        
        protected Unit _unitBase; 
        
        public bool isCanUseSkill = false;

        protected GameObject _targetEnemy = null;
        #region SkillEvent
        public UnityEvent skillStartEvent;
        public UnityEvent<GameObject> skillEvent;
        public UnityEvent skillEndEvent;
        #endregion

        protected CinemachineImpulseSource impulseSource;
        protected SetUnitCamera unitCam;
        
        protected override void Awake()
        {
            base.Awake();

            _unitBase = _owner as Unit;
            

            skillEndEvent.AddListener(CanUseSkillTrue);
            skillEvent.AddListener(StartSkill);
            ResetTileEvent += skillEnd;
        }

        protected override void Start()
        {
            base.Start();
            UnitStatCompo statCompo = _unitBase.GetUnitCompo<UnitStatCompo>();
            
            float skillDamageValue = statCompo.GetStat<float>(StatInfo.SkillDamage);
            
            float floatdamage = basicSkillDamage *= skillDamageValue;
            
            damage = (int)floatdamage;

            
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
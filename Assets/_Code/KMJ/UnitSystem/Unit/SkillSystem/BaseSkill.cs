using System;
using System.Linq;
using _01.Member.KMJ._02.Scripts.UnitSystem.Unit.UnitComponent;
using Code.Core.Events.Bus;
using Code.Core.Interfaces;
using Code.EntityComponent;
using Input;
using UnitSystem;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Code.UnitSystem.SkillSystem
{
    public abstract class BaseSkill : RangeComponent
    {
        [SerializeField] protected AttackDataSO attackData;
        
        #region UnitComponent
            protected SkillComponent _skillCompo;
            private UnitRotation rotationCompo; 
            protected UnitAnimationTrigger triggerCompo;
        #endregion

        [field: SerializeField] public Image skillImage { get; set; }
        
        private InputReader _inputReader;
        
        protected GameObject _targetEnemy = null;
        
        public float damage;

        public int useSkillPoint;

        public bool isCanUseSkill = false;
        
        protected DamageData _damageData;

        private SetUnitCamera unitCam;
        
        private BasicUnit _unit;
        
        #region SkillEvent
            public UnityEvent skillStartEvent;
            public UnityEvent<GameObject> skillEvent;
            public UnityEvent skillEndEvent;

        #endregion

        public virtual void InitializeSkill()
        {
            
        }

        protected override void Awake()
        {
            base.Awake();
            
            _unit = _owner as BasicUnit;

            _inputReader = _unit.inputSO;

            _inputReader.OnAttackEvent += UseSkill;

            rotationCompo = _unit.GetUnitCompo<UnitRotation>();
            triggerCompo = _unit.GetUnitCompo<UnitAnimationTrigger>();
            _skillCompo = _unit.GetUnitCompo<SkillComponent>();

            _damageData.damage = 1.234f;


            unitCam = GameObject.Find("TopCam").GetComponent<SetUnitCamera>();

            ResetTileEvent += skillEnd;
        }

        protected override void Start()
        {
            base.Start();
            
        }


        public virtual void OnDisable()
        {
            skillEndEvent.RemoveListener(TurnEnd);
            _inputReader.OnAttackEvent -= UseSkill;
            ResetTileEvent -= skillEnd;
        }

        public virtual void ShowSkillRange()
        {
            CheckCanAttack();
            CanUseThisSkill();
        }
        
        private void FindEnemyIsThere(GameObject enemy)
        {
            _verticalCollider.ToList().ForEach(obj =>
            {
                if (enemy == obj.gameObject)
                {
                    _targetEnemy = enemy;
                }
            });
            
            _horizontalCollider.ToList().ForEach(obj =>
            {
                if (enemy == obj.gameObject)
                {
                    _targetEnemy = enemy;
                }
            });
        }

        public void CheckCanAttack()
        {
            unitCam.SetThisUnit();
            Bus<UnitAttackControlEvent>.Raise(new UnitAttackControlEvent(true));
            Bus<UnitMoveControlEvent>.Raise(new UnitMoveControlEvent(true));
            FindObjectInRange();
        }
        
        public void skillEnd()
        {
            BlockThisSkill();
            ResetTile();
            unitCam.EndThisUnit();
        }


        private void OnDestroy()
        {
            _inputReader.OnAttackEvent -= UseSkill;
        }
        

        public void AttackEnemy()
        {
            if (isCanUseSkill)
            {
                GameObject enemy = _inputReader.GetEnemy();

                FindEnemyIsThere(enemy);
            
                rotationCompo.SetDir(enemy.transform.position);
            
                skillEvent?.Invoke(_targetEnemy);          
            
            }
            skillEnd();   
        }

        public void TurnEnd()
        {
            _unit.TurnEnd();
                
            BlockThisSkill();
        }
        
        public virtual void UseSkill()
        {
            if (isCanUseSkill == false)
                return;
            
            //if (_skillCompo.currentSkillCost - useSkillPoint < 0)
            //    return;
            //
            //_skillCompo.currentSkillCost -= useSkillPoint;

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
    }
}
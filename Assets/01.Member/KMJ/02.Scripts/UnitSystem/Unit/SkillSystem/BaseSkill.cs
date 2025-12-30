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

namespace Code.UnitSystem.SkillSystem
{
    public abstract class BaseSkill : MonoBehaviour
    {
        [SerializeField] protected UnitAnimationTrigger triggerCompo;
        [SerializeField] protected SkillComponent _skillCompo;
        
        [SerializeField] private Vector3 _attackVerticalCheckBoxSize;
        [SerializeField] private Vector3 _attackHorizontalCheckBoxSize;

        private Collider[] _attackVerticalCollider;
        private Collider[] _attackHorizontalCollider;
        
        [SerializeField] private LayerMask _whatIsGround;
        
        [SerializeField] private UnitRotation rotationCompo; 
        
        [SerializeField] protected AttackDataSO attackData;

        
        [SerializeField] private InputReader _inputReader;
        
        protected GameObject _targetEnemy = null;
        
        public float damage;

        public int useSkillPoint;

        public bool isCanUseSkill = false;
        
        protected DamageData _damageData;

        public UnityEvent skillStartEvent;
        public UnityEvent<GameObject> skillEvent;
        public UnityEvent skillEndEvent;
        
        [SerializeField] protected Unit _owner;
        
        private UnitSO _unitSO;
        
        private BasicUnit _unit;
        

        public virtual void InitializeSkill()
        {
            _inputReader.OnAttackEvent += UseSkill;
            
            _unit = _owner as BasicUnit;

            _inputReader = _unit.inputSO;
            
            _unitSO = _unit.unitSO;
            
            skillEndEvent.AddListener(TurnEnd);

            _damageData.damage = 1.234f;
        }

        public virtual void ShowSkillRange()
        {
            CheckCanAttack();
            CanUseThisSkill();
        }
        
        private void FindEnemyIsThere(GameObject enemy)
        {
            _attackVerticalCollider.ToList().ForEach(obj =>
            {
                if (enemy == obj.gameObject)
                {
                    _targetEnemy = enemy;
                }
            });
            
            _attackHorizontalCollider.ToList().ForEach(obj =>
            {
                if (enemy == obj.gameObject)
                {
                    _targetEnemy = enemy;
                }
            });
        }

        public void CheckCanAttack()
        {
            _attackVerticalCollider = Physics.OverlapBox(transform.position, _attackVerticalCheckBoxSize, Quaternion.identity, _whatIsGround);
            _attackHorizontalCollider = Physics.OverlapBox(transform.position, _attackHorizontalCheckBoxSize, Quaternion.identity, _whatIsGround);
            
            _attackVerticalCollider.ToList().ForEach(obj =>
            {
                if (obj.TryGetComponent(out IMapTile tile))
                {
                    if (!tile.HasObstacle)
                    {
                        tile.SetWalkable(true);
                    }
                }
            });
            
            _attackHorizontalCollider.ToList().ForEach(obj =>
            {
                if (obj.TryGetComponent(out IMapTile tile))
                {
                    if (!tile.HasObstacle)
                    {
                        tile.SetWalkable(true);
                    }
                }
            });
        }
        
        public void ResetTile()
        {
            if (_attackHorizontalCollider == null && _attackVerticalCollider == null)
                return;
            
            _attackHorizontalCollider.ToList().ForEach(obj =>
            {
                if (obj.TryGetComponent(out IMapTile tile))
                {
                    if (!tile.HasObstacle)
                    {
                        tile.SetWalkable(false);
                    }
                }
            });
            
            _attackVerticalCollider.ToList().ForEach(obj =>
            {
                if (obj.TryGetComponent(out IMapTile tile))
                {
                    if (!tile.HasObstacle)
                    {
                        tile.SetWalkable(false);
                    }
                }
            });
            
            _attackHorizontalCollider.ToList().Clear();
            _attackVerticalCollider.ToList().Clear();

            BlockThisSkill();
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
            ResetTile();   
        }

        public void TurnEnd()
        {
            _unit.TurnEnd();
                
            BlockThisSkill();
        }


        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, _attackVerticalCheckBoxSize);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(transform.position, _attackHorizontalCheckBoxSize);
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
        
        public virtual void SkillFeedback()
        {

        }
    }
}
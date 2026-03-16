using Code.Core.Debugs;
using Code.Core.Events.Bus;
using Code.EntityComponent;
using Code.UnitSystem;
using Code.UnitSystem.SkillSystem;
using EnemySystem;
using Input;
using UnityEngine;

namespace UnitSystem
{
    public class CharacterUnitTargeting : MonoBehaviour, IUnitComponent
    {
        [SerializeField] private InputReader inputSO;
        [SerializeField] private UnitBehaviorCompo behaviorCompo;
        [SerializeField] private SkillManageComponent skillManager;

        private GameObject _targetEnemy;
        private Unit _targetUnit;
        private EnemyTargeting _targetingCompo;
        private CharacterUnit _unit;

        public void Initialize(Unit owner)
        {
            _unit = owner as CharacterUnit;
        }

        private void Update()
        {
            HandleTargeting();
        }

        private void HandleTargeting()
        {
            if (!_unit.isMyTurn || inputSO == null)
                return;
            
            if (skillManager.GetSkillInfo() != null && skillManager.GetSkillInfo().IsActive)
            {
                SetSkillTargeting();
                return;
            }
            
            EnemyInfoTargeting();
        }

        private void EnemyInfoTargeting()
        {
            GameObject enemy = inputSO.GetEnemy();

            if (behaviorCompo.VisualPrefabs.activeInHierarchy
                || enemy == null && _targetEnemy != null)
                ClearTarget();
            else if (enemy != null)
                SetTarget(enemy);
        }

        private void SetSkillTargeting()
        {
            GameObject enemy = inputSO.GetEnemy();
            _unit.BehaveCompo.ResetTile();

            if (enemy == null)
            {
                if (_targetEnemy != null)
                {
                    _targetingCompo = _targetEnemy.GetComponent<EnemyTargeting>();

                    if (_targetingCompo != null)
                        _targetingCompo.OffTargeting();

                    Bus<EnemyHpInfo>.Raise(new EnemyHpInfo(0, 0, 0, 0, false,
                        _targetEnemy.GetComponent<Unit>().unitSO.UnitImage, true));
                    Bus<SetAtkUIEvent>.Raise(new SetAtkUIEvent());

                    _targetingCompo = null;
                    skillManager.GetSkillInfo().SetEnemyTargeting(null);
                }
            }
            else
            {
                skillManager.GetSkillInfo().FindEnemyIsThere(enemy);
                
                if (skillManager.GetSkillInfo().GetEnemy() == null) return;
                
                _targetEnemy = enemy;
                
                var skill = skillManager.GetSkillInfo();
                
                if (skill != null)
                {
                    skill.rotationCompo.SetDir(enemy.transform.position);
                    //skill.criticalSpot.CheckEnemyBody(skill.DamageData, enemy, skill.damage, skill.AddDamage);
                }
                
                EntityHealth health = enemy.GetComponent<EntityHealth>();
                _targetingCompo = enemy.GetComponent<EnemyTargeting>();
                
                if (_targetingCompo != null)
                    _targetingCompo.Targeting();
                
                Bus<EnemyHpInfo>.Raise(new EnemyHpInfo(skillManager.GetSkillInfo().AddDamage, health.CurrentHealth,
                    health.MaxHealth,
                    skillManager.GetSkillInfo().DamageData.damage, true,
                    enemy.GetComponent<Unit>().unitSO.UnitImage, true));
                
                skillManager.GetSkillInfo().SetEnemyTargeting(_targetingCompo);
                skillManager.GetSkillInfo().SetEnemy(_targetEnemy);
            }
        }
        

        private void SetTarget(GameObject enemy)
        {
            _targetEnemy = enemy;

            if (_targetEnemy == null)
                return;

            if (_targetingCompo == null)
            {
                _targetingCompo = _targetEnemy.GetComponent<EnemyTargeting>();

                if (_targetingCompo != null)
                    _targetingCompo.Targeting();

                EntityHealth health = _targetEnemy.GetComponent<EntityHealth>();
                Unit unit = _targetEnemy.GetComponent<Unit>();

                Sprite img = (unit != null && unit.unitSO != null) ? unit.unitSO.UnitImage : null;

                float currentHp = health != null ? health.CurrentHealth : 0;
                float maxHp = health != null ? health.MaxHealth : 0;

                Bus<EnemyHpInfo>.Raise(new EnemyHpInfo(0, currentHp, maxHp, 0, true, img, false, 3));
            }
        }

        private void ClearTarget()
        {
            if (_targetEnemy != null)
            {
                if (_targetingCompo == null)
                    _targetingCompo = _targetEnemy.GetComponent<EnemyTargeting>();

                if (_targetingCompo != null)
                    _targetingCompo.OffTargeting();

                Sprite img = null;

                var unit = _targetEnemy.GetComponent<Unit>();

                if (unit != null && unit.unitSO != null)
                    img = unit.unitSO.UnitImage;

                Bus<EnemyHpInfo>.Raise(new EnemyHpInfo(0, 0, 0, 0, false, img, false, 0));
            }

            _targetEnemy = null;
            _targetingCompo = null;
        }
        
        
    }
}
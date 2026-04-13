using System.Collections.Generic;
using Code.Combat.StatusEffect;
using Code.Core.Debugs;
using Code.Core.Events.Bus;
using Code.Managers;
using Code.Map;
using Code.UnitSystem;
using Code.UnitSystem.Enemies;
using UnityEngine;

namespace Code.SkillSystem
{
    public class DragonBreathSkill : EnemyBaseSkill
    { 
        [SerializeField] private int pierceLength = 3;
        [SerializeField] private int burnDuration = 2;
        [SerializeField] private int burnDamage = 5;

        private GameObject _target;
        private AbstractEnemyUnit _ownerEnemy;
        private UnitManager _unitManager;

        private void Awake()
        {
            _ownerEnemy = GetComponentInParent<AbstractEnemyUnit>();
            triggerCompo = _ownerEnemy.GetUnitCompo<UnitAnimationTrigger>();
        }

        protected void Start()
        {
            SkillEvent.AddListener(AttackAction);
            
            if (_ownerEnemy != null)
                _unitManager = _ownerEnemy.UnitManager;
        }

        public override void ForceUseSkill(GameObject target)
        {
            if (target == null)
                return;

            base.ForceUseSkill(target);
            PlayBreathAnimation();
        }

        protected override void StartEvent()
        {
            base.StartEvent();
            triggerCompo.OnAttackTrigger += TakeDamage;
            triggerCompo.OnAnimationEndTrigger += SkillEnd;
        }

        protected override void OnDestroy()
        {
            SkillEvent.RemoveListener(AttackAction);

            if (triggerCompo != null)
            {
                triggerCompo.OnAttackTrigger -= TakeDamage;
                triggerCompo.OnAnimationEndTrigger -= SkillEnd;
            }

            base.OnDestroy();
        }

        private void AttackAction(GameObject target)
        {
            _target = target;
        }

        private void TakeDamage()
        {
            if (_target == null)
                return;

            foreach (GameObject hitTarget in GetHitTargets(_target))
            {
                Bus<DamageEvent>.Raise(new DamageEvent(DamageData, attackData, hitTarget, AddDamage,
                    null, false,false,0.1f));

                if (burnDuration <= 0 || burnDamage <= 0)
                    continue;

                if (!hitTarget.TryGetComponent(out Unit targetUnit))
                    continue;

                Bus<ApplyStatusEffectEvent>.Raise(new ApplyStatusEffectEvent(targetUnit, EffectType.Burn,
                    new StatusEffectApplyData(burnDuration, burnDamage)));
            }

            UnityLogger.Log("범위 공격으로 데미지");
        }

        private bool CanHitTarget(GameObject target)
        {
            if (target == null)
                return false;

            GridMap gridMap = GridMap.Instance;

            if (gridMap == null)
                return false;

            Vector2Int origin = gridMap.WorldToGridPosition(transform.position);
            Vector2Int targetPos = gridMap.WorldToGridPosition(target.transform.position);
            Vector2Int forwardDir = GetForwardDirection(origin, targetPos);

            if (forwardDir == Vector2Int.zero)
                return false;

            for (int i = 1; i <= pierceLength; ++i)
                if (origin + (forwardDir * i) == targetPos)
                    return true;

            return false;
        }

        public override bool CanUseOnTarget(GameObject target)
            => CanHitTarget(target);

        public int GetPredictedHitCount(GameObject target)
            => GetHitTargets(target).Count;

        public override float EvaluateEnemyUseScore(GameObject target)
        {
            if (target == null || SkillSO == null)
                return float.MinValue;

            int predictedHitCount = GetPredictedHitCount(target);
            
            if (predictedHitCount <= 0)
                return float.MinValue;

            return predictedHitCount * SkillSO.SkillDamage;
        }

        private List<GameObject> GetHitTargets(GameObject target)
        {
            var hitTargets = new List<GameObject>();

            if (target == null)
                return hitTargets;

            var gridMap = GridMap.Instance;

            if (gridMap == null)
            {
                UnityLogger.LogError($"[{nameof(DragonBreathSkill)}] GridMap is missing.");
                return hitTargets;
            }

            if (_ownerEnemy != null)
                _unitManager = _ownerEnemy.UnitManager;
            
            if (_unitManager == null)
            {
                UnityLogger.LogError($"[{nameof(DragonBreathSkill)}] UnitManager is missing.");
                return hitTargets;
            }

            Vector2Int origin = gridMap.WorldToGridPosition(transform.position);
            Vector2Int targetPos = gridMap.WorldToGridPosition(target.transform.position);
            Vector2Int forwardDir = GetForwardDirection(origin, targetPos);

            if (forwardDir == Vector2Int.zero)
                return hitTargets;

            var hitTargetSet = new HashSet<GameObject>();

            for (int i = 1; i <= pierceLength; ++i)
            {
                Vector2Int hitPos = origin + forwardDir * i;

                foreach (var unit in _unitManager.GetPlayerUnits())
                {
                    if (unit == null)
                        continue;

                    if (gridMap.WorldToGridPosition(unit.transform.position) != hitPos)
                        continue;

                    if (!hitTargetSet.Add(unit.gameObject))
                        continue;

                    hitTargets.Add(unit.gameObject);
                }
            }

            return hitTargets;
        }

        private void SkillEnd()
        {
            triggerCompo.OnAttackTrigger -= TakeDamage;
            triggerCompo.OnAnimationEndTrigger -= SkillEnd;
            _target = null;
            SkillFinished(false);
            SkillEndEvent?.Invoke();
        }

        private void PlayBreathAnimation()
        {
            if (_ownerEnemy?.UnitAnimator == null || string.IsNullOrWhiteSpace(SkillSO.skillAnimationKey))
                return;
            
            _ownerEnemy.UnitAnimator.PlaySelectAnimation(SkillSO.skillAnimationKey);
        }

        private static Vector2Int GetForwardDirection(Vector2Int origin, Vector2Int target)
        {
            Vector2Int delta = target - origin;

            if (delta == Vector2Int.zero)
                return Vector2Int.zero;

            if (Mathf.Abs(delta.x) >= Mathf.Abs(delta.y))
                return new Vector2Int(delta.x > 0 ? 1 : -1, 0);

            return new Vector2Int(0, delta.y > 0 ? 1 : -1);
        }
    }
}

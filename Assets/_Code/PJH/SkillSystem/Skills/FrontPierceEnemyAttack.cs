using System.Collections.Generic;
using Code.Core.Debugs;
using Code.Core.Events.Bus;
using Code.Managers;
using Code.Map;
using Code.UnitSystem;
using UnityEngine;

namespace Code.SkillSystem
{
    public class FrontPierceEnemyAttack : BaseSkill
    { 
        [SerializeField] private int pierceLength = 3;

        private GameObject _target;
        private UnitManager _unitManager;

        protected void Start()
        {
            SkillEvent.AddListener(AttackAction);
            _unitManager = FindFirstObjectByType<UnitManager>();
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
                Bus<DamageEvent>.Raise(new DamageEvent(DamageData, attackData, hitTarget, AddDamage, null, false));
            
            UnityLogger.Log("관통 공격으로 데미지");
        }

        public bool CanHitTarget(GameObject target)
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

            for (int i = 1; i <= pierceLength; i++)
            {
                if (origin + (forwardDir * i) == targetPos)
                    return true;
            }

            return false;
        }

        public int GetPredictedHitCount(GameObject target)
            => GetHitTargets(target).Count;

        private List<GameObject> GetHitTargets(GameObject target)
        {
            List<GameObject> hitTargets = new List<GameObject>();

            if (target == null)
                return hitTargets;

            GridMap gridMap = GridMap.Instance;

            if (gridMap == null)
            {
                UnityLogger.LogError($"[{nameof(FrontPierceEnemyAttack)}] GridMap is missing.");
                return hitTargets;
            }

            if (_unitManager == null)
                _unitManager = FindFirstObjectByType<UnitManager>();

            if (_unitManager == null)
            {
                UnityLogger.LogError($"[{nameof(FrontPierceEnemyAttack)}] UnitManager is missing.");
                return hitTargets;
            }

            Vector2Int origin = gridMap.WorldToGridPosition(transform.position);
            Vector2Int targetPos = gridMap.WorldToGridPosition(target.transform.position);
            Vector2Int forwardDir = GetForwardDirection(origin, targetPos);

            if (forwardDir == Vector2Int.zero)
                return hitTargets;

            HashSet<GameObject> hitTargetSet = new HashSet<GameObject>();

            for (int i = 1; i <= pierceLength; i++)
            {
                Vector2Int hitPos = origin + forwardDir * i;

                foreach (Unit unit in _unitManager.GetPlayerUnits())
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

using Code.Map;
using Code.Utils;
using Code.UnitSystem.Enemies;
using UnityEngine;

namespace Code.SkillSystem
{
    public abstract class EnemyBaseSkill : BaseSkill
    {
        [Header("AI Positioning")]
        [SerializeField] private bool usePreferredRange;
        [SerializeField, Min(0)] private int preferredRange = 1;
        [SerializeField, Min(0)] private int minSafeRange;

        public virtual bool CanUseOnTarget(GameObject target)
        {
            GridMap gridMap = GridMap.Instance;

            if (gridMap == null)
                return false;

            return CanUseOnTargetFromPosition(gridMap.WorldToGridPos(GetCasterWorldPosition()), target);
        }

        public virtual bool CanUseOnTargetFromPosition(Vector2Int sourcePos, GameObject target)
        {
            if (target == null || SkillSO == null)
                return false;

            GridMap gridMap = GridMap.Instance;

            if (gridMap == null)
                return false;

            Vector2Int targetPos = gridMap.WorldToGridPos(target.transform.position);
            float distance = DistanceUtils.GetEuclideanDistance(sourcePos, targetPos);
            float range = Mathf.Max(0f, SkillSO.SkillRange);

            return distance <= range;
        }

        public virtual float EvaluateEnemyUseScore(GameObject target)
        {
            GridMap gridMap = GridMap.Instance;

            if (gridMap == null)
                return float.MinValue;

            return EvaluateEnemyUseScoreFromPosition(gridMap.WorldToGridPos(GetCasterWorldPosition()), target);
        }

        public virtual float EvaluateEnemyUseScoreFromPosition(Vector2Int sourcePos, GameObject target)
        {
            if (target == null || SkillSO == null)
                return float.MinValue;

            if (!CanUseOnTargetFromPosition(sourcePos, target))
                return float.MinValue;

            return SkillSO.SkillDamage;
        }

        public virtual bool ShouldPreferRepositionFromPosition(Vector2Int sourcePos, GameObject target)
        {
            if (!usePreferredRange || target == null)
                return false;

            GridMap gridMap = GridMap.Instance;

            if (gridMap == null)
                return false;

            float distance = DistanceUtils.GetEuclideanDistance(sourcePos,
                gridMap.WorldToGridPos(target.transform.position));

            return distance < GetMinSafeRange();
        }

        public virtual float EvaluatePositionPreferenceScoreFromPosition(Vector2Int sourcePos, GameObject target)
        {
            if (!usePreferredRange || target == null)
                return 0f;

            GridMap gridMap = GridMap.Instance;

            if (gridMap == null)
                return float.MinValue;

            float distance = DistanceUtils.GetEuclideanDistance(sourcePos,
                gridMap.WorldToGridPos(target.transform.position));
            float score = -Mathf.Abs(distance - GetPreferredRange());

            if (distance < GetMinSafeRange())
                score -= 1000f;

            return score;
        }

        protected Vector3 GetCasterWorldPosition()
        {
            AbstractEnemyUnit ownerEnemy = GetComponentInParent<AbstractEnemyUnit>();
            return ownerEnemy != null ? ownerEnemy.transform.position : transform.position;
        }

        private int GetPreferredRange()
            => Mathf.Max(0, preferredRange);

        private int GetMinSafeRange()
            => Mathf.Clamp(minSafeRange, 0, GetPreferredRange());
    }
}

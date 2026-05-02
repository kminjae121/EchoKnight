using Code.Map;
using Code.Utils;
using Code.UnitSystem.Enemies;
using UnityEngine;

namespace Code.SkillSystem
{
    public abstract class EnemyBaseSkill : BaseSkill
    {
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

        protected Vector3 GetCasterWorldPosition()
        {
            AbstractEnemyUnit ownerEnemy = GetComponentInParent<AbstractEnemyUnit>();
            return ownerEnemy != null ? ownerEnemy.transform.position : transform.position;
        }
    }
}

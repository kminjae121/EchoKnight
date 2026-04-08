using Code.Map;
using Code.Utils;
using UnityEngine;

namespace Code.SkillSystem
{
    public abstract class EnemyBaseSkill : BaseSkill
    {
        public virtual bool CanUseOnTarget(GameObject target)
        {
            if (target == null || SkillSO == null)
                return false;

            GridMap gridMap = GridMap.Instance;
            
            if (gridMap == null)
                return false;

            Vector2Int myPos = gridMap.WorldToGridPosition(transform.position);
            Vector2Int targetPos = gridMap.WorldToGridPosition(target.transform.position);

            float distance = DistanceUtils.GetEuclideanDistance(myPos, targetPos);
            float range = Mathf.Max(0f, SkillSO.SkillRange);

            return distance <= range;
        }

        public virtual float EvaluateEnemyUseScore(GameObject target)
        {
            if (target == null || SkillSO == null)
                return float.MinValue;

            return SkillSO.SkillDamage;
        }
    }
}
using Code.SkillSystem;
using UnityEngine;

namespace Code.UnitSystem.Enemies.AI
{
    public sealed class EnemyPlan
    {
        public Unit Target { get; set; }
        public SkillSO SelectedSkill { get; set; }
        public Vector2Int MoveTile { get; private set; }
        public bool HasMoveTile { get; private set; }

        public bool CanAttackImmediately => Target != null && SelectedSkill != null;

        public void SetMoveTile(Vector2Int moveTile)
        {
            MoveTile = moveTile;
            HasMoveTile = true;
        }

        public void ClearMoveTile()
        {
            MoveTile = default;
            HasMoveTile = false;
        }

        public void Clear()
        {
            Target = null;
            SelectedSkill = null;
            ClearMoveTile();
        }
    }
}
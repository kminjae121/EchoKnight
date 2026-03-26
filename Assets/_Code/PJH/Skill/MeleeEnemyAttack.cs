using Code.UnitSystem.Combat;
using UnityEngine;

namespace Code.SkillSystem
{
    public class MeleeEnemyAttack : BaseSkill
    {
        [SerializeField] private float attackDistance;

        protected override void Start()
        {
            base.Start();

            triggerCompo.OnTakeDamageTrigger += TakeDamage;
        }

        private void TakeDamage()
        {
            // 임펄스랑 데미지
        }
    }
}
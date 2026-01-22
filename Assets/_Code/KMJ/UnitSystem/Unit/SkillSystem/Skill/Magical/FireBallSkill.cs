using System.Collections;
using UnitSystem;
using UnityEngine;

namespace Code.UnitSystem.SkillSystem.Skill
{
    public class FireBallSkill : BaseSkill
    {
        [SerializeField] private GameObject fireBallPrefab;
        
        private UnitAnimation animtionCompo;

        protected override void Start()
        {
            base.Start();
            triggerCompo.OnFireBallTrigger += MakeArrow;
            skillEvent.AddListener(AttackAction);
            animtionCompo = _owner.GetUnitCompo<UnitAnimation>();
        }

        protected override void OnDestroy()
        { 
            triggerCompo.OnFireBallTrigger -= MakeArrow;
            skillEvent.RemoveListener(AttackAction);
            base.OnDestroy();
            
        }
        
        public void AttackAction(GameObject target)
        {
            StartCoroutine(FireBall());
            skillStartEvent?.Invoke();
        }
        
        private IEnumerator FireBall()
        {
            yield return new WaitForSeconds(2f);
            animtionCompo.PlaySelectAnimation("FIREBALL");
        }
        
        public void MakeArrow()
        {
            Vector3 pos = transform.position;

            pos.y += 0.5f;
        
            GameObject slash = Instantiate(fireBallPrefab, pos, Quaternion.identity);

            Vector3 slashRot = transform.rotation.eulerAngles;

            slashRot.y += 90;
        
            slash.transform.rotation = Quaternion.Euler(slashRot);
        }
    }
}
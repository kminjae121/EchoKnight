using System.Collections;
using System.Collections.Generic;
using UnitSystem;
using UnityEngine;

namespace Code.UnitSystem.SkillSystem.Skill.Archer
{
    public class FireArrow : BaseSkill
    {
        [SerializeField] private GameObject _ArrowPrefab;
        
        private UnitAnimation animtionCompo;
        
        private void Start()
        {
            triggerCompo.OnFireArrowTrigger += MakeArrow;
            skillEvent.AddListener(AttackAction);
            animtionCompo = _owner.GetUnitCompo<UnitAnimation>();
        }

        protected override void OnDestroy()
        {
            triggerCompo.OnFireArrowTrigger -= MakeArrow;
            skillEvent.RemoveListener(AttackAction);
            base.OnDestroy();
        }

        public void AttackAction(GameObject target)
        {
            StartCoroutine(FireArrowAction());
            skillStartEvent?.Invoke();
        }
        
        private IEnumerator FireArrowAction()
        {
            yield return new WaitForSeconds(2f);
            animtionCompo.PlaySelectAnimation("FireArrow");
        }
        
        public void MakeArrow()
        {
            Vector3 pos = transform.position;

            pos.y += 0.5f;
        
            GameObject slash = Instantiate(_ArrowPrefab, pos, Quaternion.identity);

            Vector3 slashRot = transform.rotation.eulerAngles;

            slashRot.y += 90;
        
            slash.transform.rotation = Quaternion.Euler(slashRot);
        }
    }
}
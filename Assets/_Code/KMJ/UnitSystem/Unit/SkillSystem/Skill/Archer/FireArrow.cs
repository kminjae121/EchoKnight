using System.Collections;
using System.Collections.Generic;
using Code.UnitSystem.SkillSystem;
using UnitSystem;
using UnityEngine;

    public class FireArrow : BaseSkill
    {
        [SerializeField] private GameObject _ArrowPrefab;
        
        private UnitAnimation animtionCompo;
        
        private void Start()
        {
            triggerCompo.OnFireArrowTrigger += MakeArrow;
            triggerCompo.OnFireArrowEndTrigger += SkillEnd;
            skillEvent.AddListener(AttackAction);
            animtionCompo = _owner.GetUnitCompo<UnitAnimation>();
        }

        protected override void OnDestroy()
        {
            triggerCompo.OnFireArrowTrigger -= MakeArrow;
            triggerCompo.OnFireArrowEndTrigger -= SkillEnd;
            skillEvent.RemoveListener(AttackAction);
            base.OnDestroy();
        }

        public void AttackAction(GameObject target)
        {
            StartCoroutine(FireArrowAction());
            skillStartEvent?.Invoke();
        }
        
        private void SkillEnd()
        {
            skillEndEvent?.Invoke();
            animtionCompo.PlaySelectAnimation("IDLE");
        }
        
        private IEnumerator FireArrowAction()
        {
            yield return new WaitForSeconds(2f);
            animtionCompo.PlaySelectAnimation("FIRE");
        }
        
        public void MakeArrow()
        {
            Vector3 pos = transform.position;

            pos.y += 0.5f;
        
            GameObject shootItem = Instantiate(_ArrowPrefab, pos, Quaternion.identity);

            Vector3 slashRot = transform.rotation.eulerAngles;
        
            shootItem.transform.rotation = Quaternion.Euler(slashRot);
        }
    }
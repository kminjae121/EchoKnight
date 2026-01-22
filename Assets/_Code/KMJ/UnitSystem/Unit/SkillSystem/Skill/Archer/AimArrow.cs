using System.Collections;
using Code.UnitSystem.SkillSystem;
using UnitSystem;
using UnityEngine;

    public class AimArrow : BaseSkill
    {
        [SerializeField] private GameObject _ArrowPrefab;
        
        private UnitAnimation animtionCompo;
        
        private void Start()
        {
            triggerCompo.OnAimArrowTrigger += MakeArrow;
            triggerCompo.OnAimArrowEndTrigger += SkillEnd;
            skillEvent.AddListener(AttackAction);
            animtionCompo = _owner.GetUnitCompo<UnitAnimation>();
        }

        protected override void OnDestroy()
        {
            triggerCompo.OnAimArrowTrigger -= MakeArrow;
            triggerCompo.OnAimArrowEndTrigger -= SkillEnd;
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
            animtionCompo.PlaySelectAnimation("AIM");
        }

        private void SkillEnd()
        {
            skillEndEvent?.Invoke();
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
using System.Collections;
using System.Collections.Generic;
using UnitSystem;
using Unity.Cinemachine;
using UnityEngine;

namespace Code.UnitSystem.SkillSystem.Skill
{
    public class ThrowKnifeSkill : BaseSkill
    {
        [SerializeField] private GameObject _knifePrefab;
        
        private UnitAnimation animtionCompo;
        
        private void Start()
        {
            triggerCompo.OnThrowKnifeTrigger += MakeThrowKnife;
            skillEvent.AddListener(AttackAction);
            animtionCompo = _owner.GetUnitCompo<UnitAnimation>();
        }

        protected override void OnDestroy()
        {
            triggerCompo.OnThrowKnifeTrigger -= MakeThrowKnife;
            skillEvent.RemoveListener(AttackAction);
            base.OnDestroy();
        }

        public void AttackAction(GameObject target)
        {
            StartCoroutine(SlashFlag());
            skillStartEvent?.Invoke();
        }
        
        private IEnumerator SlashFlag()
        {
            yield return new WaitForSeconds(2f);
            animtionCompo.PlaySelectAnimation("THROW");
        }
        
        public void MakeThrowKnife()
        {
            Vector3 pos = transform.position;

            pos.y += 0.5f;
        
            GameObject slash = Instantiate(_knifePrefab, pos, Quaternion.identity);

            Vector3 slashRot = transform.rotation.eulerAngles;

            slashRot.y += 90;
        
            slash.transform.rotation = Quaternion.Euler(slashRot);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using Code.UnitSystem.SkillSystem;
using UnitSystem;
using Unity.Cinemachine;
using UnityEngine;

    public class ThrowKnifeSkill : BaseSkill
    {
        [SerializeField] private GameObject _knifePrefab;
        
        private UnitAnimation animtionCompo;
        
        private void Start()
        {
            triggerCompo.OnThrowKnifeTrigger += MakeThrowKnife;
            triggerCompo.OnThrowKnifeEndTrigger += SkillEnd;
            skillEvent.AddListener(AttackAction);
            animtionCompo = _owner.GetUnitCompo<UnitAnimation>();
        }

        protected override void OnDestroy()
        {
            triggerCompo.OnThrowKnifeTrigger -= MakeThrowKnife;
            triggerCompo.OnThrowKnifeEndTrigger -= SkillEnd;
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
            impulseSource.GenerateImpulse(0.5f);  
            Vector3 pos = transform.position;

            pos.y += 0.5f;
        
            GameObject shootItem = Instantiate(_knifePrefab, pos, Quaternion.identity);

            Vector3 slashRot = transform.rotation.eulerAngles;
        
            shootItem.transform.rotation = Quaternion.Euler(slashRot);
        }
        
        private void SkillEnd()
        {
            skillEndEvent?.Invoke();
            animtionCompo.PlaySelectAnimation("IDLE");
        }
    }
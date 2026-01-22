using System.Collections;
using System.Collections.Generic;
using Code.EntityComponent;
using Code.UnitSystem.SkillSystem;
using UnitSystem;
using UnityEngine;


    public class HealSkill : BaseSkill
    {
        [SerializeField] private GameObject healPrefab;
        
        private UnitAnimation animtionCompo;

        protected override void Start()
        {
            base.Start();
            triggerCompo.OnHealTrigger += Heal;
            triggerCompo.OnHealEndTrigger += SkillEnd;
            skillEvent.AddListener(HealAction);
            animtionCompo = _owner.GetUnitCompo<UnitAnimation>();
        }

        protected override void OnDestroy()
        { 
            triggerCompo.OnHealTrigger-= Heal;
            triggerCompo.OnHealEndTrigger -= SkillEnd;
            skillEvent.RemoveListener(HealAction);
            base.OnDestroy();
            
        }
        
        public void HealAction(GameObject target)
        {
            StartCoroutine(FireBall());
            skillStartEvent?.Invoke();
        }
        
        private IEnumerator FireBall()
        {
            yield return new WaitForSeconds(2f);
            animtionCompo.PlaySelectAnimation("HEAL");
        }
        
        private void SkillEnd()
        {
            skillEndEvent?.Invoke();
        }
        
        public void Heal()
        {
            EntityHealth health = _owner.GetUnitCompo<EntityHealth>();
            
            health.HealHp(20);
        }
    }
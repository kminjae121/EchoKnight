using System.Collections;
using Code.Core.Events.Bus;
using Code.UnitSystem;
using Code.UnitSystem.Combat;
using Code.UnitSystem.SkillSystem;
using UnityEngine;


    public class HealSkill : BasicUnitSkill
    {
        [SerializeField] private GameObject healPrefab;
        
        private UnitAnimation animtionCompo;

        protected override void Start()
        {
            base.Start();
            SkillType = SkillType.ActiveSkill;
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
            Bus<UnitSetMoveEvent>.Raise(new UnitSetMoveEvent(false));
            yield return new WaitForSeconds(0.3f);
            yield return new WaitForSeconds(0.1f);
            animtionCompo.PlaySelectAnimation("HEAL");
        }

        protected override void SkillEnd()
        {
            base.SkillEnd();
            skillEndEvent?.Invoke();
            Bus<TurnEndUIEvent>.Raise(new TurnEndUIEvent(false));
            animtionCompo.PlaySelectAnimation("IDLE");
        }

        public void Heal()
        {
            UnitHealth health = _owner.GetUnitCompo<UnitHealth>();
            
            health.HealHp(20);
            healPrefab.SetActive(true);
            healPrefab.GetComponent<ParticleSystem>().Play();
        }
    }
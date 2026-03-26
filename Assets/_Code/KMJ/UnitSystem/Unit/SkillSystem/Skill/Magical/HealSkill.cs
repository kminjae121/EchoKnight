using System.Collections;
using Code.Core.Events.Bus;
using Code.UnitSystem;
using Code.UnitSystem.Combat;
using Code.UnitSystem.SkillSystem;
using UnityEngine;
using UnityEngine.UIElements;


public class HealSkill : BasicUnitSkill
    {
        [SerializeField] private GameObject healPrefab;
        
        private UnitAnimation animtionCompo;

        protected override void Start()
        {
            base.Start();
            SkillEvent.AddListener(HealAction);
            animtionCompo = _owner.GetUnitCompo<UnitAnimation>();
        }

        protected override void StartEvent()
        {
            base.StartEvent();
            triggerCompo.OnAttackTrigger += Heal;
            triggerCompo.OnAnimationEndTrigger += SkillEnd;
        }

        protected override void OnDestroy()
        { 
            SkillEvent.RemoveListener(HealAction);
            base.OnDestroy();
            
        }
        
        public void HealAction(GameObject target)
        {
            SkillStartEvent?.Invoke();
            StartCoroutine(FireBall());
        }
        
        private IEnumerator FireBall()
        {
            Bus<UnitSetMoveEvent>.Raise(new UnitSetMoveEvent(false));
            yield return new WaitForSeconds(0.4f);
            animtionCompo.PlaySelectAnimation("HEAL");
        }

        protected override void SkillEnd()
        {
            base.SkillEnd();
            triggerCompo.OnAttackTrigger-= Heal;
            triggerCompo.OnAnimationEndTrigger -= SkillEnd;
            SkillEndEvent?.Invoke();
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
using System.Collections;
using Code.Core.Events.Bus;
using Code.UnitSystem;
using Code.SkillSystem;
using UnityEngine;

    public class AddAvoideProbablity : BasicUnitSkill
    {
        [SerializeField] private GameObject effectPrefab;
        
        private UnitAnimation animtionCompo;

        private int skillCnt = 0;

        protected void Start()
        {
            SkillEvent.AddListener(AddAP);
            animtionCompo = _characterUnit.GetUnitCompo<UnitAnimation>();
        }

        protected override void StartEvent()
        {
            base.StartEvent();
            triggerCompo.OnAttackTrigger += PlusAvoideProbablity;
            triggerCompo.OnAnimationEndTrigger += SkillEnd;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            SkillEvent.RemoveListener(AddAP);
        }

        private void AddAP(GameObject obj)
        {
            SkillStartEvent?.Invoke();
            StartCoroutine(AddAvoid());
        }

        private IEnumerator AddAvoid()
        {
            Bus<UnitSetMoveEvent>.Raise(new UnitSetMoveEvent(false));
            yield return new WaitForSeconds(0.3f);
            yield return new WaitForSeconds(0.1f);
            effectPrefab.SetActive(true);
            effectPrefab.GetComponent<ParticleSystem>().Play();
            animtionCompo.PlaySelectAnimation("HEAL");
        }

        private void PlusAvoideProbablity()
        {
            if (skillCnt >= 3)
            {
                _characterUnit.InitilizeAvoideProbability();
                return;
            }

            skillCnt += 1;
            
            _characterUnit.AddAvoideProbability += 10;
            _characterUnit.unitSO.AvoidProbability += 10;
        }
        
        protected override void SkillEnd()
        {
            base.SkillEnd();
            triggerCompo.OnAnimationEndTrigger -= SkillEnd;
            triggerCompo.OnAttackTrigger -= PlusAvoideProbablity;
            SkillEndEvent?.Invoke();
            Bus<TurnEndUIEvent>.Raise(new TurnEndUIEvent(false));
            animtionCompo.PlaySelectAnimation("IDLE");
        }
    }
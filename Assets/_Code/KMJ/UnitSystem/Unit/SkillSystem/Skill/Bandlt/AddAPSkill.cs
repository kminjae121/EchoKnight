using System.Collections;
using Code.Core.Events.Bus;
using Code.UnitSystem;
using Code.UnitSystem.SkillSystem;
using UnitSystem;
using Unity.VisualScripting;
using UnityEngine;

    public class AddAPSkill : BasicUnitSkill
    {
        [SerializeField] private GameObject effectPrefab;
        
        private UnitAnimation animtionCompo;

        protected override void Start()
        {
            base.Start();
            skillEvent.AddListener(AddAP);
            animtionCompo = _owner.GetUnitCompo<UnitAnimation>();
            triggerCompo.OnAddAPTrigger += PlusAP;

            triggerCompo.OnAddAPEndTrigger += SkillEnd;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            skillEvent.RemoveListener(AddAP);
            triggerCompo.OnAddAPEndTrigger -= SkillEnd;
            triggerCompo.OnAddAPTrigger -= PlusAP;
        }

        private void AddAP(GameObject obj)
        {
            skillStartEvent?.Invoke();
            StartCoroutine(AddAp());
        }

        private IEnumerator AddAp()
        {
           
            yield return new WaitForSeconds(0.3f);
            yield return new WaitForSeconds(0.1f);
            effectPrefab.SetActive(true);
            effectPrefab.GetComponent<ParticleSystem>().Play();
            animtionCompo.PlaySelectAnimation("HEAL");
        }

        private void PlusAP()
        {
            BasicUnit unit = _owner as BasicUnit;

            unit.GetCost(25);
        }
        
        private void SkillEnd()
        {
            skillEndEvent?.Invoke();
            Bus<SetAtkUIEvent>.Raise(new SetAtkUIEvent(false));
            Bus<TurnEndUIEvent>.Raise(new TurnEndUIEvent(false));
            animtionCompo.PlaySelectAnimation("IDLE");
            Bus<UnitCamSettingEvent>.Raise(new UnitCamSettingEvent(null, false,new Vector3(0.1f,0.1f,0.1f)));
            Bus<UnitSetMoveEvent>.Raise(new UnitSetMoveEvent(true));
        }
    }
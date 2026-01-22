using System.Collections;
using UnitSystem;
using Unity.VisualScripting;
using UnityEngine;

namespace Code.UnitSystem.SkillSystem.Skill
{
    public class AddAPSkill : BaseSkill
    {
        [SerializeField] private UnitAnimationTrigger triggerCompo;

        [SerializeField] private GameObject effectPrefab;
        
        private UnitAnimation animtionCompo;

        protected override void Start()
        {
            base.Start();
            skillEvent.AddListener(AddAP);
            animtionCompo = _owner.GetUnitCompo<UnitAnimation>();
            triggerCompo.OnAddAPTrigger += PlusAP;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            skillEvent.RemoveListener(AddAP);
            triggerCompo.OnAddAPTrigger -= PlusAP;
        }

        private void AddAP(GameObject obj)
        {
            skillStartEvent?.Invoke();
            StartCoroutine(AddAp());
        }

        private IEnumerator AddAp()
        {
            yield return new WaitForSeconds(2f);
            animtionCompo.PlaySelectAnimation("HEAL");
        }

        private void PlusAP()
        {
            BasicUnit unit = _owner as BasicUnit;

            unit.GetCost(25);
        }
    }
}
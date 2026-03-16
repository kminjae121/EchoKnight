using System.Collections;
using Code.Core.Events.Bus;
using Code.UnitSystem;
using Code.UnitSystem.SkillSystem;
using UnitSystem;
using UnityEngine;

    public class LongAttack : BasicUnitSkill
    {
        [SerializeField] private float atkMoveSpeed;

        [SerializeField] private Animator animator;

        [SerializeField] private UnitAnimation animtionCompo;

        [SerializeField] private GameObject effectPrefab;
        
        private GameObject _target = null;
        
        public bool isRunningAttack = false;
        
        private Vector3 _ownTrm;

        private void Start()
        {
            triggerCompo.OnLongRangeAttackTrigger += ShootLongRangeAttack;
            triggerCompo.OnLongRangeAttackEndTrigger += AttackEnd;
            skillEvent.AddListener(AttackAction);
        }

        private void OnDestroy()
        {
            triggerCompo.OnLongRangeAttackTrigger -= ShootLongRangeAttack;
            triggerCompo.OnLongRangeAttackEndTrigger -= AttackEnd;
            skillEvent.RemoveListener(AttackAction);
        }

        public void AttackAction(GameObject target)
        {
            _ownTrm = transform.position;
            
            StartCoroutine(MeleeAttackAction(target));
        }

        private IEnumerator MeleeAttackAction(GameObject target)
        {
            yield return new WaitForSeconds(0.4f);
            
             _target = target;
             
             animtionCompo.PlaySelectAnimation("ATTACK");
        }


        private void ShootLongRangeAttack()
        {
            Vector3 dir = _target.transform.position;
            dir.y += 1.4f;
            
            effectPrefab.GetComponent<BoomingEffect>().SetDamageData(DamageData);
            effectPrefab.transform.position = dir;
            effectPrefab.SetActive(true);
        }


        private void AttackEnd()
        {
            animtionCompo.PlaySelectAnimation("IDLE");
            skillEndEvent?.Invoke();
            
            Bus<SetAtkUIEvent>.Raise(new SetAtkUIEvent(false));
            Bus<UnitSetMoveEvent>.Raise(new UnitSetMoveEvent(true));
            Bus<UnitCamSettingEvent>.Raise(new UnitCamSettingEvent(null, false,new Vector3(0.1f,0.1f,0.1f)));
        }
    }
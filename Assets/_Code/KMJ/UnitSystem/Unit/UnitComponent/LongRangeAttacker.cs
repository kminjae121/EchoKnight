using System.Collections;
using System.Collections.Generic;
using _Code.KMJ.UnitSystem.Unit.UnitComponent;
using Code.Core.Events.Bus;
using Code.UnitSystem;
using UnitSystem;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class LongRangeAttacker : MonoBehaviour
{
        [SerializeField] private UnitAttackComponent atkCompo;

        [SerializeField] private float atkMoveSpeed;

        [SerializeField] private Animator animator;

        [SerializeField] private UnitAnimation animtionCompo;

        [SerializeField] private UnitAnimationTrigger triggerCompo;

        [SerializeField] private GameObject effectPrefab;


        private GameObject _target = null;
        
        public bool isRunningAttack = false;
        
        private Vector3 _ownTrm;

        private void Start()
        {
            triggerCompo.OnLongRangeAttackTrigger += ShootLongRangeAttack;
            triggerCompo.OnLongRangeAttackEndTrigger += AttackEnd;
            atkCompo.attackEvent.AddListener(AttackAction);
        }

        private void OnDestroy()
        {
            triggerCompo.OnLongRangeAttackTrigger -= ShootLongRangeAttack;
            triggerCompo.OnLongRangeAttackEndTrigger -= AttackEnd;
            atkCompo.attackEvent.RemoveListener(AttackAction);
        }

        public void AttackAction(GameObject target)
        {
            _ownTrm = transform.position;
            StartCoroutine(MeleeAttackAction(target));
        }

        private IEnumerator MeleeAttackAction(GameObject target)
        {
            
            yield return new WaitForSeconds(0.3f);
            _target = null;
            
            yield return new WaitForSeconds(0.1f);
            
             animtionCompo.PlaySelectAnimation("ATTACK");

             _target = target;
        }


        private void ShootLongRangeAttack()
        {
            Vector3 dir = _target.transform.position;

            dir.y += 1.4f;
            
            effectPrefab.GetComponent<BoomingEffect>().SetDamageData(atkCompo._damageData);
            effectPrefab.transform.position = dir;
            
            effectPrefab.SetActive(true);
        }


        private void AttackEnd()
        {
            animtionCompo.PlaySelectAnimation("IDLE");
            atkCompo.attackEndEvent?.Invoke();
            Bus<SetAtkUIEvent>.Raise(new SetAtkUIEvent(false));
            Bus<UnitSetMoveEvent>.Raise(new UnitSetMoveEvent(true));
            Bus<UnitCamSettingEvent>.Raise(new UnitCamSettingEvent(null, false,new Vector3(0.1f,0.1f,0.1f)));
        }
}

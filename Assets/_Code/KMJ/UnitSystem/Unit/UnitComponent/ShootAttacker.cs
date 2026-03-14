using System.Collections;
using Code.AttackSystem;
using Code.Core.Events.Bus;
using Code.UnitSystem;
using UnitSystem;
using Unity.Cinemachine;
using UnityEngine;

namespace Code.UnitSystem
{
    public class ShootAttacker : MonoBehaviour
    {
        [SerializeField] private UnitAttackComponent atkCompo;

        [SerializeField] private float atkMoveSpeed;

        [SerializeField] private Animator animator;

        [SerializeField] private UnitAnimation animtionCompo;

        [SerializeField] private UnitAnimationTrigger triggerCompo;

        private ShootItemAttackManager _shootItemManager;
        
        
        private CinemachineImpulseSource impulseSource;
        
        private GameObject _target = null;

        private void Start()
        {
            triggerCompo.OnShootAttackTrigger += Shoot;
            triggerCompo.OnShootAttackEndTrigger += AttackEnd;
            atkCompo.attckExecutor.attackEvent.AddListener(AttackAction);
            _shootItemManager = GetComponentInChildren<ShootItemAttackManager>();
        }

        private void OnDestroy()
        {
            triggerCompo.OnShootAttackTrigger -= Shoot;
            triggerCompo.OnShootAttackEndTrigger -= AttackEnd;
            atkCompo.attckExecutor.attackEvent.RemoveListener(AttackAction);
        }

        public void AttackAction(GameObject target)
        {
            StartCoroutine(ShootAttackSet(target));
        }

        private IEnumerator ShootAttackSet(GameObject target)
        {
            yield return new WaitForSeconds(0.4f);
            
            _target = null;
            _target = target;
            
            animtionCompo.PlaySelectAnimation("ATTACK");
        }

        private void Shoot()
        {
            Vector3 pos = transform.position;

            pos.y += 1.6f;
            
            Vector3 slashRot = transform.rotation.eulerAngles;
            
            _shootItemManager.SetTarget(_target);
            _shootItemManager.SetDamageData(atkCompo.attckExecutor.GetDamageData());
            _shootItemManager.CreateShootItem("ShootItem",pos, slashRot);

            atkCompo.CharacterUnit.impulseSource.GenerateImpulse(0.3f);
        }
        
        private void AttackEnd()
        {
            animtionCompo.PlaySelectAnimation("IDLE");
            
            Bus<SetAtkUIEvent>.Raise(new SetAtkUIEvent(false));
            Bus<UnitCamSettingEvent>.Raise(new UnitCamSettingEvent(null, false,new Vector3(0.1f,0.1f,0.1f)));
            Bus<UnitSetMoveEvent>.Raise(new UnitSetMoveEvent(true));
            
            atkCompo.attckExecutor.attackEndEvent?.Invoke();
        }
    }
}
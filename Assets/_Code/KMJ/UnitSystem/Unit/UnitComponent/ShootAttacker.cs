using System.Collections;
using Code.Core.Events.Bus;
using Code.UnitSystem;
using UnitSystem;
using Unity.Cinemachine;
using UnityEngine;

namespace _Code.KMJ.UnitSystem.Unit.UnitComponent
{
    public class ShootAttacker : MonoBehaviour
    {
        [SerializeField] private UnitAttackComponent atkCompo;

        [SerializeField] private float atkMoveSpeed;

        [SerializeField] private Animator animator;

        [SerializeField] private UnitAnimation animtionCompo;

        [SerializeField] private UnitAnimationTrigger triggerCompo;

        [SerializeField] private float attackMoveDistance = 1.5f;

        [SerializeField] private GameObject shootPrefabs;
        
        private CinemachineImpulseSource impulseSource;
        
        private GameObject _target = null;
        
        public bool isRunningAttack = false;
        
        private Vector3 _ownTrm;

        private void Start()
        {
            triggerCompo.OnShootAttackTrigger += Shoot;
            triggerCompo.OnShootAttackEndTrigger += AttackEnd;
            atkCompo.attackEvent.AddListener(AttackAction);
            impulseSource = GameObject.Find("ImpulseSource").GetComponent<CinemachineImpulseSource>();
        }

        private void OnDestroy()
        {
            triggerCompo.OnShootAttackTrigger -= Shoot;
            triggerCompo.OnShootAttackEndTrigger -= AttackEnd;
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
            
            _target = target;
            animtionCompo.PlaySelectAnimation("ATTACK");

        }

        private void Shoot()
        {
            Vector3 pos = transform.position;

            pos.y += 0.5f;
        
            GameObject shootItem = Instantiate(shootPrefabs, pos, Quaternion.identity);

            Vector3 slashRot = transform.rotation.eulerAngles;

            ShootItem shootItemCompo = shootItem.GetComponent<ShootItem>();
            shootItemCompo.SetTarget(_target);
            shootItemCompo.SetDamageData(atkCompo._damageData);
            
            shootItem.transform.rotation = Quaternion.Euler(slashRot);
            impulseSource.GenerateImpulse(0.3f);
        }
        
        private void AttackEnd()
        {
            animtionCompo.PlaySelectAnimation("IDLE");
            Bus<SetAtkUIEvent>.Raise(new SetAtkUIEvent(false));
            Bus<UnitCamSettingEvent>.Raise(new UnitCamSettingEvent(null, false));
            atkCompo.attackEndEvent?.Invoke();
        }
    }
}
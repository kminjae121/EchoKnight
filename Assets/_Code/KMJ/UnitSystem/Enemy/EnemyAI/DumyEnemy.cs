 using _Code.Core.Managers;
using Code.Core.Events.Bus;
using Code.UnitSystem;
using UnityEngine;

namespace EnemySystem
{
    public class DumyEnemy : Unit
    {
        [SerializeField] private UnitAnimation animationCompo;
        [SerializeField] private UnitAnimationTrigger triggerCompo;
        [SerializeField] private ParticleSystem bloodParticles;
        
        private void Start()
        {
            Bus<UnitSpawnEvent>.Raise(new UnitSpawnEvent(this));
            animationCompo.PlaySelectAnimation("IDLE");
            triggerCompo.OnEnemyAnimationEndTrigger += ChangeIdle;
            triggerCompo.OnEnemyDieEndTrigger += Die;
        }
        
        public override void OnTurnStart()
        {
            TurnEnd();
        }
        
        private void Die()
        {
            gameObject.SetActive(false);
            StageManager.Instance.RemoveEnemy(this.gameObject);
        }

        private void ChangeIdle()
        {
            animationCompo.PlaySelectAnimation("IDLE");
        }

        protected override void Dead()
        {
            DeadEnemy();
            base.Dead();
        }
        
        

        public void TurnEnd()
        {
            Bus<UnitTurnEndEvent>.Raise(new UnitTurnEndEvent(this));
        }

        protected override void Hit()
        {
            bloodParticles.gameObject.SetActive(true);
            bloodParticles.Play();
            animationCompo.PlaySelectAnimation("IDLE");
            animationCompo.PlaySelectAnimation("HIT");
            base.Hit();
        }

        public void DeadEnemy()
        {
            animationCompo.PlaySelectAnimation("IDLE");
            animationCompo.PlaySelectAnimation("DIE");
        }
    }
}
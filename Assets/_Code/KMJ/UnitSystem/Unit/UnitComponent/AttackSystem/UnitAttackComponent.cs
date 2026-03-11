using Code.Core.Events.Bus;
using Code.UnitSystem;
using Input;
using UnitSystem;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

namespace Code.AttackSystem
{
    public class UnitAttackComponent : MonoBehaviour, IUnitComponent
    {
        [field: SerializeField] public CriticalSpot CriticalSpot { get; private set; }
        [field: SerializeField] public AttackPresenter atkPresenter { get; private set; }
        [field: SerializeField] public AttackExecutor attckExecutor { get; private set; }
        [field: SerializeField] public AttackTargetSelector attackTargetSelector { get; private set; }

        public CharacterUnit CharacterUnit { get; set; }
        
        private InputReader _inputReader;

        public UnityEvent attackStartEvent;
        
        public void Initialize(Unit owner)
        {
            CharacterUnit = owner as CharacterUnit;
            
            Bus<UnitAttackEvent>.Subscribe(CheckCanAttack);
            attckExecutor.attackEndEvent.AddListener(AttackEnded);
            
            _inputReader = CharacterUnit.InputSO;
            _inputReader.OnAttackEvent += AttackEnemy;
        }
        

        private void OnDestroy()
        {
            attckExecutor.attackEndEvent?.RemoveListener(AttackEnded);

            if (_inputReader != null)
                _inputReader.OnAttackEvent -= AttackEnemy;

            Bus<UnitAttackEvent>.Unsubscribe(CheckCanAttack);
        }
        


        public void CheckCanAttack(UnitAttackEvent evt)
        {
            if (evt.isAttack)
            {
                if (!CharacterUnit.isMyTurn) return;
                atkPresenter.ShowAttackUI(true);

                attackStartEvent?.Invoke();
                attackTargetSelector.FindObjectInRange();
            }
            else
            {
                atkPresenter.ShowAttackUI(false);

                attackTargetSelector.ResetTile();
                attackTargetSelector.EndAct();
            }
        }

        public void AttackEnemy()
        {
            if (!(CharacterUnit.isMyTurn && attackTargetSelector.IsActive)) return;
            
            var enemy = _inputReader.GetEnemy();
            attackTargetSelector.FindEnemyIsThere(enemy);
            
            if (attackTargetSelector._targetEnemy == null) return;
            
            atkPresenter.ShowAttackUI(false);
            attackTargetSelector._targetingCompo?.OffTargeting();
            attackTargetSelector.ResetTile();   
            AttackStart();
        }

        private void AttackStart()
        {
            if (attackTargetSelector._targetEnemy == null) return;
            
            attckExecutor.TryExecute(attackTargetSelector._targetEnemy);
            atkPresenter.SetCamTarget(CharacterUnit.gameObject);
            atkPresenter.OnAttackExecuted();
        }

        private void AttackEnded()
        {
            CharacterUnit.BehaveCompo.ReCheckInRange();
            Bus<UnitSetMoveEvent>.Raise(new UnitSetMoveEvent(true));
        }
    }
}
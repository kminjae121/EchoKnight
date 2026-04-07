using System.Collections;
using _Code.Passive;
using Code.Core.Managers;
using Code.Core.Events.Bus;
using Code.Core.Interfaces;
using Code.Managers;
using Code.SkillSystem;
using Code.UI;
using Code.UnitSystem.Combat;
using Input;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Code.UnitSystem
{
    public class CharacterUnit : Unit
    {
        [Header("Basic Unit Refs")]
        [field: SerializeField] public InputReader InputSO { get; private set; }
        [SerializeField] private LayerMask whatIsGround;
        [SerializeField] private Image unitImage;

        #region UnitCompo
        
        public UnitHealth HealthCompo { get; private set; }

        public UnitMoveCompo MoveCompo { get; private set; }
        [field:SerializeField] public SkillComponent SkillCompo { get; private set; }
        public UnitAnimationTrigger TriggerCompo { get; private set; }
        public UnitManageRangeCompo UnitRangeCompo { get; private set; }
        public UnitStatCompo UnitStatCompo { get; private set; }
        public UnitSkillCost SkillCostCompo { get; private set; }
        
        public PassiveComponent PassiveCompo { get; private set; }
        
        public UnitOutLineCompo OutLineCompo { get; private set; }

        #endregion
        
        public int PlayableUnitID { get; set; } = -1;
        public bool IsConfirmationSkill { get; set; }
        
        public GameObject _startTile;
        private Button endTurnBtn;
        
        private readonly Vector3 _dampingSpeed = new(1.5f,1.5f,1.5f);

        public UnityEvent OnTurnStartEvent;
        public UnityEvent OnTurnEndEvent;

        private void Start()
        {
            TriggerCompo = GetUnitCompo<UnitAnimationTrigger>();
            MoveCompo = GetUnitCompo<UnitMoveCompo>();
            UnitRangeCompo =  GetUnitCompo<UnitManageRangeCompo>();
            UnitStatCompo = GetUnitCompo<UnitStatCompo>();
            SkillCostCompo =  GetUnitCompo<UnitSkillCost>();
            OutLineCompo =  GetUnitCompo<UnitOutLineCompo>();
            PassiveCompo = GetUnitCompo<PassiveComponent>();
            HealthCompo = GetUnitCompo<UnitHealth>();   
            
            Bus<SetAtkUIEvent>.Raise(new SetAtkUIEvent(false));

            if (TriggerCompo != null)
                TriggerCompo.OnDeadEvent += HandleDieAnimationEnd;

            MoveCompo.CurrentMapTile = _startTile.GetComponent<IMapTile>();
            
            if(_startTile != null)
                transform.position = _startTile.transform.position;
            
            Bus<SetAtkUIEvent>.Raise(new SetAtkUIEvent(true));
        }
        
        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            if (TriggerCompo != null)
                TriggerCompo.OnDeadEvent -= HandleDieAnimationEnd;
        }

        public void SetObject(Button btn,SkillCostUI skillCostUI)
        {
            endTurnBtn = btn;
        }

        public override void OnTurnStart()
        {
            base.OnTurnStart();
            Bus<UnitCamSettingEvent>.Raise(new UnitCamSettingEvent(gameObject, false,_dampingSpeed));
            Bus<SetAtkUIEvent>.Raise(new SetAtkUIEvent(true));
            Bus<TurnEndUIEvent>.Raise(new TurnEndUIEvent(false));

            SkillCompo.ResetSkillsCount();
            
            SkillCostCompo.AddSkillCost();
            
            SkillCompo.UpdateSkillUI();
            
            PassiveCompo.StartAllAlwaysPassives();
            
            if (endTurnBtn != null)
                endTurnBtn.onClick.AddListener(TurnEnd);

            if (MoveCompo != null)
            {
                MoveCompo.FindObjectInRange(unitSO.MoveRange);
                MoveCompo.moveCount = 0;
            }
            
            OnTurnStartEvent?.Invoke();
            
            Bus<WhatUnitTurnEvent>.Raise(new WhatUnitTurnEvent(unitSO.UnitType));
        }

        public override void OnTurnEnd()
        {
            base.OnTurnEnd();
            OnTurnEndEvent?.Invoke();
            PassiveCompo.StopAllAlwaysPassives();
            Bus<UnitMoveControlEvent>.Raise(new UnitMoveControlEvent(true));
            Bus<UnitAttackControlEvent>.Raise(new UnitAttackControlEvent(true));
        }

        protected override void Hit()
        {
            if (AnimationCompo != null)
            {
                AnimationCompo.RestartFromEntry();
                AnimationCompo.PlaySelectAnimation("HIT");
                StartCoroutine(ReturnIdleAnimation());
            }
            base.Hit();
        }

        public void TurnEnd()
        {
            if (isMyTurn)
            {
                UnitRangeCompo.RemoveAllRange();
                if (endTurnBtn != null)
                    endTurnBtn.onClick.RemoveListener(TurnEnd);
                
                OnTurnEnd();
            }
        }

        
        public void HandleDieAnimationEnd()
        {
            gameObject.SetActive(false);
            
            if (StageManager.Instance != null)
                StageManager.Instance.PlayerDie();
        }

        public void Die()
        {
            if (AnimationCompo != null)
                AnimationCompo.PlaySelectAnimation("DEAD");
        }

        protected override void Dead()
        {
            base.Dead();
            Die();
        }
        
        private IEnumerator ReturnIdleAnimation()
        {
            yield return new WaitForSeconds(1.5f);
            AnimationCompo.ReturnIdleAnimation();
        }
    }
}
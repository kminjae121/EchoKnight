using System;
using System.Linq;
using _Code.Core.Managers;
using Code.Core.Events.Bus;
using Code.Core.Interfaces;
using Code.Managers;
using Code.UI;
using Code.UnitSystem;
using Code.UnitSystem.SkillSystem;
using GameEventChannel;
using Input;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace  UnitSystem
{
    public class BasicUnit : Unit
    {
        public TurnCostGaugeManager gaugeManager { get; set; }
        [field: SerializeField] public InputReader inputSO { get; private set; }
        
        [SerializeField] private GameEventChannelSO unitDeadChannel;

        [SerializeField] private LayerMask whatIsGround;

        public SkillComponent skillCompo { get; private set; }
        public UnitAnimation animationComponent { get; private set; }
        
        public UnitAnimationTrigger triggerCompo { get; private set; }

        public int maxUsingCost = 100;

        private UnitControl _controlUI;

        private Button endTurnBtn;

        public float CurrentCost { get; private set; } = 100;
        

        [SerializeField] private Image unitImage;

        public int PlayableUnitID { get; set; } = -1;

        private UnitMovement movementCompo;

        public GameObject _startTile = null;
        
        
        private void Start()
        {
            gaugeManager = GameObject.Find("TurnManager").GetComponent<TurnCostGaugeManager>();
            _controlUI = GameObject.Find("BaseButton").GetComponent<UnitControl>();
            endTurnBtn = GameObject.Find("TurnEndBtn").GetComponent<Button>();

            skillCompo = GetUnitCompo<SkillComponent>();
            triggerCompo = GetUnitCompo<UnitAnimationTrigger>();
            movementCompo = GetUnitCompo<UnitMovement>();
            
            animationComponent = GetUnitCompo<UnitAnimation>();

            triggerCompo.OnDeadEvent += LastDie;

            CurrentCost = 100;

            movementCompo._currentMapTile = _startTile;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            triggerCompo.OnDeadEvent -= LastDie;
        }

        public override void OnTurnStart()
        {
            CurrentCost = maxUsingCost;

            float value = Mathf.Clamp01(CurrentCost / maxUsingCost);

            int idx = -1;

            for (int i = 0; i <= 2; i++)
            {
                Bus<SkillUIEvent>.Raise(new SkillUIEvent(i, null,null,null));
            }
            if (skillCompo.skills != null)
            {
                 skillCompo.skills.ToList().ForEach(skill =>
                 {
                     idx += 1;
                     Bus<SkillUIEvent>.Raise(new SkillUIEvent(idx, skill.Key,skill.Value.skillImage,skillCompo));
                 });  
            }
            
            endTurnBtn.onClick.AddListener(TurnEnd);
            
            Bus<ApSliderEvent>.Raise(new ApSliderEvent(value));
            OnStartTurnEvent?.Invoke();
            base.OnTurnStart();
            isMyTurn = true;
        }

        public override void OnTurnEnd()
        {
            Bus<UnitMoveControlEvent>.Raise(new UnitMoveControlEvent(true));
            Bus<UnitAttackControlEvent>.Raise(new UnitAttackControlEvent(true));
            base.OnTurnEnd();
        }

        protected override void Hit()
        {
            animationComponent.RestartFromEntry();
            animationComponent.PlaySelectAnimation("HIT");
            base.Hit();
        }
        

        public void TurnEnd()
        {
            if (isMyTurn == true)
            {
                endTurnBtn.onClick.RemoveListener(TurnEnd);
                Bus<UnitTurnEndEvent>.Raise(new UnitTurnEndEvent(this));
            }

        }

        protected override void Dead()
        {
            base.Dead();
            Die();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
        }

        private void OnValidate()
        {
            if (unitSO != null)
            {
                gameObject.name = unitSO.UnitName;
            }
        }

        public bool GetCost(int cost)
        {
            if (CurrentCost >= maxUsingCost || CurrentCost + cost >= maxUsingCost)
                return false;
            
            CurrentCost += cost;
            float value = Mathf.Clamp01(CurrentCost / maxUsingCost);
            
            Bus<ApSliderEvent>.Raise(new ApSliderEvent(value));
            return true;
        }


        public float GetCurrentCost()
        {
            return CurrentCost;
        }

        public void RemoveCost(float cost)
        {
            CurrentCost -= cost;
            
            if (CurrentCost <= 0)
            {
                CurrentCost = 0;
            }
            //코스트 줄어드는중
            
            float value = Mathf.Clamp01(CurrentCost / maxUsingCost);
            
            Bus<ApSliderEvent>.Raise(new ApSliderEvent(value));
        }


        public void SelectThisUnit(bool isSelected)
        {
            
        }

        public void Die()
        {
            animationComponent.PlaySelectAnimation("DEAD");
        }

        public void LastDie()
        {
            gameObject.SetActive(false);
            StageManager.Instance.PlayerDie();
        }
    }
}
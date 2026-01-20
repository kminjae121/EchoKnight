using System;
using System.Linq;
using Code.Core.Events.Bus;
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
        [field: SerializeField] public InputReader inputSO { get; private set; }
        
        [SerializeField] private GameEventChannelSO unitDeadChannel;

        public SkillComponent skillCompo { get; private set; }
        public UnitAnimation animationComponent { get; private set; }

        public int maxUsingCost = 100;

        private UnitControl _controlUI;

        private Button endTurnBtn;
        
        public float CurrentCost { get; private set; }
        

        [SerializeField] private Image unitImage;
        
        
        private void Start()
        {
            _controlUI = GameObject.Find("BaseButton").GetComponent<UnitControl>();
            endTurnBtn = GameObject.Find("TurnEnd").GetComponent<Button>();

            skillCompo = GetUnitCompo<SkillComponent>();
            
            animationComponent = GetUnitCompo<UnitAnimation>();
            
            endTurnBtn.onClick.AddListener(TurnEnd);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            endTurnBtn.onClick.RemoveListener(TurnEnd);
        }

        public override void OnTurnStart()
        {
            isMyTurn = true;
            CurrentCost = maxUsingCost;

            float value = Mathf.Clamp01(CurrentCost / maxUsingCost);

            int idx = -1;

            if (skillCompo.skills == null)
            {
                 skillCompo.skills.ToList().ForEach(skill =>
                 {
                     idx += 1;
                     Bus<SkillUIEvent>.Raise(new SkillUIEvent(idx, skill.Key,skill.Value.skillImage,skillCompo));
                 });  
            }
            
            Bus<ApSliderEvent>.Raise(new ApSliderEvent(value));
            OnStartTurnEvent?.Invoke();
            base.OnTurnStart();
        }

        public override void OnTurnEnd()
        {
            isMyTurn = false;
            
            base.OnTurnEnd();
            TurnEnd();
            
            Bus<UnitMoveControlEvent>.Raise(new UnitMoveControlEvent(true));
            Bus<UnitAttackControlEvent>.Raise(new UnitAttackControlEvent(true));
        }

        protected override void Hit()
        {
            animationComponent.PlaySelectAnimation("HIT");
            base.Hit();
        }
        

        public void TurnEnd()
        {
            if (isMyTurn)
            {
                OnEndTurnEvent?.Invoke();
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
            return true;
        }


        public float GetCurrentCost()
        {
            return CurrentCost;
        }

        public void RemoveCost(float cost)
        {
            CurrentCost -= cost;
            
            Debug.Log(CurrentCost);
            
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
    }
}
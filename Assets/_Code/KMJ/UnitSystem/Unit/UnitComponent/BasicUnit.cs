using System;
using System.Linq;
using _Code.Core.Managers;
using _Code.KMJ.UnitSystem.Unit.UnitComponent;
using Code.Core.Events.Bus;
using Code.Core.Interfaces;
using Code.EntityComponent;
using Code.Managers;
using Code.UI;
using Code.UnitManaging;
using Code.UnitSystem;
using Code.UnitSystem.SkillSystem;
using EnemySystem;
using GameEventChannel;
using Input;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace  UnitSystem
{
    public class BasicUnit : Unit
    {
        public UnitBehavaveCompo behaveCompo { get; set; }
        public TurnCostGaugeManager gaugeManager { get; set; }
        [field: SerializeField] public InputReader inputSO { get; private set; }
        
        [SerializeField] private GameEventChannelSO unitDeadChannel;

        [SerializeField] private LayerMask whatIsGround;

        public SkillComponent skillCompo { get; private set; }
        public UnitAnimation animationComponent { get; private set; }
        
        public UnitAnimationTrigger triggerCompo { get; private set; }
        
        public UnitAttackComponent atkCompo { get; private set; }

        private UnitControl _controlUI;

        private Button endTurnBtn;
        

        [SerializeField] private Image unitImage;

        public int PlayableUnitID { get; set; } = -1;

        private UnitMovement movementCompo;

        public GameObject _startTile = null;
        
        private GameObject _targetEnemy = null;
        private EnemyTargeting _targetingCompo = null;
        
        
        private void Start()
        {
            gaugeManager = GameObject.Find("TurnManager").GetComponent<TurnCostGaugeManager>();
            _controlUI = GameObject.Find("BaseButton").GetComponent<UnitControl>();
            endTurnBtn = GameObject.Find("TurnEndBtn").GetComponent<Button>();

            skillCompo = GetUnitCompo<SkillComponent>();
            triggerCompo = GetUnitCompo<UnitAnimationTrigger>();
            //movementCompo = GetUnitCompo<UnitMovement>();
            behaveCompo = GetUnitCompo<UnitBehavaveCompo>();
            atkCompo = GetUnitCompo<UnitAttackComponent>();
            
            animationComponent = GetUnitCompo<UnitAnimation>();
            
            Bus<UnitSetMoveEvent>.Subscribe(StartWalk);

            triggerCompo.OnDeadEvent += LastDie;

            //movementCompo._currentMapTile = _startTile;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            triggerCompo.OnDeadEvent -= LastDie;
        }

        public override void OnTurnStart()
        {
            Bus<UnitCamSettingEvent>.Raise(new UnitCamSettingEvent(this.gameObject, false));
            OwnUnitManage.Instance.currentCost += 20;

            float value = Mathf.Clamp01(OwnUnitManage.Instance.currentCost / 100);

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
            behaveCompo.FindObjectInRange();
            Bus<TurnEndUIEvent>.Raise(new TurnEndUIEvent(false));
            isMyTurn = true;
        }

        public override void OnTurnEnd()
        {
            Bus<UnitMoveControlEvent>.Raise(new UnitMoveControlEvent(true));
            Bus<UnitAttackControlEvent>.Raise(new UnitAttackControlEvent(true));
            behaveCompo.ResetTile();
            base.OnTurnEnd();
        }
        

        public void StartWalk(UnitSetMoveEvent evt)
        {
            if (isMyTurn)
            {
                behaveCompo.ReCheckInRange();
            }
        }

        protected override void Hit()
        {
            animationComponent.RestartFromEntry();
            animationComponent.PlaySelectAnimation("HIT");
            base.Hit();
        }

        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.Space))
            {
                Bus<UnitCamSettingEvent>.Raise(new UnitCamSettingEvent(this.gameObject, false));
            }

            if (isMyTurn && !atkCompo._isAct)
            {
                GameObject enemy = inputSO.GetEnemy();

                if(enemy == null && _targetEnemy != null)
                {
                    _targetingCompo = _targetEnemy.GetComponent<EnemyTargeting>();
                    
                    _targetingCompo.OffTargeting();
                    Bus<EnemyHpInfo>.Raise(new EnemyHpInfo(0,0,0, 
                        0, false,_targetEnemy.GetComponent<Unit>().unitSO.UnitImage,false,0));

                    _targetingCompo = null;
                }
                else if (enemy != null)
                {
                    _targetEnemy = enemy;
                    if (_targetEnemy != null && _targetingCompo == null)
                    {
                        EntityHealth health = _targetEnemy.GetComponent<EntityHealth>();
                        
                        _targetingCompo = _targetEnemy.GetComponent<EnemyTargeting>();
                        _targetingCompo.Targeting();
                        
                        Bus<EnemyHpInfo>.Raise(new EnemyHpInfo(0,health.CurrentHealth,health.MaxHealth, 
                            0, true,_targetEnemy.GetComponent<Unit>().unitSO.UnitImage,false,3));
                    }
                }
            }
            else
            {
                if (_targetEnemy != null && _targetingCompo != null) 
                {
                    _targetingCompo = _targetEnemy.GetComponent<EnemyTargeting>();
                    _targetingCompo.OffTargeting();
                    
                    Bus<EnemyHpInfo>.Raise(new EnemyHpInfo(0,0,0, 
                        0, false,_targetEnemy.GetComponent<Unit>().unitSO.UnitImage,false,0));
                    _targetingCompo = null;
                }
            }
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
            if (OwnUnitManage.Instance.currentCost >= 100 || OwnUnitManage.Instance.currentCost + cost >= 100)
                return false;
            
            OwnUnitManage.Instance.currentCost += cost;
            float value = Mathf.Clamp01(OwnUnitManage.Instance.currentCost / 100);
            
            Bus<ApSliderEvent>.Raise(new ApSliderEvent(value));
            return true;
        }


        public float GetCurrentCost()
        {
            return OwnUnitManage.Instance.currentCost;
        }

        public void RemoveCost(float cost)
        {
            OwnUnitManage.Instance.currentCost -= cost;
            
            if (OwnUnitManage.Instance.currentCost <= 0)
            {
                OwnUnitManage.Instance.currentCost = 0;
            }
            //코스트 줄어드는중
            
            float value = Mathf.Clamp01(OwnUnitManage.Instance.currentCost / 100);
            
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
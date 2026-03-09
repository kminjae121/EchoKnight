using System.Collections;
using _Code.Core.Managers;
using _Code.KMJ.UnitSystem.Unit.UnitComponent;
using Code.Core.Events.Bus;
using Code.Managers;
using Code.UI;
using Code.UnitManaging;
using Code.UnitSystem;
using Code.UnitSystem.SkillSystem;
using EnemySystem;
using Input;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

namespace UnitSystem
{
    public class CharacterUnit : Unit
    {
        [Header("Basic Unit Refs")]
        [field: SerializeField] public InputReader InputSO { get; private set; }
        [SerializeField] private LayerMask whatIsGround;
        [SerializeField] private Image unitImage;

        public UnitBehaviorCompo BehaveCompo { get; set; }
        public SkillComponent SkillCompo { get; private set; }
        public UnitAnimationTrigger TriggerCompo { get; private set; }
        
        public UnitManageRangeCompo UnitRangeCompo { get; private set; }
        
        public UnitStatCompo UnitStatCompo { get; private set; }
        
        public UnitCostComponent UnitCostComponentCompo { get; private set; }
        public int PlayableUnitID { get; set; } = -1;
        public GameObject _startTile = null;
        public TurnCostGaugeManager GaugeManager { get; set; }
        private Button endTurnBtn;
        public CinemachineImpulseSource impulseSource { get; private set; }

        private void Start()
        {
            SkillCompo = GetUnitCompo<SkillComponent>();
            TriggerCompo = GetUnitCompo<UnitAnimationTrigger>();
            BehaveCompo = GetUnitCompo<UnitBehaviorCompo>();
            UnitRangeCompo =  GetUnitCompo<UnitManageRangeCompo>();
            UnitStatCompo = GetUnitCompo<UnitStatCompo>();
            UnitCostComponentCompo = GetUnitCompo<UnitCostComponent>();
            
            Bus<SetAtkUIEvent>.Raise(new SetAtkUIEvent(false));

            if (TriggerCompo != null)
                TriggerCompo.OnDeadEvent += HandleDieAnimationEnd;

            BehaveCompo._currentMapTile = _startTile;
            
            transform.position = _startTile.transform.position;
            
            Bus<SetAtkUIEvent>.Raise(new SetAtkUIEvent(true));
        }
        
        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            if (TriggerCompo != null)
                TriggerCompo.OnDeadEvent -= HandleDieAnimationEnd;
        }

        public void SetObject(TurnCostGaugeManager manager, Button btn,CinemachineImpulseSource source)
        {
            GaugeManager = manager;
            endTurnBtn = btn;
            impulseSource = source;
        }

        public override void OnTurnStart()
        {
            Bus<UnitCamSettingEvent>.Raise(new UnitCamSettingEvent(gameObject, false,new Vector3(1.5f,1.5f,1.5f)));
            Bus<SetAtkUIEvent>.Raise(new SetAtkUIEvent(false));
            
            if (OwnUnitManage.Instance != null)
                OwnUnitManage.Instance.currentCost += 20;

            UnitCostComponentCompo.UpdateAPGauge();
            SkillCompo.UpdateSkillUI();

            if (endTurnBtn != null)
                endTurnBtn.onClick.AddListener(TurnEnd);
            
            OnStartTurnEvent?.Invoke();
            base.OnTurnStart();
            
            if (BehaveCompo != null)
                BehaveCompo.FindObjectInRange();
                
            Bus<TurnEndUIEvent>.Raise(new TurnEndUIEvent(false));
            
            Bus<SetAtkUIEvent>.Raise(new SetAtkUIEvent(false));
            isMyTurn = true;
        }

        public override void OnTurnEnd()
        {
            Bus<UnitMoveControlEvent>.Raise(new UnitMoveControlEvent(true));
            Bus<UnitAttackControlEvent>.Raise(new UnitAttackControlEvent(true));
            
            if (BehaveCompo != null)
                BehaveCompo.ResetTile();
            
            UnitRangeCompo.RemoveAllRange();
            
            Bus<SetAtkUIEvent>.Raise(new SetAtkUIEvent(true));
            base.OnTurnEnd();
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
                if (endTurnBtn != null)
                    endTurnBtn.onClick.RemoveListener(TurnEnd);
                
                OnTurnEnd();
                Bus<UnitTurnEndEvent>.Raise(new UnitTurnEndEvent(this));
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
using System;
using System.Collections;
using System.Runtime.InteropServices.ComTypes;
using _Code.Core.Managers;
using _Code.KMJ.UnitSystem.Unit.UnitComponent;
using Code.Core.Events.Bus;
using Code.EntityComponent;
using Code.Managers;
using Code.UI;
using Code.UnitManaging;
using Code.UnitSystem;
using Code.UnitSystem.SkillSystem;
using EnemySystem;
using Input;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UnitSystem
{
    public class BasicUnit : Unit
    {
        [Header("Basic Unit Refs")]
        [field: SerializeField] public InputReader inputSO { get; private set; }
        [SerializeField] private LayerMask whatIsGround;
        [SerializeField] private Image unitImage;

        public UnitBehavaveCompo behaveCompo { get; set; }
        public TurnCostGaugeManager gaugeManager { get; set; }
        public SkillComponent skillCompo { get; private set; }
        public UnitAnimationTrigger triggerCompo { get; private set; }
        public UnitAttackComponent atkCompo { get; private set; }
        
        public UnitManageRangeCompo unitRangeCompo { get; private set; }
        
        public UnitStatCompo unitStatCompo { get; private set; }

        public int PlayableUnitID { get; set; } = -1;
        public GameObject _startTile = null;

        private Button endTurnBtn;
        private UnitControl _controlUI;
        private GameObject _targetEnemy = null;
        private EnemyTargeting _targetingCompo = null;

        private void Start()
        {
            var turnManagerObj = GameObject.Find("TurnManager");
            if (turnManagerObj) gaugeManager = turnManagerObj.GetComponent<TurnCostGaugeManager>();

            var baseBtnObj = GameObject.Find("BaseButton");
            if (baseBtnObj) _controlUI = baseBtnObj.GetComponent<UnitControl>();

            var endTurnBtnObj = GameObject.Find("TurnEndBtn");
            if (endTurnBtnObj) endTurnBtn = endTurnBtnObj.GetComponent<Button>();

            skillCompo = GetUnitCompo<SkillComponent>();
            triggerCompo = GetUnitCompo<UnitAnimationTrigger>();
            behaveCompo = GetUnitCompo<UnitBehavaveCompo>();
            unitRangeCompo =  GetUnitCompo<UnitManageRangeCompo>();
            atkCompo = GetUnitCompo<UnitAttackComponent>();
            unitStatCompo = GetUnitCompo<UnitStatCompo>();
            
            Bus<UnitSetMoveEvent>.Subscribe(StartWalk);
            
            Bus<SetAtkUIEvent>.Raise(new SetAtkUIEvent(false));

            if (triggerCompo != null)
                triggerCompo.OnDeadEvent += LastDie;

            behaveCompo._currentMapTile = _startTile;
            
            transform.position = _startTile.transform.position;
            
            Bus<SetAtkUIEvent>.Raise(new SetAtkUIEvent(true));
        }

        protected override void OnDestroy()
        {
            if (triggerCompo != null)
                triggerCompo.OnDeadEvent -= LastDie;
            
            Bus<UnitSetMoveEvent>.Unsubscribe(StartWalk);
            base.OnDestroy();
        }

        public override void OnTurnStart()
        {
            Bus<UnitCamSettingEvent>.Raise(new UnitCamSettingEvent(this.gameObject, false,new Vector3(1.5f,1.5f,1.5f)));
            Bus<SetAtkUIEvent>.Raise(new SetAtkUIEvent(false));
            
            if (OwnUnitManage.Instance != null)
                OwnUnitManage.Instance.currentCost += 20;

            UpdateAPGauge();
            UpdateSkillUI();

            if (endTurnBtn != null)
                endTurnBtn.onClick.AddListener(TurnEnd);
            
            OnStartTurnEvent?.Invoke();
            base.OnTurnStart();
            
            if (behaveCompo != null)
                behaveCompo.FindObjectInRange();
                
            Bus<TurnEndUIEvent>.Raise(new TurnEndUIEvent(false));
            
            Bus<SetAtkUIEvent>.Raise(new SetAtkUIEvent(false));
            isMyTurn = true;
        }

        public override void OnTurnEnd()
        {
            Bus<UnitMoveControlEvent>.Raise(new UnitMoveControlEvent(true));
            Bus<UnitAttackControlEvent>.Raise(new UnitAttackControlEvent(true));
            
            if (behaveCompo != null)
                behaveCompo.ResetTile();
            unitRangeCompo.RemoveAllRange();
            
            Bus<SetAtkUIEvent>.Raise(new SetAtkUIEvent(true));
            base.OnTurnEnd();
        }

        public void StartWalk(UnitSetMoveEvent evt)
        {
            if (isMyTurn && behaveCompo != null && evt.isStart == false)
            {
                behaveCompo.ResetTile();
            }
            else if (isMyTurn && behaveCompo != null && evt.isStart == true)
            {
                behaveCompo.FindObjectInRange();
            }
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

        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.Space))
            {
                Bus<UnitCamSettingEvent>.Raise(new UnitCamSettingEvent(this.gameObject, false,new Vector3(1.5f,1.5f,1.5f)));
            }

            if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
            {
                RangesCompo.RemoveAllRange();
                Bus<UnitSetMoveEvent>.Raise(new UnitSetMoveEvent(true));
                Bus<SetAtkUIEvent>.Raise(new SetAtkUIEvent(false));
            }

            HandleTargeting();
        }

        private void HandleTargeting()
        {
            if (!isMyTurn || inputSO == null) return;
            if (atkCompo != null && atkCompo._isAct) return;

            GameObject enemy = inputSO.GetEnemy();

            if (behaveCompo.visualPrefabs.activeInHierarchy)
            {
                ClearTarget();
            }
            else if (enemy == null && _targetEnemy != null)
            {
                ClearTarget();
            }
            else if (enemy != null)
            {
                SetTarget(enemy);
            }
        }

        private IEnumerator ReturnIdleAnimation()
        {
            yield return new WaitForSeconds(1.5f);
            AnimationCompo.ReturnIdleAnimation();
        }

        private void SetTarget(GameObject enemy)
        {
            _targetEnemy = enemy;
            if (_targetEnemy == null) return;

            if (_targetingCompo == null)
            {
                _targetingCompo = _targetEnemy.GetComponent<EnemyTargeting>();
                if (_targetingCompo != null) _targetingCompo.Targeting();

                var health = _targetEnemy.GetComponent<EntityHealth>();
                var unit = _targetEnemy.GetComponent<Unit>();
                
                Sprite img = (unit != null && unit.unitSO != null) ? unit.unitSO.UnitImage : null;
                float currentHp = health != null ? health.CurrentHealth : 0;
                float maxHp = health != null ? health.MaxHealth : 0;

                Bus<EnemyHpInfo>.Raise(new EnemyHpInfo(0, currentHp, maxHp, 0, true, img, false, 3));
            }
        }

        private void ClearTarget()
        {
            if (_targetEnemy != null)
            {
                if (_targetingCompo == null) _targetingCompo = _targetEnemy.GetComponent<EnemyTargeting>();
                if (_targetingCompo != null) _targetingCompo.OffTargeting();

                Sprite img = null;
                var unit = _targetEnemy.GetComponent<Unit>();
                if (unit != null && unit.unitSO != null) img = unit.unitSO.UnitImage;

                Bus<EnemyHpInfo>.Raise(new EnemyHpInfo(0, 0, 0, 0, false, img, false, 0));
            }
            _targetEnemy = null;
            _targetingCompo = null;
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

        protected override void Dead()
        {
            base.Dead();
            Die();
        }

        private void OnValidate()
        {
            if (unitSO != null) gameObject.name = unitSO.UnitName;
        }

        public bool GetCost(int cost)
        {
            if (OwnUnitManage.Instance == null) return false;
            if (OwnUnitManage.Instance.currentCost >= 100 || OwnUnitManage.Instance.currentCost + cost >= 100)
                return false;

            OwnUnitManage.Instance.currentCost += cost;
            UpdateAPGauge();
            return true;
        }

        public float GetCurrentCost()
        {
            return OwnUnitManage.Instance != null ? OwnUnitManage.Instance.currentCost : 0;
        }

        public void RemoveCost(float cost)
        {
            if (OwnUnitManage.Instance == null) return;

            OwnUnitManage.Instance.currentCost -= cost;
            if (OwnUnitManage.Instance.currentCost <= 0) OwnUnitManage.Instance.currentCost = 0;
            
            UpdateAPGauge();
        }

        private void UpdateAPGauge()
        {
            if (OwnUnitManage.Instance == null) return;
            float value = Mathf.Clamp01(OwnUnitManage.Instance.currentCost / 100);
            Bus<ApSliderEvent>.Raise(new ApSliderEvent(value));
        }

        private void UpdateSkillUI()
        {
            for (int i = 0; i <= 2; i++) Bus<SkillUIEvent>.Raise(new SkillUIEvent(i, null,0, null, null));
            
            if (skillCompo != null && skillCompo.skills != null)
            {
                int idx = 0;
                foreach (var skill in skillCompo.skills)
                {
                    Bus<SkillUIEvent>.Raise(new SkillUIEvent(idx, skill.Key, skill.Value.useSkillPoint,skill.Value.skillImage, skillCompo));
                    idx++;
                }
            }
        }

        public void SelectThisUnit(bool isSelected) { }

        public void Die()
        {
            if (AnimationCompo != null)
                AnimationCompo.PlaySelectAnimation("DEAD");
        }

        public void LastDie()
        {
            gameObject.SetActive(false);
            
            if (StageManager.Instance != null)
                StageManager.Instance.PlayerDie();
        }
    }
}